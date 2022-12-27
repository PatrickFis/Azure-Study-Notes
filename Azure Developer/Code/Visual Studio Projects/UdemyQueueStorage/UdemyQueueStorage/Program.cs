using Azure.Storage.Queues;
using Azure.Storage.Queues.Models;
using Newtonsoft.Json;
using UdemyQueueStorage;

string connectionString = "Connection string here";
string queueName = "appqueue";

// Methods for interacting with the storage queue from this app
//SendMessage("Test Message 1");
//SendMessage("Test Message 2");
//Console.WriteLine($"The number of messages in the queue is {GetQueueLength()}.");
//PeekMessage();
//ReceiveMessage();

// Methods to send objects to the storage queue
//SendOrderMessage("O1", 100);
//SendOrderMessage("O2", 200);
//PeekAtOrderMessage();

// Methods to send base 64 encoded messages to be used by an Azure Function
SendOrderMessageInBase64("O1", 100);
SendOrderMessageInBase64("O2", 200);

// Send messages to a storage account queue
void SendMessage(string message)
{
    QueueClient queueClient = new(connectionString, queueName);

    if(queueClient.Exists())
    {
        queueClient.SendMessage(message);
        Console.WriteLine("The message has been sent");
    }
}

void SendOrderMessage(string orderId, int quantity)
{
    QueueClient queueClient = new(connectionString, queueName);

    if (queueClient.Exists())
    {
        Order order = new() { Id = orderId, Quantity = quantity };
        queueClient.SendMessage(JsonConvert.SerializeObject(order));
        Console.WriteLine("The order has been sent");
    }
}

void SendOrderMessageInBase64(string orderId, int quantity)
{
    QueueClient queueClient = new(connectionString, queueName);

    if (queueClient.Exists())
    {
        Order order = new() { Id = orderId, Quantity = quantity };
        var jsonObject = JsonConvert.SerializeObject(order);
        var bytes = System.Text.Encoding.UTF8.GetBytes(jsonObject);
        var message = System.Convert.ToBase64String(bytes);

        queueClient.SendMessage(message);
        Console.WriteLine("The order has been sent");
    }
}

// Peek at the messages in the queue
void PeekMessage()
{
    QueueClient queueClient = new(connectionString, queueName);
    int maxMessages = 10;

    PeekedMessage[] peekMessages = queueClient.PeekMessages(maxMessages);

    Console.WriteLine("The messages in the queue are:");
    foreach(var peekMessage in peekMessages)
    {
        Console.WriteLine(peekMessage.Body);
    }
}

void PeekAtOrderMessage()
{
    QueueClient queueClient = new(connectionString, queueName);
    int maxMessages = 10;

    PeekedMessage[] peekMessages = queueClient.PeekMessages(maxMessages);

    Console.WriteLine("The messages in the queue are:");
    foreach (var peekMessage in peekMessages)
    {
        Order order = JsonConvert.DeserializeObject<Order>(peekMessage.Body.ToString());
        Console.WriteLine($"ID: {order.Id}. Quantity: {order.Quantity}.");
    }
}

// Receive messages from the queue and delete them afterwards
void ReceiveMessage()
{
    QueueClient queueClient = new(connectionString, queueName);
    int maxMessages = 10;

    QueueMessage[] queueMessages = queueClient.ReceiveMessages(maxMessages);

    Console.WriteLine("The messages received from the queue are:");
    foreach (var message in queueMessages)
    {
        Console.WriteLine(message.Body);
        queueClient.DeleteMessage(message.MessageId, message.PopReceipt);
    }
}

// Get a count of messages in the queue
int GetQueueLength()
{
    QueueClient queueClient = new(connectionString, queueName);

    if (queueClient.Exists())
    {
        QueueProperties properties = queueClient.GetProperties();
        return properties.ApproximateMessagesCount;
    }
    return 0;
}