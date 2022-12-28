using Azure.Messaging.ServiceBus;
using Newtonsoft.Json;
using UdemyServiceBus;

string connectionString = "Connection string here";
string queueName = "appqueue";

ServiceBusClient serviceBusClient = new(connectionString);
ServiceBusProcessor serviceBusProcessor = serviceBusClient.CreateProcessor(queueName, new ServiceBusProcessorOptions());

serviceBusProcessor.ProcessMessageAsync += ProcessMessage;
serviceBusProcessor.ProcessErrorAsync += ErrorHandler;

await serviceBusProcessor.StartProcessingAsync();
Console.WriteLine("Waiting for messages.");
Console.ReadKey();

await serviceBusProcessor.StopProcessingAsync();
await serviceBusProcessor.DisposeAsync();
await serviceBusClient.DisposeAsync();

static async Task ProcessMessage(ProcessMessageEventArgs messageEvents)
{
    Order order = JsonConvert.DeserializeObject<Order>(messageEvents.Message.Body.ToString());
    Console.WriteLine($"Order ID: {order.OrderID}. Quantity: {order.Quantity}. Unit price: {order.UnitPrice}.");
}

static Task ErrorHandler(ProcessErrorEventArgs args)
{
    Console.WriteLine(args.Exception.ToString());
    return Task.CompletedTask;
}