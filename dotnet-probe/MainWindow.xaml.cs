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

namespace dotnet_probe;

/// <summary>
/// Interaction logic for MainWindow.xaml
/// </summary>
public partial class MainWindow : Window
{
    public MainWindow()
    {
        InitializeComponent();
    }

    private void AzureMonitorButton_Click(object sender, RoutedEventArgs e)
    {
        
    }
    
    private void SsoWebButton_Click(object sender, RoutedEventArgs e)
    {
        // var poc1Window = new POC1Window();
        // poc1Window.Show();
    }

    private void SsoWpfWamButton_Click(object sender, RoutedEventArgs e)
    {
        // var poc1Window = new POC1Window();
        // poc1Window.Show();
    }

    private void SsoWpfWebView2Button_Click(object sender, RoutedEventArgs e)
    {
        // var poc2Window = new POC2Window();
        // poc2Window.Show();
    }
}