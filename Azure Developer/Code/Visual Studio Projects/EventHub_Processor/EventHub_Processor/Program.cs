using Azure.Messaging.EventHubs;
using Azure.Messaging.EventHubs.Processor;
using Azure.Storage.Blobs;
using System.Text;

namespace EventHub_Processor;
class Program
{
    private static string listenEventConnectionString = "replace me";
    private static string consumerGroup = "$Default";
    private static string storageConnectionString = "replace me";
    private static string containerName = "eventhub";

    static async Task Main(string[] args)
    {
        // Client to interact with the Azure Storage Account
        BlobContainerClient blobClient = new BlobContainerClient(storageConnectionString, containerName);

        // Client to interact with Event Hubs
        EventProcessorClient eventClient = new EventProcessorClient(blobClient, consumerGroup, listenEventConnectionString);

        eventClient.ProcessErrorAsync += OurErrorHandler;
        eventClient.ProcessEventAsync += OurEventsHandler;

        await eventClient.StartProcessingAsync();

        await Task.Delay(TimeSpan.FromSeconds(30));

        Console.ReadKey();
    }

    static async Task OurEventsHandler(ProcessEventArgs eventArgs)
    {
        Console.WriteLine($"Sequence number {eventArgs.Data.SequenceNumber}");
        Console.WriteLine(Encoding.UTF8.GetString(eventArgs.Data.EventBody));
        await eventArgs.UpdateCheckpointAsync(eventArgs.CancellationToken);
    }

    static Task OurErrorHandler(ProcessErrorEventArgs eventArgs)
    {
        Console.WriteLine(eventArgs.Exception.Message);
        return Task.CompletedTask;
    }
}
