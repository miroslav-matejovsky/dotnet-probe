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
    private const string ServiceName = "mirmat-probe-monitored-app";

    private readonly AzureMonitorConfig _config;
    private MonitoredApp? _app;

    public AzureMonitorControl(AzureMonitorConfig config)
    {
        _config = config;
        InitializeComponent();
    }

    private async void StartAppButton_Click(object sender, System.Windows.RoutedEventArgs e)
    {
        _app = new MonitoredApp(ServiceName, 5000, _config);
        await _app.Start();
    }
    
    private async void StopAppButton_Click(object sender, System.Windows.RoutedEventArgs e)
    {
        if (_app != null)
        {
            await _app.Stop();
        }
    }
}