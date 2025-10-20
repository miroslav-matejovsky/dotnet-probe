using Azure.Monitor.OpenTelemetry.Exporter;
using OpenTelemetry.Metrics;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;
using Serilog;

namespace dotnet_probe.azure;

public record AzureMonitorConfig(string ConnectionString);

public class MonitoredApp(AzureMonitorConfig config) : IAsyncDisposable
{
    private WebApplication? _app;
    private const string ServiceName = "mirmat-probe-monitored-app";

    public async Task Start()
    {
        Log.Information("Preparing monitored app...");
        var builder = WebApplication.CreateBuilder();
        builder.Services.AddSerilog(dispose: true);
        builder.Services.AddOpenTelemetry()
            .ConfigureResource(resource => resource.AddService(
                serviceName: ServiceName,
                serviceVersion: "0.1.0"))
            .WithTracing(tracing => tracing
                .AddSource(ServiceName)
                .AddAspNetCoreInstrumentation()
                .AddConsoleExporter()
                .AddAzureMonitorTraceExporter(options =>
                    options.ConnectionString = config.ConnectionString
                ))
            .WithMetrics(metrics => metrics
                .AddMeter(ServiceName)
                .AddConsoleExporter()
                .AddAzureMonitorMetricExporter(options =>
                    options.ConnectionString = config.ConnectionString
                ));

        _app = builder.Build();
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