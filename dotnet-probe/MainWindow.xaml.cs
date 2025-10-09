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
        InitializeComponent();
        // Redirect Console output to the UI textbox so Serilog Console sink appears in the TextBox
        Console.SetOut(new TextBoxWriter(LogTextBox));
    }

    private void AzureMonitorButton_Click(object sender, RoutedEventArgs e)
    {
        Log.Information("Azure Monitor button clicked");
    }
    
    private void SsoWebButton_Click(object sender, RoutedEventArgs e)
    {
        Log.Information("SSO Web button clicked");
        // var poc1Window = new POC1Window();
        // poc1Window.Show();
    }

    private void SsoWpfWamButton_Click(object sender, RoutedEventArgs e)
    {
        Log.Information("SSO WPF WAM button clicked");
        // var poc1Window = new POC1Window();
        // poc1Window.Show();
    }

    private void SsoWpfWebView2Button_Click(object sender, RoutedEventArgs e)
    {
        Log.Information("SSO WPF WebView2 button clicked");
        // var poc2Window = new POC2Window();
        // poc2Window.Show();
    }
}