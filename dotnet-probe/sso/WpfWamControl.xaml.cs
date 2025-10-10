using System.Windows;
using System.Windows.Controls;
using Serilog;

namespace dotnet_probe.sso;

/// <summary>
/// Interaction logic for SsoWpfWamControl.xaml
/// </summary>
public partial class WpfWamControl : UserControl
{
    private readonly ClientConfig _config;
    
    public WpfWamControl(ClientConfig config)
    {
        _config = config;
        InitializeComponent();
    }

    private async void LoginButton_Click(object sender, RoutedEventArgs e)
    {
        Log.Information("Using WAM to authenticate user for TenantId: {TenantId}, ClientId: {ClientId}", _config.TenantId, _config.ClientId);
        var parentWindow = Window.GetWindow(this);
        if (parentWindow == null)
        {
            Log.Error("Parent window is null");
            return;
        }

        var result = await EntraId.AuthenticateUserViaMaw(parentWindow, _config.TenantId, _config.ClientId);
        if (result != null)
        {
            Log.Information("Authentication successful for {AccountUsername}", result.Account.Username);
        }
        else
        {
            Log.Warning("Authentication failed or was cancelled");
        }
    }
}