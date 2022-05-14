using Azure.Messaging.EventHubs.Producer;
using Azure.Messaging.EventHubs;
using System.Text;
using Azure.Messaging.EventHubs.Consumer;

namespace EventHub
{
    internal class Program
    {
        private static readonly string sendEventsConnectionString = "replace me";
        private static readonly string listenEventsConnectionString = "replace me";
        private static readonly string eventConsumerGroup = "$Default";


        static async Task Main(string[] args)
        {
            //SendEvents();

            ReceiveEvents();
        }

        static void SendEvents()
        {
            
            // Events to send
            List<Order> orders = new List<Order>()
            {
                new Order() {OrderID="O1",Quantity=10,UnitPrice=9.99m,DiscountCategory="a"},
                new Order() {OrderID="O2",Quantity=15,UnitPrice=19.99m,DiscountCategory="b"},
                new Order() {OrderID="O3",Quantity=20,UnitPrice=29.99m,DiscountCategory="c"},
                new Order() {OrderID="O4",Quantity=25,UnitPrice=39.99m,DiscountCategory="d"},
                new Order() {OrderID="O5",Quantity=30,UnitPrice=49.99m,DiscountCategory="e"}
            };

            // Sends events to Event Hub
            EventHubProducerClient client = new EventHubProducerClient(sendEventsConnectionString);

            // The client sends events in batches, so that's what this is
            EventDataBatch batch = client.CreateBatchAsync().GetAwaiter().GetResult();

            // Add the events to the batch encoded in UTF8
            foreach (Order order in orders)
            {
                batch.TryAdd(new EventData(Encoding.UTF8.GetBytes(order.ToString())));
            }

            // Send the events using the client
            client.SendAsync(batch).GetAwaiter().GetResult();

            Console.WriteLine("Events sent successfully");
        }

        static async void ReceiveEvents()
        {
            // Create a client to receive the events
            EventHubConsumerClient client = new EventHubConsumerClient(eventConsumerGroup, listenEventsConnectionString);

            // Iterate over the events
            await foreach (PartitionEvent _event in client.ReadEventsAsync())
            {
                // Print out information about the event
                Console.WriteLine($"Partition ID {_event.Partition.PartitionId})");
                Console.WriteLine($"Data Offset {_event.Data.Offset})");
                Console.WriteLine($"Sequence Number {_event.Data.SequenceNumber})");
                Console.WriteLine($"Partition Key {_event.Data.PartitionKey})");
                Console.WriteLine($"Event Body {Encoding.UTF8.GetString(_event.Data.EventBody)}");
            }

            // Wait for user input so that the events can be read
            Console.ReadKey();
        }
    }
}