using System.IO;
using System.Text.Json;
using Azure.Identity;
using Azure.Monitor.Ingestion;
using Core.azure;

namespace Core.Tests.azure;

[TestFixture]
public class AzureMonitorTests
{
    
    [Ignore("For exploratory testing only")]
    [Test]
    public void TestSendingLogs()
    {
        var configPath = Path.Combine(TestContext.CurrentContext.TestDirectory, "azure", "local.config.json");
        var json = File.ReadAllText(configPath);
        var config = JsonSerializer.Deserialize<AzureMonitorConfig>(json);
        Assert.That(config, Is.Not.Null);
        var endpoint = new Uri(config.DataCollectionEndpointUri);
        var credential = new DefaultAzureCredential();
        var client = new LogsIngestionClient(endpoint, credential);
        DateTimeOffset currentTime = DateTimeOffset.UtcNow;
    }
    
    [Ignore("For exploratory testing only")]
    [Test]
    public void TestSendingMetrics()
    {
        var configPath = Path.Combine(TestContext.CurrentContext.TestDirectory, "azure", "local.config.json");
        var json = File.ReadAllText(configPath);
        var config = JsonSerializer.Deserialize<AzureMonitorConfig>(json);
        Assert.That(config, Is.Not.Null);
        var monitor = new AzureMonitor();
        monitor.SendMetrics(new List<(string, string)>
        {
            ("AppMetric1", "15.3"),
            ("AppMetric2", "23.5")
        });
    }
    
    
}