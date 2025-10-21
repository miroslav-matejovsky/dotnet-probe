using System.IO;
using System.Text.Json;
using Azure.Identity;
using Azure.Monitor.Ingestion;
using Azure.Monitor.OpenTelemetry.Exporter;
using Microsoft.Extensions.Logging;
using OpenTelemetry;
using OpenTelemetry.Metrics;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;

namespace Core.Tests.azure;

public record AzureMonitorConfig(
    string MetricsConnectionString,
    string DataCollectionEndpointUri,
    string RuleId,
    string StreamName
);

[TestFixture]
public class AzureMonitorTests
{
    private static readonly AzureMonitorConfig Config;

    static AzureMonitorTests()
    {
        var configPath = Path.Combine(TestContext.CurrentContext.TestDirectory, "azure", "local.config.json");
        var json = File.ReadAllText(configPath);
        var config = JsonSerializer.Deserialize<AzureMonitorConfig>(json);
        Assert.That(config, Is.Not.Null);
        Config = config!;
    }

    [Ignore("For exploratory testing only")]
    [Test]
    public async Task TestSendingMetrics()
    {
        using var metricsProvider = Sdk.CreateMeterProviderBuilder()
            .SetResourceBuilder(ResourceBuilder.CreateDefault().AddService(
                serviceName: "mirmat-local-probe",
                serviceVersion: "0.1.0"))
            // .AddConsoleExporter()
            .AddAzureMonitorMetricExporter(options =>
            {
                options.ConnectionString = Config.MetricsConnectionString;
                options.StorageDirectory = TestContext.CurrentContext.WorkDirectory + "\\AzureMonitorMetrics";
                options.SamplingRatio = 1.0f;
            })
            .AddAspNetCoreInstrumentation()
            .Build();
        
        await Task.Delay(10_000);
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
        var endpoint = new Uri(Config.DataCollectionEndpointUri);
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