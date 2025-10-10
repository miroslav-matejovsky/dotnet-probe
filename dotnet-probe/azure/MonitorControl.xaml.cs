using System.Windows.Controls;
using Serilog;

namespace dotnet_probe.azure;

/// <summary>
/// Interaction logic for SsoWebControl.xaml
/// </summary>
public partial class MonitorControl : UserControl
{
    public MonitorControl()
    {
        InitializeComponent();
    }

    private void SendMetricsButton_Click(object sender, System.Windows.RoutedEventArgs e)
    {
        // TODO: Implement sending metrics to Azure Monitor
        string key = KeyTextBox.Text;
        string value = ValueTextBox.Text;
        // For now, just log
        Serilog.Log.Information("Sending metric: {Key} = {Value}", key, value);
    }
}