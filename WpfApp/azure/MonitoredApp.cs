using Azure.Monitor.OpenTelemetry.Exporter;
using OpenTelemetry.Metrics;
using OpenTelemetry.Resources;
using Serilog;

namespace dotnet_probe.azure;

public record AzureMonitorConfig(string ConnectionString);

public class MonitoredApp(string serviceName, AzureMonitorConfig config) : IAsyncDisposable
{
    private WebApplication? _app;
    
    public async Task Start()
    {
        Log.Information("Preparing monitored app...");
        var builder = WebApplication.CreateBuilder();
        builder.Services.AddSerilog(dispose: true);
        builder.Services.AddOpenTelemetry()
            .ConfigureResource(resource => resource.AddService(
                serviceName: serviceName,
                serviceVersion: "0.1.0",
                serviceNamespace: "mirmat-local-probe"
            ))
            .WithMetrics(metrics => metrics
                // .AddMeter(ServiceName)
                .AddConsoleExporter()
                .AddPrometheusExporter()
                .AddAspNetCoreInstrumentation()
                .AddHttpClientInstrumentation()
                .AddAzureMonitorMetricExporter(options =>
                    options.ConnectionString = config.ConnectionString
                ));
            // .WithTracing(tracing => tracing
            //     // .AddSource(ServiceName)
            //     .AddAspNetCoreInstrumentation()
            //     .AddHttpClientInstrumentation()
            //     .AddConsoleExporter()
            //     .AddAzureMonitorTraceExporter(options =>
            //         options.ConnectionString = config.ConnectionString
            //     ));

        _app = builder.Build();
        _app.UseOpenTelemetryPrometheusScrapingEndpoint();
        _app.UseRouting();
        _app.Map("/health", () => "OK");
        _app.Map("/", () => "Hello from monitored app!");

        Log.Information("Starting monitored app...");
        await _app.StartAsync();
        Log.Information("Monitored app started.");
    }

    public async Task Stop()
    {
        Log.Information("Stopping monitored app...");
        if (_app != null)
        {
            var token = new CancellationTokenSource(TimeSpan.FromSeconds(5)).Token;
            await _app.StopAsync(token);
        }
    }

    public async ValueTask DisposeAsync()
    {
        GC.SuppressFinalize(this);
        await Stop();
    }
}