using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Microsoft.Extensions.Configuration;
using Serilog;

namespace dotnet_probe;

/// <summary>
/// Interaction logic for MainWindow.xaml
/// </summary>
public partial class MainWindow : Window
{
    private readonly IConfiguration _config;

    public MainWindow()
    {
        Closed += MainWindow_Closed;
        InitializeComponent();
        // Redirect Console output to the UI textbox so Serilog Console sink appears in the TextBox
        Console.SetOut(new TextBoxWriter(LogTextBox));
        _config = new ConfigurationBuilder()
            .AddJsonFile("appsettings.json")
            .Build();

        Log.Information("Application started with configuration {@Config}", _config.AsEnumerable());
    }

    private void SsoWpfWamButton_Click(object sender, RoutedEventArgs e)
    {
        try
        {
            var entraIdClientConfig = _config.GetRequiredSection("sso:wam:entraId").Get<sso.EntraIdClientConfig>()!;
            var keycloakClientConfig = _config.GetRequiredSection("sso:wam:keycloak").Get<sso.KeycloakClientConfig>()!;
            DynamicContent.Content = new sso.WpfWamControl(entraIdClientConfig, keycloakClientConfig);
            SsoWpfWamToggle.IsChecked = true;
            SsoWebToggle.IsChecked = false;
            SsoWpfWebView2Toggle.IsChecked = false;
            AzureMonitorToggle.IsChecked = false;
        }
        catch (Exception ex)
        {
            Log.Error(ex, "Error loading WPF WAM control with configuration {@Config}", _config.AsEnumerable());
        }
    }

    private void SsoWebButton_Click(object sender, RoutedEventArgs e)
    {
        try
        {
            var config = _config.GetRequiredSection("sso:web").Get<sso.WebServerConfig>()!;
            DynamicContent.Content = new sso.WebControl(config);
            SsoWpfWamToggle.IsChecked = false;
            SsoWebToggle.IsChecked = true;
            SsoWpfWebView2Toggle.IsChecked = false;
            AzureMonitorToggle.IsChecked = false;
        }
        catch (Exception ex)
        {
            Log.Error(ex, "Error loading SSO Web control with configuration {@Config}", _config.AsEnumerable());
        }
    }

    private void AzureMonitorButton_Click(object sender, RoutedEventArgs e)
    {
        DynamicContent.Content = new azure.AzureMonitorControl();
        SsoWpfWamToggle.IsChecked = false;
        SsoWebToggle.IsChecked = false;
        SsoWpfWebView2Toggle.IsChecked = false;
        AzureMonitorToggle.IsChecked = true;
    }


    private void SsoWpfWebView2Button_Click(object sender, RoutedEventArgs e)
    {
        DynamicContent.Content = new sso.WpfWebView2Control();
        SsoWpfWamToggle.IsChecked = false;
        SsoWebToggle.IsChecked = false;
        SsoWpfWebView2Toggle.IsChecked = true;
        AzureMonitorToggle.IsChecked = false;
    }

    private static void MainWindow_Closed(object? sender, EventArgs e)
    {
        Log.Information("Application closing");
        Log.CloseAndFlush();
    }
}