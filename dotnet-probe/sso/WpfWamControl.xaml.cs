using System.Windows;
using System.Windows.Controls;
using Serilog;

namespace dotnet_probe.sso;

/// <summary>
/// Interaction logic for SsoWpfWamControl.xaml
/// </summary>
public partial class WpfWamControl : UserControl
{
    private readonly EntraIdClientConfig _entraIdClientConfig;
    private readonly KeycloakClientConfig _keycloakClientConfig;
    
    public WpfWamControl(EntraIdClientConfig entraIdClientConfig, KeycloakClientConfig keycloakClientConfig)
    {
        _entraIdClientConfig = entraIdClientConfig;
        _keycloakClientConfig = keycloakClientConfig;
        InitializeComponent();
        Log.Information("WPF WAM Control initialized with EntraIdClientConfig: {@EntraIdClientConfig} and KeycloakClientConfig: {@KeycloakClientConfig}", entraIdClientConfig, keycloakClientConfig);
    }

    private async void LoginButton_Click(object sender, RoutedEventArgs e)
    {
        Log.Information("Using WAM to authenticate user for TenantId: {TenantId}, ClientId: {ClientId}", _entraIdClientConfig.TenantId, _entraIdClientConfig.ClientId);
        var parentWindow = Window.GetWindow(this);
        if (parentWindow == null)
        {
            Log.Error("Parent window is null");
            return;
        }

        var result = await EntraId.AuthenticateUserViaMaw(parentWindow, _entraIdClientConfig.TenantId, _entraIdClientConfig.ClientId);
        if (result == null)
        {
            Log.Warning("Authentication failed or was cancelled");
            return;

        }
        Log.Information("Authentication successful for {AccountUsername}", result.Account.Username);
        var keycloak = new Keycloak(_keycloakClientConfig);
        Log.Information("Exchanging token with Keycloak at {Url} for client {ClientId}", _keycloakClientConfig.Url, _keycloakClientConfig.ClientId);
        result = await keycloak.TokenExchange(result);
        if (result == null)
        {
            Log.Warning("Token exchange failed");
            return;
        }
        Log.Information("Token exchange successful. New token for {AccountUsername}", result.Account.Username);
    }

    private void LoginWithCredentialsButton_Click(object sender, RoutedEventArgs e)
    {
        var username = UsernameTextBox.Text;
        var password = PasswordBox.Password;
        Log.Information("Login with credentials attempted for username: {Username}", username);
        // TODO: Implement authentication logic
    }
}