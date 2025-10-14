using Azure.Identity;
using Azure.Monitor.Ingestion;

namespace Core.azure;

public record AzureMonitorConfig(
    string DataCollectionEndpointUri,
    string RuleId,
    string StreamName
);

public class AzureMonitor
{
    public void SendMetrics(List<(string, string)> metrics)
    {
        
        var endpoint = new Uri("<data_collection_endpoint_uri>");
        var credential = new DefaultAzureCredential();
        var client = new LogsIngestionClient(endpoint, credential);
        
        var currentTime = DateTime.UtcNow;
        BinaryData data = BinaryData.FromObjectAsJson(
            new[] {
                new
                {
                    Time = currentTime,
                    Computer = "Computer1",
                    AdditionalContext = new
                    {
                        InstanceName = "user1",
                        TimeZone = "Pacific Time",
                        Level = 4,
                        CounterName = "AppMetric1",
                        CounterValue = 15.3
                    }
                },
                new
                {
                    Time = currentTime,
                    Computer = "Computer2",
                    AdditionalContext = new
                    {
                        InstanceName = "user2",
                        TimeZone = "Central Time",
                        Level = 3,
                        CounterName = "AppMetric1",
                        CounterValue = 23.5
                    }
                },
            });
        
        // Response response = await client.UploadAsync(
        //     ruleId,
        //     streamName,
        //     RequestContent.Create(data)).ConfigureAwait(false);
        // Implementation to send metric to Azure Monitor
    }
}