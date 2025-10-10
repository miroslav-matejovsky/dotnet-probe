using System.Windows;
using System.Windows.Controls;
using Serilog;

namespace dotnet_probe.sso;

/// <summary>
/// Interaction logic for SsoWpfWamControl.xaml
/// </summary>
public partial class WpfWamControl : UserControl
{
    public WpfWamControl()
    {
        InitializeComponent();
    }

    private async void LoginButton_Click(object sender, RoutedEventArgs e)
    {
        Log.Information("Login button clicked in WpfWamControl");
        var parentWindow = Window.GetWindow(this);
        if (parentWindow == null)
        {
            Log.Error("Parent window is null");
            return;
        }

        var result = await EntraId.AuthenticateUserViaMaw(parentWindow, "a", "a");
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