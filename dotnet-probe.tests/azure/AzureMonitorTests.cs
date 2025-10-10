using Azure.Identity;
using Azure.Monitor.Ingestion;

namespace dotnet_probe.tests.azure;

[TestFixture]
public class AzureMonitorTests
{
    
    [Ignore("For exploratory testing only")]
    [Test]
    public void TestSendingData()
    {
        var endpoint = new Uri("<data_collection_endpoint_uri>");
        var credential = new DefaultAzureCredential();
        var client = new LogsIngestionClient(endpoint, credential);
        DateTimeOffset currentTime = DateTimeOffset.UtcNow;
    }
    
}