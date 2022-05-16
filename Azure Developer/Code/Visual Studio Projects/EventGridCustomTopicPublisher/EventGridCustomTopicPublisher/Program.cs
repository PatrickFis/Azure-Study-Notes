using Azure;
using Azure.Messaging.EventGrid;
using System.Text.Json;

namespace EventGridCustomTopicPublisher
{
    internal class Program
    {
        private static Uri topicEndpoint = new Uri("replace me");
        private static AzureKeyCredential topicAccessKey = new AzureKeyCredential("replace me");
        static void Main(string[] args)
        {
            EventGridPublisherClient client = new EventGridPublisherClient(topicEndpoint, topicAccessKey);

            Order order = new Order()
            {
                OrderID = "A1",
                UnitPrice = 9.99m,
                Quantity = 1337
            };

            List<EventGridEvent> orders = new List<EventGridEvent>()
            {
                new EventGridEvent("Placing new order", "app.orderEvent", "1.0", JsonSerializer.Serialize(order))
            };

            client.SendEvents(orders);
        }
    }
}