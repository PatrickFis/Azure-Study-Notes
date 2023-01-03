using Azure.Messaging.EventHubs.Consumer;
using System.Text;

string connectionString = "Connection string from SAS with listen permission";
string consumerGroup = "$Default";

await GetPartitionIds();
await ReadEvents();
//await ReadEventsFromFirstPartition();

async Task GetPartitionIds()
{
    EventHubConsumerClient eventHubConsumerClient = new(consumerGroup, connectionString);

    string[] partitionIds = await eventHubConsumerClient.GetPartitionIdsAsync();

    foreach(string partitionId in partitionIds)
    {
        Console.WriteLine($"Partition ID {partitionId}");
    }
}

async Task ReadEvents()
{
    EventHubConsumerClient eventHubConsumerClient = new(consumerGroup, connectionString);
    var cancellationSource = new CancellationTokenSource();
    cancellationSource.CancelAfter(TimeSpan.FromSeconds(300));

    await foreach (PartitionEvent partitionEvent in eventHubConsumerClient.ReadEventsAsync(cancellationSource.Token))
    {
        Console.WriteLine($"Partition ID {partitionEvent.Partition.PartitionId}");
        Console.WriteLine($"Data Offset {partitionEvent.Data.Offset}");
        Console.WriteLine($"Sequence Number {partitionEvent.Data.SequenceNumber}");
        Console.WriteLine($"Partition Key {partitionEvent.Data.PartitionKey}");
        Console.WriteLine(Encoding.UTF8.GetString(partitionEvent.Data.EventBody));
    }
}

async Task ReadEventsFromFirstPartition()
{
    EventHubConsumerClient eventHubConsumerClient = new(consumerGroup, connectionString);
    string partitionId = (await eventHubConsumerClient.GetPartitionIdsAsync()).First();
    var cancellationSource = new CancellationTokenSource();
    cancellationSource.CancelAfter(TimeSpan.FromSeconds(300));

    await foreach(PartitionEvent partitionEvent in eventHubConsumerClient.ReadEventsFromPartitionAsync(partitionId, EventPosition.Latest, cancellationSource.Token))
    {
        Console.WriteLine($"Partition ID {partitionEvent.Partition.PartitionId}");
        Console.WriteLine($"Data Offset {partitionEvent.Data.Offset}");
        Console.WriteLine($"Sequence Number {partitionEvent.Data.SequenceNumber}");
        Console.WriteLine($"Partition Key {partitionEvent.Data.PartitionKey}");
        Console.WriteLine(Encoding.UTF8.GetString(partitionEvent.Data.EventBody));
    }
}