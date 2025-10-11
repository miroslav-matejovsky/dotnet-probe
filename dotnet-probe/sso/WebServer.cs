using Microsoft.Extensions.Options;
using Serilog;

namespace dotnet_probe.sso;

public record WebServerConfig(string Url);

public class WebServer(WebServerConfig config) : IAsyncDisposable
{
    private WebApplication? _app;
    
    public async Task Start()
    {
        var args = new[] { "--urls", config.Url };
        var options = new WebApplicationOptions
        {
            ContentRootPath = AppContext.BaseDirectory,
            WebRootPath = "wwwroot",
            Args = args
        };
        var builder = WebApplication.CreateBuilder(options);
        // builder.Services.Configure<KeycloakOptions>(builder.Configuration.GetSection("Keycloak"));

        builder.Services.AddSerilog(dispose: true);
        _app = builder.Build();
        _app.UseDefaultFiles();
        _app.UseStaticFiles();
        _app.UseRouting();
        _app.MapGet("/config", (IOptions<KeycloakOptions> opts) => Results.Json(new
        {
            url = opts.Value.Url,
            realm = opts.Value.Realm,
            clientId = opts.Value.ClientId
        }));


        await _app.StartAsync(); 
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