using System.Windows.Controls;
using Azure.Identity;
using Azure.Monitor.Ingestion;
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
        var metrics = new List<(string Key, string Value)>
        {
            (Key1TextBox.Text, Value1TextBox.Text),
            (Key2TextBox.Text, Value2TextBox.Text),
            (Key3TextBox.Text, Value3TextBox.Text),
            (Key4TextBox.Text, Value4TextBox.Text)
        };

        var endpoint = new Uri("<data_collection_endpoint_uri>");
        var credential = new DefaultAzureCredential();
        var client = new LogsIngestionClient(endpoint, credential);
        
        foreach (var (key, value) in metrics)
        {
            if (!string.IsNullOrWhiteSpace(key) && !string.IsNullOrWhiteSpace(value))
            {
                Log.Information("Sending metric: {Key} = {Value}", key, value);
            }
        }
    }
}