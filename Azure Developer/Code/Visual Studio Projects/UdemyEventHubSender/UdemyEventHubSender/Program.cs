using Azure.Messaging.EventHubs;
using Azure.Messaging.EventHubs.Producer;
using Newtonsoft.Json;
using System.Text;
using UdemyEventHubSender;

string connectionString = "Connection string from SAS with send permission";
string eventHubName = "apphub";

List <Device> deviceList = new()
{
    new Device() { deviceId = "D1", temperature = 40.0f },
    new Device() { deviceId = "D1", temperature = 39.9f },
    new Device() { deviceId = "D2", temperature = 36.4f },
    new Device() { deviceId = "D2", temperature = 37.4f },
    new Device() { deviceId = "D3", temperature = 38.9f },
    new Device() { deviceId = "D4", temperature = 35.4f },
};

List<Device> partitionKeyDeviceList = new()
{
    new Device() { deviceId = "D1", temperature = 40.0f },
    new Device() { deviceId = "D1", temperature = 39.9f }
};

//await SendData();
await SendDataWithSpecificPartitionKey();

async Task SendData()
{
    EventHubProducerClient eventHubProducerClient = new(connectionString, eventHubName);
    EventDataBatch eventDataBatch = await eventHubProducerClient.CreateBatchAsync();

    foreach(Device device in deviceList)
    {
        EventData eventData = new(Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(device)));
        if(!eventDataBatch.TryAdd(eventData))
        {
            Console.WriteLine("Error occurred while adding data to the batch");
        }
    }

    await eventHubProducerClient.SendAsync(eventDataBatch);
    Console.WriteLine("Events are sent");
    await eventHubProducerClient.DisposeAsync();
}

async Task SendDataWithSpecificPartitionKey()
{
    EventHubProducerClient eventHubProducerClient = new(connectionString, eventHubName);

    List<EventData> data = new();

    foreach (Device device in partitionKeyDeviceList)
    {
        EventData eventData = new(Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(device)));
        data.Add(eventData);
    }

    await eventHubProducerClient.SendAsync(data, new SendEventOptions() { PartitionKey = "D1" });
    Console.WriteLine("Events are sent");
    await eventHubProducerClient.DisposeAsync();
}