using System.Windows;
using System.Windows.Controls;
using Serilog;

namespace dotnet_probe.sso;

/// <summary>
/// Interaction logic for SsoWpfWamControl.xaml
/// </summary>
public partial class WpfControl : UserControl
{
    private readonly EntraIdClientConfig _entraIdClientConfig;
    private readonly KeycloakClientConfig _keycloakClientConfig;
    private readonly Keycloak _keycloak;
    
    public WpfControl(EntraIdClientConfig entraIdClientConfig, KeycloakClientConfig keycloakClientConfig)
    {
        _entraIdClientConfig = entraIdClientConfig;
        _keycloakClientConfig = keycloakClientConfig;
        InitializeComponent();
        _keycloak = new Keycloak(_keycloakClientConfig);
        Log.Information("WPF WAM Control initialized with EntraIdClientConfig: {@EntraIdClientConfig} and KeycloakClientConfig: {@KeycloakClientConfig}", entraIdClientConfig, keycloakClientConfig);
    }

    private async void LoginWamButton_Click(object sender, RoutedEventArgs e)
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
        var kcToken = await _keycloak.TokenExchange(result);
        if (kcToken == null)
        {
            Log.Error("Token exchange failed");
            return;
        }
        Log.Information("Token exchange successful. New token for {AccountUsername}", result.Account.Username);
    }

    private async void LoginWithCredentialsButton_Click(object sender, RoutedEventArgs e)
    {
        var username = UsernameTextBox.Text;
        var password = PasswordBox.Password;
        Log.Information("Login with credentials attempted for username: {Username}", username);
        var result = await _keycloak.LoginUser(username, password);
        Log.Information("Login with credentials process completed: {result}", result);
    }
}