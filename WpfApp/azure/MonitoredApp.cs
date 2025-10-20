using Serilog;

namespace dotnet_probe.azure;

public class MonitoredApp
{
    private WebApplication? _app;

    public async Task Start()
    {
        Log.Information("Preparing monitored app...");
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