using System.Collections.Generic;
using System.Windows.Controls;
using Azure.Identity;
using Azure.Monitor.Ingestion;
using Serilog;

namespace dotnet_probe.azure;

/// <summary>
/// Interaction logic for AzureMonitorControl.xaml
/// </summary>
public partial class AzureMonitorControl : UserControl
{
    public AzureMonitorControl()
    {
        InitializeComponent();
    }

    private void SendMetricsButton_Click(object sender, System.Windows.RoutedEventArgs e)
    {
        // TODO: Implement sending metrics to Azure Monitor
        var metrics = new List<(string Key, string Value)>
        {
            (KeyTextBox.Text, ValueTextBox.Text)
        };
        
        foreach (var (key, value) in metrics)
        {
            if (!string.IsNullOrWhiteSpace(key) && !string.IsNullOrWhiteSpace(value))
            {
                Log.Information("Sending metric: {Key} = {Value}", key, value);
            }
        }
    }
}