using Microsoft.Extensions.Options;
using Serilog;

namespace dotnet_probe.sso;

public record WebServerConfig(string ServerUrl, WebServerKeycloakConfig Keycloak);

public record WebServerKeycloakConfig(string Url, string Realm, string ClientId);

public class WebServer(WebServerConfig config) : IAsyncDisposable
{
    private WebApplication? _app;

    public async Task Start()
    {
        Log.Information("Preparing web server at {Url}", config.ServerUrl);
        Log.Information("Keycloak config: {@Keycloak}", config.Keycloak);

        var args = new[] { "--urls", config.ServerUrl };
        var options = new WebApplicationOptions
        {
            ContentRootPath = AppContext.BaseDirectory,
            WebRootPath = "wwwroot",
            Args = args
        };
        var builder = WebApplication.CreateBuilder(options);
        builder.Services.AddSerilog(dispose: true);
        _app = builder.Build();
        _app.UseDefaultFiles();
        _app.UseStaticFiles();
        _app.UseRouting();
        _app.MapGet("/config", async ctx => await ctx.Response.WriteAsJsonAsync(config.Keycloak));
        await _app.StartAsync();
        Log.Information("Web server started at {Url}", config.ServerUrl);
    }

    public async Task Stop()
    {
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