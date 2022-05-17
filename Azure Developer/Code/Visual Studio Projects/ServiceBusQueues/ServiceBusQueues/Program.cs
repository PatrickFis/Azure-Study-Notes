using Azure.Messaging.ServiceBus;

namespace ServiceBusQueues
{
    internal class Program
    {
        private const string Value = @"
Enter 1 to send messages to the queue
Enter 2 to peek at the next message
Enter 3 to peek at the next message and delete it
Enter 4 to receive messages from the queue
Enter 0 to close the program";

        // This is a connection string to a shared access policy allowing you to send messages to the queue
        private static string sendMessageConnectionString = "Endpoint=sb://az204patrickservicebus.servicebus.windows.net/;SharedAccessKeyName=Send;SharedAccessKey=/R2YDZ1gAwi8/41K0esR+WqOPQbXnmuFaga6G6OkL1w=;EntityPath=appqueue";

        // This is a connection string to a shared access policy allowing you to listen to messages on the queue
        private static string listenMessageConnectionString = "Endpoint=sb://az204patrickservicebus.servicebus.windows.net/;SharedAccessKeyName=Listen;SharedAccessKey=OnvSga155kMNwNfZbPGF7Bg7tSGSwrPmlGET/UESzqQ=;EntityPath=appqueue";

        private static string queueName = "appqueue";

        static void Main(string[] args)
        {
            while (true)
            {
                Console.WriteLine(Value);
                var input = Console.ReadLine();
                if (input == null || input.Equals("0"))
                {
                    break;
                }
                else if (input.Equals("1"))
                {
                    SendMessagesToQueue();
                }
                else if (input.Equals("2"))
                {
                    PeekAtQueueMessage(false);
                }
                else if (input.Equals("3"))
                {
                    PeekAtQueueMessage(true);
                }
                else if (input.Equals("4"))
                {
                    ReceiveQueueMessages();
                }
            }





        }

        private static void SendMessagesToQueue(int timeToLive = 14)
        {
            List<Order> orders = new List<Order>()
            {
                new Order() {OrderID = "AA1", Quantity = 1, UnitPrice = 9.99m},
                new Order() {OrderID = "AB1", Quantity = 2, UnitPrice = 19.99m},
                new Order() {OrderID = "AC1", Quantity = 3, UnitPrice = 29.99m},
                new Order() {OrderID = "AD1", Quantity = 4, UnitPrice = 39.99m},
                new Order() {OrderID = "AE1", Quantity = 25, UnitPrice = 49.99m}
            };

            ServiceBusClient client = new ServiceBusClient(sendMessageConnectionString);

            ServiceBusSender sender = client.CreateSender(queueName);

            foreach (Order _order in orders)
            {
                ServiceBusMessage message = new ServiceBusMessage(_order.ToString());
                message.ContentType = "application/json";
                sender.SendMessageAsync(message).GetAwaiter().GetResult();
            }

            Console.WriteLine("Messages sent");
        }

        private static void PeekAtQueueMessage(bool deleteMessage)
        {
            ServiceBusClient client = new ServiceBusClient(listenMessageConnectionString);

            ServiceBusReceiver receiver = client.CreateReceiver(queueName, new ServiceBusReceiverOptions()
            {
                ReceiveMode = ServiceBusReceiveMode.PeekLock
            });

            ServiceBusReceivedMessage message = receiver.ReceiveMessageAsync().GetAwaiter().GetResult();

            Console.WriteLine(message.Body.ToString());

            if (deleteMessage)
            {
                receiver.CompleteMessageAsync(message);
            }
        }

        private static void ReceiveQueueMessages()
        {
            ServiceBusClient client = new ServiceBusClient(listenMessageConnectionString);

            ServiceBusReceiver receiver = client.CreateReceiver(queueName, new ServiceBusReceiverOptions()
            {
                ReceiveMode = ServiceBusReceiveMode.ReceiveAndDelete
            });

            var messages = receiver.ReceiveMessagesAsync(100).GetAwaiter().GetResult();

            foreach (var message in messages)
            {
                Console.WriteLine(message.Body.ToString());
            }
        }
    }
}