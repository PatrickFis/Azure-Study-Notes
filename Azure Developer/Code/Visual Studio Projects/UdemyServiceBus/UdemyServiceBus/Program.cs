using Azure.Messaging.ServiceBus;
using Newtonsoft.Json;
using UdemyServiceBus;

string connectionString = "Connection string here";
string deadLetterConnectionString = String.Concat(connectionString, "/$DeadLetterQueue");
string queueName = "orderqueue";
string deadLetterQueueName = String.Concat(queueName, "/$DeadLetterQueue");

string[] Importance = new string[] { "High", "Medium", "Low" };

var orders = new List<Order>()
{
    new Order() { OrderID = "01", Quantity = 100, UnitPrice = 9.99F },
    new Order() { OrderID = "02", Quantity = 200, UnitPrice = 10.99F },
    new Order() { OrderID = "03", Quantity = 300, UnitPrice = 8.99F }
};

await SendMessage(orders);
//await GetProperties();
//await PeekMessages();
//await PeekAtSingleMessage();
//await ReceiveMessages(deadLetterConnectionString, deadLetterQueueName);

// Send messages to the queue and mark them as JSON. Add a custom Importance property to them. An optional time to live (TTL) is set on the messages to override the default 14 day setting.
async Task SendMessage(List<Order> orders, int timeToLive = default)
{
    ServiceBusClient serviceBusClient = new(connectionString);
    ServiceBusSender serviceBusSender = serviceBusClient.CreateSender(queueName);

    ServiceBusMessageBatch serviceBusMessageBatch = await serviceBusSender.CreateMessageBatchAsync();
    int i = 0;
    int messageId = 0;
    foreach(Order order in orders)
    {
        ServiceBusMessage serviceBusMessage = new ServiceBusMessage(JsonConvert.SerializeObject(order));
        serviceBusMessage.ContentType = "application/json";
        serviceBusMessage.ApplicationProperties.Add("Importance", Importance[i++ % 3]);
        if(timeToLive > 0)
        {
            serviceBusMessage.TimeToLive = TimeSpan.FromSeconds(timeToLive);
        }

        serviceBusMessage.MessageId = messageId.ToString();
        messageId++;

        if (!serviceBusMessageBatch.TryAddMessage(serviceBusMessage)) {
            throw new Exception("Error occurred while adding message to batch");
        }
    }

    Console.WriteLine("Sending messages");
    await serviceBusSender.SendMessagesAsync(serviceBusMessageBatch);

    await serviceBusSender.DisposeAsync();
    await serviceBusClient.DisposeAsync();
}

// Peek at messages in the queue
async Task PeekMessages()
{
    ServiceBusClient serviceBusClient = new(connectionString);
    ServiceBusReceiver serviceBusReceiver = serviceBusClient.CreateReceiver(queueName, new ServiceBusReceiverOptions() { ReceiveMode = ServiceBusReceiveMode.PeekLock });

    IAsyncEnumerable<ServiceBusReceivedMessage> messages = serviceBusReceiver.ReceiveMessagesAsync();

    await foreach(ServiceBusReceivedMessage message in messages)
    {
        Order order = JsonConvert.DeserializeObject<Order>(message.Body.ToString());
        Console.WriteLine($"Order ID: {order.OrderID}. Quantity: {order.Quantity}. Unit price: {order.UnitPrice}.");
    }
}

// Peek at a single message in the queue and then complete (delete) it
async Task PeekAtSingleMessage()
{
    ServiceBusClient serviceBusClient = new(connectionString);
    ServiceBusReceiver serviceBusReceiver = serviceBusClient.CreateReceiver(queueName, new ServiceBusReceiverOptions() { ReceiveMode = ServiceBusReceiveMode.PeekLock });

    ServiceBusReceivedMessage message = await serviceBusReceiver.ReceiveMessageAsync();

    Order order = JsonConvert.DeserializeObject<Order>(message.Body.ToString());
    Console.WriteLine($"Order ID: {order.OrderID}. Quantity: {order.Quantity}. Unit price: {order.UnitPrice}.");

    await serviceBusReceiver.CompleteMessageAsync(message);
}

// Receive and delete messages in the queue
async Task ReceiveMessages(string connectionString, string queueName)
{
    ServiceBusClient serviceBusClient = new(connectionString);
    ServiceBusReceiver serviceBusReceiver = serviceBusClient.CreateReceiver(queueName, new ServiceBusReceiverOptions() { ReceiveMode = ServiceBusReceiveMode.ReceiveAndDelete });

    IAsyncEnumerable<ServiceBusReceivedMessage> messages = serviceBusReceiver.ReceiveMessagesAsync();

    await foreach (ServiceBusReceivedMessage message in messages)
    {
        Order order = JsonConvert.DeserializeObject<Order>(message.Body.ToString());
        Console.WriteLine($"Order ID: {order.OrderID}. Quantity: {order.Quantity}. Unit price: {order.UnitPrice}.");
    }
}

// Peek at the messages in the queue and display their ID, sequence number, and custom importance property.
async Task GetProperties()
{
    ServiceBusClient serviceBusClient = new(connectionString);
    ServiceBusReceiver serviceBusReceiver = serviceBusClient.CreateReceiver(queueName, new ServiceBusReceiverOptions() { ReceiveMode = ServiceBusReceiveMode.PeekLock });

    IAsyncEnumerable<ServiceBusReceivedMessage> messages = serviceBusReceiver.ReceiveMessagesAsync();

    await foreach (ServiceBusReceivedMessage message in messages)
    {
        Console.WriteLine($"Message ID: {message.MessageId}. Sequence Number: {message.SequenceNumber}. Message Importance: {message.ApplicationProperties["Importance"]}.");
    }

    await serviceBusReceiver.DisposeAsync();
    await serviceBusClient.DisposeAsync();
}