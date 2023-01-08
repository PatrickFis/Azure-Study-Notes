using Azure;
using Azure.Messaging.EventGrid;
using Newtonsoft.Json;
using UdemyPublishEvents;

Uri topicEndpoint = new Uri("Topic endpoint here");
AzureKeyCredential azureKeyCredential = new AzureKeyCredential("Access key for the topic here");

var orders = new List<Order>()
{
    new Order() { OrderId = "O1", UnitPrice = 9.99f, Quantity = 100},
    new Order() { OrderId = "O2", UnitPrice = 10.99f, Quantity = 200},
    new Order() { OrderId = "O3", UnitPrice = 11.99f, Quantity = 300}
};

EventGridPublisherClient eventGridPublisherClient = new(topicEndpoint, azureKeyCredential);

string subject = "Adding new order";
string eventType = "app.neworder";
string dataVersion = "1.0";

List<EventGridEvent> events = new();

foreach(Order order in orders)
{
    EventGridEvent eventGridEvent = new(subject, eventType, dataVersion, JsonConvert.SerializeObject(order));
    events.Add(eventGridEvent);
}

await eventGridPublisherClient.SendEventsAsync(events);