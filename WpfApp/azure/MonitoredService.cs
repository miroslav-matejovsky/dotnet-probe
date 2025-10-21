using System.Net.Http;
using Azure.Monitor.OpenTelemetry.AspNetCore;
using Azure.Monitor.OpenTelemetry.Exporter;
using OpenTelemetry.Metrics;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;
using Serilog;

namespace dotnet_probe.azure;

public record AzureMonitorConfig(string ConnectionString);

public class MonitoredService(int id, AzureMonitorConfig config) : IAsyncDisposable
{
    private WebApplication? _app;

    public async Task Start()
    {
     
        var serviceName = $"mirmat-probe-{id}";
        var port = 5000 + id;
        Log.Information("Preparing monitored service with name {ServiceName} on port {Port}", serviceName, port);

        var args = new[] { "--urls", $"http://localhost:{port}" };
        var options = new WebApplicationOptions { Args = args };

        var builder = WebApplication.CreateBuilder(options);
        builder.Services.AddSerilog(dispose: true);
        builder.Services.AddHttpClient();
        builder.Services.AddHostedService<PeriodicHttpClientService>();
        builder.Services.AddOpenTelemetry()
            // Signal-specific AddAzureMonitorExporter / UseAzureMonitor methods is not allowed
            // .UseAzureMonitor(azureOptions =>
            //     azureOptions.ConnectionString = config.ConnectionString
            // )
            .ConfigureResource(resource => resource.AddService(
                serviceName: serviceName,
                serviceVersion: "0.1.0",
                serviceNamespace: "mirmat-local"
                // serviceInstanceId: $"{id}"
            ))
            .WithMetrics(metrics => metrics
                // .AddMeter(ServiceName)
                // .AddConsoleExporter()
                .AddPrometheusExporter()
                .AddAspNetCoreInstrumentation()
                .AddHttpClientInstrumentation()
                .AddAzureMonitorMetricExporter(azureOptions =>
                    azureOptions.ConnectionString = config.ConnectionString
                ))
            // Tracing is required for Application Map in Azure Monitor
            .WithTracing(tracing => tracing
                .AddSource(serviceName)
                .AddAspNetCoreInstrumentation()
                .AddHttpClientInstrumentation()
                // .AddConsoleExporter()
                .AddAzureMonitorTraceExporter(azureOptions =>
                    azureOptions.ConnectionString = config.ConnectionString
                ));
        
        builder.Services.AddApplicationInsightsTelemetry(insightOptions =>
        {
            insightOptions.ConnectionString = config.ConnectionString;
        });

        _app = builder.Build();
        _app.UseOpenTelemetryPrometheusScrapingEndpoint();
        _app.UseRouting();
        _app.Map("/health", () => "OK");
        _app.Map("/", () => $"Hello from monitored service {serviceName} with id {id}!");

        Log.Information("Starting service {ServiceName}  {Id} ...", serviceName, id);
        await _app.StartAsync();
        Log.Information("Monitored service {ServiceName} started on port {Port}", serviceName, port);
    }

    public async Task Stop()
    {
        Log.Information("Stopping monitored app...");
        if (_app != null)
        {
            var token = new CancellationTokenSource(TimeSpan.FromSeconds(5)).Token;
            await _app.StopAsync(token);
            await _app.DisposeAsync();
        }
    }

    public async ValueTask DisposeAsync()
    {
        GC.SuppressFinalize(this);
        await Stop();
    }
}

public class PeriodicHttpClientService(IHttpClientFactory httpClientFactory) : BackgroundService
{
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        var client = httpClientFactory.CreateClient();
        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                await client.GetAsync("https://api.github.com", stoppingToken);
                // Log.Debug("Made HTTP request to github");
            }
            catch (Exception e)
            {
                Log.Error(e, "Error making HTTP request to github");
            }

            await Task.Delay(TimeSpan.FromSeconds(2), stoppingToken);
        }
    }
}