using System.IO;
using System.Text.Json;
using Azure.Identity;
using Azure.Monitor.Ingestion;
using Azure.Monitor.OpenTelemetry.Exporter;
using Microsoft.Extensions.Logging;
using OpenTelemetry;
using OpenTelemetry.Metrics;
using OpenTelemetry.Trace;

namespace Core.Tests.azure;

public record AzureMonitorConfig(
    string DataCollectionEndpointUri,
    string RuleId,
    string StreamName
);

[TestFixture]
public class AzureMonitorTests
{
    private static readonly AzureMonitorConfig _config;

    static AzureMonitorTests()
    {
        var configPath = Path.Combine(TestContext.CurrentContext.TestDirectory, "azure", "local.config.json");
        var json = File.ReadAllText(configPath);
        var config = JsonSerializer.Deserialize<AzureMonitorConfig>(json);
        Assert.That(config, Is.Not.Null);
        _config = config!;
    }

    [Ignore("For exploratory testing only")]
    [Test]
    public void TestSendingMetrics()
    {
        var metricsProvider = Sdk.CreateMeterProviderBuilder()
            .AddAzureMonitorMetricExporter(options =>
            {
                options.ConnectionString = "<Your Connection String>";
                options.StorageDirectory = "C:\\SomeDirectory";
            })
            .Build();
        // var monitor = new AzureMonitor();
        // monitor.SendMetrics(new List<(string, string)>
        // {
        // ("AppMetric1", "15.3"),
        // ("AppMetric2", "23.5")
        // });
    }


    [Ignore("For exploratory testing only")]
    [Test]
    public void TestSendingLogs()
    {
        var loggerFactory = LoggerFactory.Create(builder =>
        {
            builder.AddOpenTelemetry(logging =>
            {
                logging.AddAzureMonitorLogExporter(options =>
                {
                    options.ConnectionString = "<Your Connection String>";
                    options.StorageDirectory = "C:\\SomeDirectory";
                });
            });
        });
        var credential = new DefaultAzureCredential();
        var endpoint = new Uri(_config.DataCollectionEndpointUri);
        var client = new LogsIngestionClient(endpoint, credential);
        DateTimeOffset currentTime = DateTimeOffset.UtcNow;
    }

    [Ignore("For exploratory testing only")]
    [Test]
    public void TestSendingTraces()
    {
        var tracerProvider = Sdk.CreateTracerProviderBuilder()
            .AddAzureMonitorTraceExporter(options =>
            {
                options.ConnectionString = "<Your Connection String>";
                options.StorageDirectory = "C:\\SomeDirectory";
            })
            .Build();
    }
}