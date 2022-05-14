# Azure Event Grid
Azure Event Grid simplifies consumption and lowers costs by eliminating the need for constant polling. Event Grid routes events from Azure and non-Azure resources and distributes the events to registered subscriber endpoints.

## Features
- Enables event-drive, reactive programming
- Operates on a pub-sub (publish-subscribe) model
- Publishers emit events but they don't specify how they should be handled, subscribers decide which events they want to handle
- Event Grid allows you to select the Azure resource you want to subscribe to and then lets you specify an event handler of Webhook endpoint to send the event to
- Event Grid supports your own events using custom topics
- Filters can be used to route specific events to different endpoints, multicast to multiple endpoints, and make sure your messages are reliably delivered

## Concepts
Azure Event Grid has five key concepts
- Events - What happened. An event is the smallest amount of info that fully describes something that happened in the system. Every event has common info like: source, time, and unique identifier. Every event also has specific information only relevant to the specific type of event. Events of size up to 64 KB are covered by a General Availability (GA) SLA. Support for events up to 1 MB is currently in preview. Events over 64 KB are charged in 64 KB increments.
- Event sources - Where an event happened. Your apps can be the source for custom events. Event sources are responsible for sending their events to Event Grid.
- Topics - Topics provide an endpoint for event sources to send their events. The publisher creates the topic and decides if a source needs one or more topics. Topics are collections of related events. Subscribers can subscribe to topics to respond to the events. There are two types of topics.
  - System topics - Built-in topics provided by Azure services. Not seen in your subscription but you can subscribe to them if you have access to the resource.
  - Custom topics - App and third-party topics.
- Event subscriptions -Subscriptions tell Event Grid the topics that you're interested in receiving. When creating the subscription you provide an endpoint for handling the event. The events can be filtered by event type or subject pattern. You can also set an expiration for the subscription so that you don't have to worry about cleaning it up if it's only needed for a limited time.
- Event handlers - An event handler is the place where an event is sent to be processed. You can use an Azure service or your own Webhook as the handler. Depending on the type of handler Event Grid uses different ways to guarantee that the event was delivered. 

## Event Schemas
All publishers publish events with the following properties
``` json
[
  {
    "topic": string,
    "subject": string,
    "id": string,
    "eventType": string,
    "eventTime": string,
    "data":{
      object-unique-to-each-publisher
    },
    "dataVersion": string,
    "metadataVersion": string
  }
]
```

Event properties:
| Property        | Type   | Required                                                                                                                                              | Description                                                                                                                          |
| --------------- | ------ | ----------------------------------------------------------------------------------------------------------------------------------------------------- | ------------------------------------------------------------------------------------------------------------------------------------ |
| topic           | string | No. If not included, Event Grid will stamp onto the event. If included it must match the event grid topic Azure Resource Manager ID exactly.          | Full resource path to the event source. This field isn't writeable. Event Grid provides this value.                                  |
| subject         | string | Yes                                                                                                                                                   | Publisher-defined path to the event subject.                                                                                         |
| eventType       | string | Yes                                                                                                                                                   | One of the registered event types for this event source.                                                                             |
| eventTime       | string | Yes                                                                                                                                                   | The time the event is generated based on the provider's UTC time.                                                                    |
| id              | string | Yes                                                                                                                                                   | Unique identifier for the event.                                                                                                     |
| data            | object | No                                                                                                                                                    | Event data specific to the resource provider.                                                                                        |
| dataVersion     | string | No. If not included it will be stamped with an empty value.                                                                                           | The schema version of the data object. The publisher defines the schema version.                                                     |
| metadataVersion | string | No. If not included, Event Grid will stamp onto the event. If included, must match the Event Grid Schema metadataVersion exactly (currently, only 1). | The schema version of the event metadata. Event Grid defines the schema of the top-level properties. Event Grid provides this value. |

For custom topics the event publisher determines the data object. Custom topics should also create subjects for events that make it easy for subscribers to use to determine if they're interested in the event.

Event Grid also supports the JSON implementation of CloudEvents v1.0 and HTTP protocol binding. Event Grid supports transformations on the wire between CloudEvents schema and the Event Grids schema.

## Event Delivery Durability
- Event Grid tries to deliver each event at least once for each matching subscription immediately. If there isn't an acknowledgement or if a failure occurs then Event Grid will retry delivery based on a fixed retry schedule and retry policy. Note that Event Grid doesn't guarantee the order that events are delivered so subscribers may receive them out of order.
- Delivery failures cause Event Grid to decide to retry the event, dead-letter (note: this must be configured or events will be dropped if they would be dead-lettered) it, or drop the event based on the error. Events will be dead-lettered if a retry would not fix the error (which can be determined by the error code).
  - Delivery will not be retried for Azure resources for the following error codes: 400 Bad Request, 413 Request Entity Too Large, 403 Forbidden
  - Delivery will not be retried for Webhooks for the following error codes: 400 Bad Request, 413 Request Entity Too Large, 403 Forbidden, 404 Not Found, 401 Unauthorized
- Event Grid will wait for 30 seconds for a response from an endpoint before queuing the message for retry. Event Grid uses an exponential backoff retry policy for event delivery.
- Event Grid will try to remove the event from the retry queue if it receives a response within 3 minutes, but duplicates may still be received. Event Grid may skip certain retries if an endpoint is consistently unhealthy, down for a long period, or appears to be overwhelmed.
- Retry policy can be configured to drop events if the following two limits are reached
  - Maximum number of attempts - 1-30, default 30
  - Event time-to-live (TTL) - 1-1440, default 1440 minutes
- Event Grid can send events in batches (used for high-throughput and disabled by default) with two settings
  - Max events per batch - 1-5000. Batch creation is not delayed if there are not enough events to fill the entire batch.
  - Preferred batch size in kilobytes - Target ceiling for batch size. The batch may be smaller if there aren't enough events, and it is possible that batches will be larger if a single event is larger than the preferred size.
- Event Grid delays delivery and retries of events to endpoints that are assumed to be experiencing issues.
- Events are dead-lettered when one of the following conditions is met
  - Event isn't delivered within TTL period
  - Number of retries has exceeded the limit
- Event subscriptions allow you to setup HTTP headers to be included in delivered events. 10 headers can be set up when creating an event subscription and each one should not exceed 4,096 (4K) bytes.

## Access Control
- Access is controled through Azure RBAC and four built-in roles are provided.
  - Event Grid Subscription Reader - Lets you read Event Grid event subscriptions
  - Event Grid Subscription Contributor - Lets you manage Event Grid event subscription operations
  - Event Grid Contributor - Lets you create and manage Event Grid resources
  - Event Grid Data Sender - Lets you send events to Event Grid topics
- You must have the Microsoft.EventGrid/EventSubscriptions/Write permission on the resource that is the event source to create an event subscription for the resource.
  - The resource for system topics is `/subscriptions/{subscription-id}/resourceGroups/{resource-group-name}/providers/{resource-provider}/{resource-type}/{resource-name}` while the resource for custom topics is `/subscriptions/{subscription-id}/resourceGroups/{resource-group-name}/providers/Microsoft.EventGrid/topics/{topic-name}`.

## Receiving events using Webhooks
- Event Grid supports POSTing HTTP requests to Webhook endpoints with events in the request body
- Event Grid requires you to prove that you own a Webhook endpoint before delivering events to prevent attackers from flooding your endpoint. Azure automatically validates ownership for the following three Azure services
  - Azure Logic Apps with Event Grid Connector
  - Azure Automation via webhook
  - Azure Functions with Event Grid Trigger
- Endpoints can be validated in two different ways (there is a third option in preview, might need to look into that since it says 2018)
  - Synchronous handshake - Event Grid will send a validation event to your endpoint which includes a validationCode property. Your application will validate the event and return the validation code in the response synchronously.
  - Asynchronous handshake - Certain cases don't allow you to response with the validation code synchronously.
  - The 2018-05-01 preview has added support for a manual validation handshake that contains a URL that can be used to validate the subscription.

## Filtering Events
Events can be filtered on the following
- Event types
  - You may want to only send certain event types to your endpoint
``` json
"filter": {
  "includedEventTypes": [
    "Microsoft.Resources.ResourceWriteFailure",
    "Microsoft.Resources.ResourceWriteSuccess"
  ]
}
```
- Subject begins or ends with
``` json
"filter": {
  "subjectBeginsWith": "/blobServices/default/containers/mycontainer/log",
  "subjectEndsWith": ".jpg"
}
```
- Advanced fields and operators
  - Advanced filtering allows you to specify an operator type (type of comparison), a key (the field in the event data that you're using to filter, can be a number, boolean, or string), and value or values (the value or values to compare to the key)
``` json
"filter": {
  "advancedFilters": [
    {
      "operatorType": "NumberGreaterThanOrEquals",
      "key": "Data.Key1",
      "value": 5
    },
    {
      "operatorType": "StringContains",
      "key": "Subject",
      "values": ["container1", "container2"]
    }
  ]
}
```

## Setting up Event Grid using the CLI
1. Set up environment variables
``` bash
let rNum=$RANDOM*$RANDOM
myLocation=eastus
myTopicName="az204-egtopic-${rNum}"
mySiteName="az204-egsite-${rNum}"
mySiteURL="https://${mySiteName}.azurewebsites.net"
```
2. Create a resource group
``` bash
az group create --name az204-eventgrid-patrick-rg --location $myLocation
```
2. Enable an Event Grid resource provider (only needs to be run on subscriptions that haven't previously used Event Grid)
``` bash
az provider register --namespace Microsoft.EventGrid

# Query the status as it can take a few minutes
az provider show --namespace Microsoft.EventGrid --query "registrationState"
```
3. Create a custom topic
``` bash
az eventgrid topic create --name $myTopicName \
    --location $myLocation \
    --resource-group az204-eventgrid-patrick-rg
```
4. Create a message endpoint and open the website
``` bash
az deployment group create \
    --resource-group az204-eventgrid-patrick-rg \
    --template-uri "https://raw.githubusercontent.com/Azure-Samples/azure-event-grid-viewer/main/azuredeploy.json" \
    --parameters siteName=$mySiteName hostingPlanName=viewerhost

echo "Your web app URL: ${mySiteURL}"
```
5. Subscribe to a custom topic
``` bash
endpoint="${mySiteURL}/api/updates"
subId=$(az account show --subscription "" | jq -r '.id')

az eventgrid event-subscription create \
    --source-resource-id "/subscriptions/$subId/resourceGroups/az204-eventgrid-patrick-rg/providers/Microsoft.EventGrid/topics/$myTopicName" \
    --name az204ViewerSub \
    --endpoint $endpoint
```
6. Send an event to the custom topic
``` bash
# Retrieve URLs and a key
topicEndpoint=$(az eventgrid topic show --name $myTopicName -g az204-eventgrid-patrick-rg --query "endpoint" --output tsv)
key=$(az eventgrid topic key list --name $myTopicName -g az204-eventgrid-patrick-rg --query "key1" --output tsv)

# Create event data
event='[ {"id": "'"$RANDOM"'", "eventType": "recordInserted", "subject": "myapp/vehicles/motorcycles", "eventTime": "'`date +%Y-%m-%dT%H:%M:%S%z`'", "data":{ "make": "Contoso", "model": "Monster"},"dataVersion": "1.0"} ]'

# Send the event
curl -X POST -H "aeg-sas-key: $key" -d "$event" $topicEndpoint
```
7. Verify that an event was received on the website that was created earlier
8. Clean up the resource group when you're done
``` bash
az group delete --name az204-eventgrid-patrick-rg --no-wait
```


# Azure Event Hubs
Azure Event Hubs is a big data streaming platform and event ingestion service. It can transform data and store it in real time using an analytics provider or batching/storage adapters.

## Features & Concepts
- Event Hubs is a front door for an event pipeline (aka an event ingestor). An event ingestor is a component or service that sits between publishers and consumers and decouples the production of an event stream from the consumption of those events.

### Key Concepts
- Event Hub client - Primary interface for devs to interact with the client library. There are various clients available for publishing or consuming events.
  - Event Hub producer - Client which serves as a source of data from an application
  - Event Hub consumer - Client which reads info from Event Hub and processes it (often something like Azure Stream Analytics, Apache Spark, or Apache Storm)
- Partition - Ordered sequence of events in Event Hub. Partitions are used to organize data associated with the parallelism required by event consumers. The number of partitions is specified when an event hub is created and cannot be changed.
- Consumer group - A view of the entire Event Hub. This allows multiple consumers to each have a separate view of an event stream so that they can read it independently at their own pace. There can be at most 5 concurrent readers on a partition per consumer group, though it is recommended that there only be one active consumer for a given partition and consumer group pairing. Each active reader receives all the events from the partition, so if there are multiple readers then they'll receive duplicate events.
- Event receivers - Something that reads event data from an event hub. Consumers connect via AMQP 1.0 sessions.
- Throughput units or processing units - Pre-purchased units of capacity that control your throughput capacity.

## Event Hub Capture
Event Hub allows you to store data in a blob or a data lake at no extra cost.
- Event Hubs Capture allows you to store off data in blob storage or a data lake.
- Captured data is written in the Apache Avro format.
- Captured data does not use your egress quota as it reads directly from internal Event Hubs storage.
- Scaling is controlled by throughput units. One throughput unit = 1 MB per second or 1000 events per second of ingress and twice that amount of egress. Standard Event Hubs can be configured for 1-20 throughput units (more can be requested through a support request).

## Scaling your processing application (see MS Learn)
https://docs.microsoft.com/en-us/learn/modules/azure-event-hubs/4-event-processing

## Access Control
Access to Event Hubs can be managed through AAD and shared access signatures. Access to the data in Event Hubs can be managed through AAD and OAuth with these roles
- Azure Event Hubs Data Owner: Use this role to give complete access to Event Hubs resources.
- Azure Event Hubs Data Sender: Use this role to give send access to Event Hubs resources.
- Azure Event Hubs Data Receiver: Use this role to give receiving access to Event Hubs resources.

## Usage example (TODO)


# Misc
Table generated from https://www.tablesgenerator.com/markdown_tables.


# Studying from Youtube Event Grid [Link](https://www.youtube.com/watch?v=ekJFp3TJN14&list=PLLc2nQDXYMHpekgrToMrDpVtFtvmRSqVt&index=17)

# Studying from Youtube Event Hubs [Link](https://www.youtube.com/watch?v=HwZldR8KlKM&list=PLLc2nQDXYMHpekgrToMrDpVtFtvmRSqVt&index=18)
##  Creating a new Event Hubs resource
1. Go to the portal and start creating an Event Hubs resource
2. Select a subscription and resource group
3. Enter a name for the namespace and the other options
4. Create the resource

## Add an Event Hub to the new resource
1. After the deployment has finished navigate to your new resource
2. Click on the + Event Hub button at the top
3. Give the Event Hub a name and decide on partitions, retention, and if data capture is needed
4. Create the Event Hub

## Interact with the Event Hub in a C# console application to send events
1. Generate a console application as usual in Visual Studio.
2. Add the Azure.Messaging.EventHubs dependency
3. Navigate to the Shared access policies for the hub that you created in the previous step
4. Click + Add to add a new policy
5. Give the policy a name and the Send permission
6. Click on the policy and copy either of the connection strings
7. Store the connection string in a variable in the console app
8. Create a new class and override the ToString() method to serialize the object as JSON
9. Create a list of the class in step 8 to send to the Event Hub
10. Create an EventHubProducerClient using the connection string from step 6 for sending events
11. Use the client to create an EventDataBatch
12. Iterate over the list of objects from step 9 and add them as EventData to the batch from step 11
13. Send the events to Event Hub using the client
14. Run the program

## Interact with the Event Hub in a C# console application to receive events (modify program from previous section)
1. Using the existing Event Hub add another Shared access policy that allows for listening and store it in a variable in the program
2. To receive events you need to use a consumer group and Event Hubs comes with a default consumer group of $Default that can be used
3. Create an EventHubConsumerClient using the consumer group and connection string from step 1 and 2
4. Receive the events using the client and print out information about them
5. Run the program (and note that it can be run multiple times)
   1. Events are not deleted from Event Hubs as the point is to allow multiple readers to read the data at the same time. Events are instead deleted after the retention period expires.