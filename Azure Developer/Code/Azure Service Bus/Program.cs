using System;
using System.Threading.Tasks;
using Azure.Messaging.ServiceBus;

namespace MyApp // Note: actual namespace depends on the project name.
{
    internal class Program
    {
        static string connectionString = "Endpoint=sb://az204svcbus11752.servicebus.windows.net/;SharedAccessKeyName=RootManageSharedAccessKey;SharedAccessKey=2Tqzooj75w+UUwKiY1a6C/uShKBLQbkW52TuaENOTYA=";
        static string queueName = "az204-queue";
        static ServiceBusClient client;
        static ServiceBusSender sender;
        static ServiceBusProcessor processor;
        private const int numberOfMessages = 3;

        static async Task Main()
        {
            await SendMessages();
            await ReceiveMessages();
        }

        static async Task SendMessages()
        {
            // Set up clients
            client = new ServiceBusClient(connectionString);
            sender = client.CreateSender(queueName);

            // Create a batch of messages, note the using keyword which is basically shorthand for try-with-resources and AutoClosable from Java
            using ServiceBusMessageBatch messageBatch = await sender.CreateMessageBatchAsync();
            for (int i = 0; i < numberOfMessages; i++)
            {
                // Add the messages to a batch
                if (!messageBatch.TryAddMessage(new ServiceBusMessage($"Message {i}")))
                {
                    throw new Exception($"Exception {i} has occurred.");
                }
            }

            // Try sending the messages
            try
            {
                await sender.SendMessagesAsync(messageBatch);
                Console.WriteLine($"A batch of {numberOfMessages} messages has been published to the queue.");
            }
            finally
            {
                // Dispose of the clients
                await sender.DisposeAsync();
                await client.DisposeAsync();
            }
        }

        static async Task ReceiveMessages()
        {
            client = new ServiceBusClient(connectionString);
            processor = client.CreateProcessor(queueName, new ServiceBusProcessorOptions());

            try
            {
                // += means to subscribe to the event
                processor.ProcessMessageAsync += HandleMessages;
                processor.ProcessErrorAsync += ErrorHandler;

                await processor.StartProcessingAsync();

                Console.WriteLine("Waiting for user input to exit");
                Console.ReadKey();

                await processor.StopProcessingAsync();
            }
            finally
            {
                await processor.DisposeAsync();
                await client.DisposeAsync();
            }
        }

        static async Task HandleMessages(ProcessMessageEventArgs args)
        {
            string body = args.Message.Body.ToString();
            Console.WriteLine($"Received: {body}");

            // Complete the message and delete it from the queue
            await args.CompleteMessageAsync(args.Message);
        }

        static Task ErrorHandler(ProcessErrorEventArgs args)
        {
            Console.WriteLine(args.Exception.ToString());
            return Task.CompletedTask;
        }
    }
}