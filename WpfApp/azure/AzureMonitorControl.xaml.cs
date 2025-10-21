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
    private readonly AzureMonitorConfig _config;
    private readonly List<MonitoredService> _services = [];

    public AzureMonitorControl(AzureMonitorConfig config)
    {
        _config = config;
        InitializeComponent();
    }

    private async void StartServicesButton_Click(object sender, System.Windows.RoutedEventArgs e)
    {
        var numberOfServices = int.Parse(ServiceCountTextBox.Text);
        Log.Information("Starting {NumberOfServices} monitored services", numberOfServices);
        // Create monitored services
        for (var i = 0; i < numberOfServices; i++)
        {
            var app = new MonitoredService(i, _config);
            _services.Add(app);
        }
        // Start all services asynchronously
        var startTasks = _services.Select(app => app.Start()).ToList();
        Log.Information("Starting all monitored services...");
        try
        {
            await Task.WhenAll(startTasks);
        }
        catch (Exception ex)
        {
            Log.Error(ex, "Error starting monitored services");
        }
        Log.Information("All monitored services started");
    }

    private async void StopServicesButton_Click(object sender, System.Windows.RoutedEventArgs e)
    {
        var stopTasks = _services.Select(app => app.Stop()).ToList();
        await Task.WhenAll(stopTasks);
    }
}