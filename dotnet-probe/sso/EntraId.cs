using System.Windows;
using Microsoft.Identity.Client;
using Microsoft.Identity.Client.Broker;
using Serilog;

namespace dotnet_probe.sso;

public record EntraIdClientConfig(string ClientId, string TenantId);

public static class EntraId
{
    public static async Task<AuthenticationResult?> AuthenticateUserViaMaw(Window window, string tenantId,
        string clientId)
    {
        if (string.IsNullOrEmpty(clientId))
        {
            Log.Error("Client ID is empty");
            return null;
        }

        var acquireScopes = new[] { "User.Read" };

        var brokerOptions = new BrokerOptions(BrokerOptions.OperatingSystems.Windows)
        {
            Title = "SSO WPF Probe"
        };

        var applicationOptions = new PublicClientApplicationOptions
        {
            TenantId = tenantId,
            ClientId = clientId
        };

        Log.Information("Acquiring Window handle for authentication UI via WAM");
        var wih = new System.Windows.Interop.WindowInteropHelper(window);
        var hWnd = wih.Handle;

        var app =
            PublicClientApplicationBuilder
                .CreateWithApplicationOptions(applicationOptions)
                .WithDefaultRedirectUri()
                .WithParentActivityOrWindow(() => hWnd)
                .WithBroker(brokerOptions)
                .Build();

        AuthenticationResult? result;
        Log.Information("Trying to get previously signed-in user or use OS account for silent authentication");
        var accounts = await app.GetAccountsAsync();
        var existingAccount = accounts.FirstOrDefault();
        try
        {
            if (existingAccount != null)
            {
                Log.Debug("Found existing account in cache: {Username}, {HomeAccountId}", existingAccount.Username,
                    existingAccount.HomeAccountId);
                result = await app.AcquireTokenSilent(acquireScopes, existingAccount).ExecuteAsync();
            }
            // Next, try to sign in silently with the account that the user is signed into Windows
            else
            {
                Log.Debug("No existing account in cache, trying OS account");
                result = await app.AcquireTokenSilent(acquireScopes, PublicClientApplication.OperatingSystemAccount)
                    .ExecuteAsync();
            }
        }
        // Can't get a token silently, go interactive
        catch (MsalUiRequiredException ex)
        {
            Log.Warning("Silent token acquisition failed: {Error}", ex.Message);
            Log.Information("Falling back to interactive authentication");
            try
            {
                result = await app.AcquireTokenInteractive(acquireScopes).ExecuteAsync();
            }
            catch (Exception iex)
            {
                Log.Error("Interactive authentication failed: {Error}", iex.Message);
                return null;
            }
        }
        catch (Exception ex)
        {
            Log.Error("Authentication failed: {Error}", ex.Message);
            return null;
        }

        return result;
    }
}