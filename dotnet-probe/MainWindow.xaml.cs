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
using dotnet_probe.sso;
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
        Log.Information("SSO WPF WAM button clicked");
        var clientConfig = _config.GetRequiredSection("sso:wam").Get<ClientConfig>()!;
        DynamicContent.Content = new sso.WpfWamControl(clientConfig);
    }


    private void AzureMonitorButton_Click(object sender, RoutedEventArgs e)
    {
        Log.Information("Azure Monitor button clicked");
        DynamicContent.Content = new azure.AzureMonitorControl();
    }
    
    private void SsoWebButton_Click(object sender, RoutedEventArgs e)
    {
        Log.Information("SSO Web button clicked");
        DynamicContent.Content = new sso.WebControl();
    }
    private void SsoWpfWebView2Button_Click(object sender, RoutedEventArgs e)
    {
        Log.Information("SSO WPF WebView2 button clicked");
        DynamicContent.Content = new sso.WpfWebView2Control();
    }
    
    private static void MainWindow_Closed(object? sender, EventArgs e)
    {
        Log.Information("Application closing");
        Log.CloseAndFlush();
    }
}