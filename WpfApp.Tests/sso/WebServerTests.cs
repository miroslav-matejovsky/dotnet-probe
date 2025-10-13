using dotnet_probe.sso;
using Serilog;

namespace dotnet_probe.tests.sso;

[TestFixture]
public class WebServerTests
{
    
    
    [OneTimeSetUp]
    public void OneTimeSetUp()
    {
        // Configure and create the logger here
        Log.Logger = new LoggerConfiguration()
            .WriteTo.Console()
            .CreateLogger();
    }
    
    [Ignore("just exploratory")]
    [Test]
    public async Task TestWebServerStartStop()
    {
        const string urls = "http://localhost:5000";
        var keycloakConfig = new WebServerKeycloakConfig(
            Url: "not-important-here-url",
            Realm: "not-important-here-realm",
            ClientId: "not-important-here-client-id"
        );
        var config = new WebServerConfig(urls, keycloakConfig);
        var webServer = new WebServer(config);
        var task = webServer.Start();
        // This is a placeholder test. Implement actual web server start/stop logic here.
        // Assert.Pass("Web server start/stop test passed.");

        await Task.Delay(2000);
        // Perform an HTTP GET request to verify the server is running
        using var client = new HttpClient();
        var response = await client.GetAsync(urls);
        Assert.That(response.IsSuccessStatusCode, Is.True, "Server should respond successfully.");
        
        await webServer.Stop();
        await task;
    }
}