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
    private readonly MonitoredApp _app;

    public AzureMonitorControl()
    {
        InitializeComponent();
        _app = new MonitoredApp();
    }

    private async void StartAppButton_Click(object sender, System.Windows.RoutedEventArgs e)
    {
        await _app.Start();
    }
    
    private async void StopAppButton_Click(object sender, System.Windows.RoutedEventArgs e)
    {
        await _app.Stop();
    }
}