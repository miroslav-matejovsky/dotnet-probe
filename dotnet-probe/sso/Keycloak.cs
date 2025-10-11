using Microsoft.Identity.Client;
using Serilog;

namespace dotnet_probe.sso;

public record KeycloakClientConfig(
    string Url,
    string Realm,
    string ClientId,
    string ClientSecret,
    string EntraIdProvider
)
{
    public string TokenEndpoint => $"{Url}/realms/{Realm}/protocol/openid-connect/token";
}

public class Keycloak(KeycloakClientConfig config)
{
    public async Task<AuthenticationResult?> TokenExchange(AuthenticationResult entraIdResult)
    {
        Log.Information("Starting token exchange with Keycloak");
        var subjectToken = entraIdResult.AccessToken;
        // var subjectToken = entraIdResult.IdToken;
        if (string.IsNullOrEmpty(subjectToken))
        {
            Log.Error("No EntraId access token available for exchange");
            return null;
        }

        var tokenEndpoint = config.TokenEndpoint;
        var clientId = config.ClientId;
        var clientSecret = config.ClientSecret;

        try
        {
            using var http = new System.Net.Http.HttpClient();
            Log.Information("Exchanging token with Keycloak at {Endpoint} for client {ClientId}", tokenEndpoint,
                clientId);
            var form = new List<KeyValuePair<string, string>>
            {
                new("grant_type", "urn:ietf:params:oauth:grant-type:token-exchange"),
                new("subject_token", subjectToken),
                // new("subject_token_type", "urn:ietf:params:oauth:token-type:id_token"),
                new("subject_token_type", "urn:ietf:params:oauth:token-type:access_token"),
                new("subject_issues", config.EntraIdProvider),
                new("requested_token_type", "urn:ietf:params:oauth:token-type:id_token"),
                new("client_id", clientId),
                new("client_secret", clientSecret),
                new("scope", "openid"),
            };

            var content = new System.Net.Http.FormUrlEncodedContent(form);
            Log.Debug("Posting token-exchange to Keycloak endpoint {Endpoint}", tokenEndpoint);
            var resp = await http.PostAsync(tokenEndpoint, content);
            var body = await resp.Content.ReadAsStringAsync();

            if (!resp.IsSuccessStatusCode)
            {
                Log.Error("Keycloak token exchange failed ({Status}): {Body}", (int)resp.StatusCode, body);
                return null;
            }

            using var doc = System.Text.Json.JsonDocument.Parse(body);
            var root = doc.RootElement;

            var kcAccess =
                root.TryGetProperty("access_token", out var at) && at.ValueKind == System.Text.Json.JsonValueKind.String
                    ? at.GetString()
                    : null;
            var kcIdToken =
                root.TryGetProperty("id_token", out var it) && it.ValueKind == System.Text.Json.JsonValueKind.String
                    ? it.GetString()
                    : null;
            var expiresIn = 0;
            if (root.TryGetProperty("expires_in", out var ei))
            {
                if (ei.ValueKind == System.Text.Json.JsonValueKind.Number && ei.TryGetInt32(out var val))
                {
                    expiresIn = val;
                }
                else
                {
                    int.TryParse(ei.GetString(), out expiresIn);
                }
            }

            var scope =
                root.TryGetProperty("scope", out var sc) && sc.ValueKind == System.Text.Json.JsonValueKind.String
                    ? sc.GetString()
                    : null;

            Log.Information(
                "Keycloak exchange succeeded. AccessTokenLength: {Len}, HasIdToken: {HasId}, ExpiresIn: {Expires}, Scope: {Scope}",
                kcAccess?.Length ?? 0, !string.IsNullOrEmpty(kcIdToken), expiresIn, scope ?? "(none)");


            // Note: MSAL's AuthenticationResult cannot be constructed here; return null after performing the exchange.
            return null;
        }
        catch (Exception ex)
        {
            Log.Error("Exception during Keycloak token exchange: {Error}", ex.Message);
            return null;
        }
    }
}