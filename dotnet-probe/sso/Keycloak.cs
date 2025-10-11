using Microsoft.Identity.Client;
using Serilog;

namespace dotnet_probe.sso;

public static class Keycloak
{
    public static async Task<AuthenticationResult?> ExchangeTokenWithKeycloak(AuthenticationResult entraIdResult)
    {
        Log.Information("Starting token exchange with Keycloak");
        var subjectToken = entraIdResult.AccessToken;

        if (string.IsNullOrEmpty(subjectToken))
        {
            Log.Error("No EntraId access token available for exchange");
            return null;
        }

        var keycloakEndpoint = Environment.GetEnvironmentVariable("KEYCLOAK_TOKEN_ENDPOINT")
                               ?? "http://localhost:8080/realms/sso-probe/protocol/openid-connect/token";
        var clientId = Environment.GetEnvironmentVariable("KEYCLOAK_CLIENT_ID");
        var clientSecret = Environment.GetEnvironmentVariable("KEYCLOAK_CLIENT_SECRET");

        if (string.IsNullOrEmpty(clientId))
        {
            Log.Error("Keycloak client id not configured (env KEYCLOAK_CLIENT_ID)");
            return null;
        }

        if (string.IsNullOrEmpty(clientSecret))
        {
            Log.Error("Keycloak client secret not configured (env KEYCLOAK_CLIENT_SECRET)");
            return null;
        }

        try
        {
            using var http = new System.Net.Http.HttpClient();
            Log.Information("Exchanging token with Keycloak at {Endpoint} for client {ClientId}", keycloakEndpoint,
                clientId);
            var form = new List<KeyValuePair<string, string>>
            {
                new("grant_type", "urn:ietf:params:oauth:grant-type:token-exchange"),
                new("subject_token", subjectToken),
                // new("subject_token_type", "urn:ietf:params:oauth:token-type:id_token"),
                new("subject_token_type", "urn:ietf:params:oauth:token-type:access_token"),
                new("subject_issues", "entraid-saml"),
                // new("requested_token_type", "urn:ietf:params:oauth:token-type:id_token"),
                new("client_id", clientId),
                new("client_secret", clientSecret),
                new("scope", "openid"),
            };

            var content = new System.Net.Http.FormUrlEncodedContent(form);
            Log.Debug("Posting token-exchange to Keycloak endpoint {Endpoint}", keycloakEndpoint);
            var resp = await http.PostAsync(keycloakEndpoint, content);
            var body = await resp.Content.ReadAsStringAsync();

            if (!resp.IsSuccessStatusCode)
            {
                Log.Error("Keycloak token exchange failed ({Status}): {Body}", (int)resp.StatusCode, body);
                return null;
            }

            using var doc = System.Text.Json.JsonDocument.Parse(body);
            var root = doc.RootElement;

            string? kcAccess =
                root.TryGetProperty("access_token", out var at) && at.ValueKind == System.Text.Json.JsonValueKind.String
                    ? at.GetString()
                    : null;
            string? kcIdToken =
                root.TryGetProperty("id_token", out var it) && it.ValueKind == System.Text.Json.JsonValueKind.String
                    ? it.GetString()
                    : null;
            int expiresIn = 0;
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

            string? scope =
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