using System.Windows;
using System.Windows.Controls;
using Serilog;

namespace dotnet_probe.sso;

/// <summary>
/// Interaction logic for SsoWebControl.xaml
/// </summary>
public partial class WebControl : UserControl
{
    private readonly WebServer _webServer;
    
    public WebControl(WebServerConfig config)
    {
        InitializeComponent();
        _webServer = new WebServer(config);
    }

    private async void StartServer(object sender, RoutedEventArgs e)
    {
        Log.Information("Starting web server...");
        await _webServer.Start();
    }

    private async void StopServer(object sender, RoutedEventArgs e)
    {
        Log.Information("Stopping web server...");
        await _webServer.Stop();
    }
}