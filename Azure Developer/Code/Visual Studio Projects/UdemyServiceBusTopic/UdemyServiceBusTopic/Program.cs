using Azure.Messaging.ServiceBus;
using Newtonsoft.Json;
using UdemyServiceBusTopic;

string sendMessageConnectionString = "Connection string here";
string listenMessageConnectionString = "Connection string here";
string topicName = "apptopic";
string subscriptionName = "SubscriptionA";

string[] Importance = new string[] { "High", "Medium", "Low" };

var orders = new List<Order>()
{
    new Order() { OrderID = "01", Quantity = 100, UnitPrice = 9.99F },
    new Order() { OrderID = "02", Quantity = 200, UnitPrice = 10.99F },
    new Order() { OrderID = "03", Quantity = 300, UnitPrice = 8.99F }
};

await SendMessage(orders);
//await ReceiveMessages(listenMessageConnectionString, topicName);

// Send messages to the topic and mark them as JSON. Add a custom Importance property to them. An optional time to live (TTL) is set on the messages to override the default 14 day setting.
async Task SendMessage(List<Order> orders, int timeToLive = default)
{
    ServiceBusClient serviceBusClient = new(sendMessageConnectionString);
    ServiceBusSender serviceBusSender = serviceBusClient.CreateSender(topicName);

    ServiceBusMessageBatch serviceBusMessageBatch = await serviceBusSender.CreateMessageBatchAsync();
    int i = 0;
    int messageId = 0;
    foreach (Order order in orders)
    {
        ServiceBusMessage serviceBusMessage = new ServiceBusMessage(JsonConvert.SerializeObject(order));
        serviceBusMessage.ContentType = "application/json";
        serviceBusMessage.ApplicationProperties.Add("Importance", Importance[i++ % 3]);
        if (timeToLive > 0)
        {
            serviceBusMessage.TimeToLive = TimeSpan.FromSeconds(timeToLive);
        }

        serviceBusMessage.MessageId = messageId.ToString();
        messageId++;

        if (!serviceBusMessageBatch.TryAddMessage(serviceBusMessage))
        {
            throw new Exception("Error occurred while adding message to batch");
        }
    }

    Console.WriteLine("Sending messages");
    await serviceBusSender.SendMessagesAsync(serviceBusMessageBatch);

    await serviceBusSender.DisposeAsync();
    await serviceBusClient.DisposeAsync();
}

// Receive and delete messages in the topic
async Task ReceiveMessages(string connectionString, string topicName)
{
    ServiceBusClient serviceBusClient = new(connectionString);
    ServiceBusReceiver serviceBusReceiver = serviceBusClient.CreateReceiver(topicName, subscriptionName, new ServiceBusReceiverOptions() { ReceiveMode = ServiceBusReceiveMode.ReceiveAndDelete });

    IAsyncEnumerable<ServiceBusReceivedMessage> messages = serviceBusReceiver.ReceiveMessagesAsync();

    await foreach (ServiceBusReceivedMessage message in messages)
    {
        Order order = JsonConvert.DeserializeObject<Order>(message.Body.ToString());
        Console.WriteLine($"Order ID: {order.OrderID}. Quantity: {order.Quantity}. Unit price: {order.UnitPrice}.");
    }
}