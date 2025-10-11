using System.Windows;
using System.Windows.Controls;
using Serilog;

namespace dotnet_probe.sso;

/// <summary>
/// Interaction logic for SsoWebControl.xaml
/// </summary>
public partial class WebControl : UserControl
{
    public WebControl()
    {
        InitializeComponent();
    }

    private void StartServer(object sender, RoutedEventArgs e)
    {
        Log.Information("Starting web server...");
    }

    private void StopServer(object sender, RoutedEventArgs e)
    {
        Log.Information("Stopping web server...");
    }
}