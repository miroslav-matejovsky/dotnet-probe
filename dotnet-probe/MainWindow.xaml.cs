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
using Serilog;

namespace dotnet_probe;

/// <summary>
/// Interaction logic for MainWindow.xaml
/// </summary>
public partial class MainWindow : Window
{
    public MainWindow()
    {
        Closed += MainWindow_Closed;
        InitializeComponent();
        // Redirect Console output to the UI textbox so Serilog Console sink appears in the TextBox
        Console.SetOut(new TextBoxWriter(LogTextBox));
    }

    private void AzureMonitorButton_Click(object sender, RoutedEventArgs e)
    {
        Log.Information("Azure Monitor button clicked");
        DynamicContent.Content = new azure.MonitorControl();
    }
    
    private void SsoWebButton_Click(object sender, RoutedEventArgs e)
    {
        Log.Information("SSO Web button clicked");
        DynamicContent.Content = new sso.WebControl();
    }

    private void SsoWpfWamButton_Click(object sender, RoutedEventArgs e)
    {
        Log.Information("SSO WPF WAM button clicked");
        DynamicContent.Content = new sso.WpfWamControl();
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