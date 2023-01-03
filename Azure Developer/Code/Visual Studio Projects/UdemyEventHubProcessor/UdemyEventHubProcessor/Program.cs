using Azure.Messaging.EventHubs;
using Azure.Messaging.EventHubs.Processor;
using Azure.Storage.Blobs;

string connectionString = "Connection string from SAS with listen permission";
string consumerGroup = "$Default";
string blobConnectionString = "Storage account connection string";
string containerName = "checkpoint";

BlobContainerClient blobContainerClient = new(blobConnectionString, containerName);
EventProcessorClient eventProcessorClient = new(blobContainerClient, consumerGroup, connectionString);

eventProcessorClient.ProcessEventAsync += ProcessEvents;
eventProcessorClient.ProcessErrorAsync += ErrorHandler;

await eventProcessorClient.StartProcessingAsync();
Console.ReadKey();
await eventProcessorClient.StopProcessingAsync();

async Task ProcessEvents(ProcessEventArgs processEvent)
{
    Console.WriteLine(processEvent.Data.EventBody.ToString());
}

static Task ErrorHandler(ProcessErrorEventArgs errorEvent)
{
    Console.WriteLine(errorEvent.Exception.Message);
    return Task.CompletedTask;
}