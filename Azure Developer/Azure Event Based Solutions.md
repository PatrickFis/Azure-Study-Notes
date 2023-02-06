# Azure Event Grid
Azure Event Grid simplifies consumption and lowers costs by eliminating the need for constant polling. Event Grid routes events from Azure and non-Azure resources and distributes the events to registered subscriber endpoints.

## Features
- Enables event-driven, reactive programming
- Operates on a pub-sub (publish-subscribe) model
- Publishers emit events but they don't specify how they should be handled, subscribers decide which events they want to handle
- Event Grid allows you to select the Azure resource you want to subscribe to and then lets you specify an event handler or Webhook endpoint to send the event to
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
- Azure Event Hubs is a big data streaming platform and event ingestion service. It can transform data and store it in real time using an analytics provider or batching/storage adapters.
- Event Hubs is useful for some of the following scenarios:
  - Anomaly detection (fraud/outliers)
  - Application logging
  - Analytics pipelines, such as clickstreams
  - Live dashboards
  - Archiving data
  - Transaction processing
  - User telemetry processing
  - Device telemetry streaming

## Features & Concepts
- Event Hubs is a front door for an event pipeline (aka an event ingestor). An event ingestor is a component or service that sits between publishers and consumers and decouples the production of an event stream from the consumption of those events.

### Key Concepts
- Event Hub client - Primary interface for devs to interact with the client library. There are various clients available for publishing or consuming events.
  - Event Hub producer - Client which serves as a source of data from an application
  - Event Hub consumer - Client which reads info from Event Hub and processes it (often something like Azure Stream Analytics, Apache Spark, or Apache Storm)
- Partition - Ordered sequence of events in Event Hub. Partitions are used to organize data associated with the parallelism required by event consumers. The number of partitions is specified when an event hub is created and cannot be changed (except at the dedicated cluster and premium tier).
  - https://learn.microsoft.com/en-us/azure/event-hubs/event-hubs-scalability#partitions has good information on partitions.
  - Partitions can be thought of as a "commit log."
  - Under the hood maintaining a log that preserves the order of events requires that events are kept together in the underlying storage and its replicas. This results in a throughput ceiling for the log. Partitions allow for multiple parallel logs to be used for the same event hub and multiply the available raw IO throughput capacity.
  - Processing events may require a large amount of parallel processing capacity. Partitions are how your solution feeds those processes and yet ensures that each event has a clear processing owner.
  - The recommendation is that you choose to create at least as many partitions as you expect that you'll need during the peak load of your application for that particular event hub.
  - Changing the partition count for a dedicated or premium tier event hub will change the distribution of streams across partitions because the mapping of partition keys will change, so changing the count should be avoided at these tiers if your application cares about the relative order of events.
  - Event streams need to be structured in a way that lets you take advantage of having multiple partitions. If you need absolute order preservation then you may not be able to use many partitions, and extra partitions make the processing side of things more complex.
  - Partition count doesn't affect pricing. That's entirely affected by pricing units (throughput units for the standard tier, processing units for premium, and capacity units for dedicated).
  - General recommendation: Create 1 partition per 1 MB/s of expected throughput
  - Data is mapped to a particular partition using a partition key.
    - The partition key can be sent by the sender which event hubs will pass through a hashing function to create the partition assignment. If you don't specify the key then a round-robin assignment is used.
- Consumer group - A view of the entire Event Hub. This allows multiple consumers to each have a separate view of an event stream so that they can read it independently at their own pace. There can be at most 5 concurrent readers on a partition per consumer group, though it is recommended that there only be one active consumer for a given partition and consumer group pairing. Each active reader receives all the events from the partition, so if there are multiple readers then they'll receive duplicate events.
- Event receivers - Something that reads event data from an event hub. Consumers connect via AMQP 1.0 sessions.
- Throughput units or processing units - Pre-purchased units of capacity that control your throughput capacity.

## Event Hub Capture
Event Hub allows you to store data in a blob or a data lake at no extra cost.
- Event Hubs Capture allows you to store off data in blob storage or a data lake.
- Captured data is written in the Apache Avro format.
- Captured data does not use your egress quota as it reads directly from internal Event Hubs storage.
- Scaling is controlled by throughput units. One throughput unit = 1 MB per second or 1000 events per second of ingress and twice that amount of egress. Standard Event Hubs can be configured for 1-20 throughput units (more can be requested through a support request).
- Capture lets you configure a window to control capturing behavior. The window is a minimum size and time configuration with a "first wins policy," so the first trigger encountered causes a capture operation. Example: 15 minute, 100 MB capture window with 1 MB of data sent per second. The size window triggers before the time window.
- Storage naming convention: {Namespace}/{EventHub}/{PartitionId}/{Year}/{Month}/{Day}/{Hour}/{Minute}/{Second}
  - Example: https://mystorageaccount.blob.core.windows.net/mycontainer/mynamespace/myeventhub/0/2017/12/08/03/03/17.avro
- Captures are the easiest way to get data into Azure. Batch processing can be done on captures using Azure Data Lake, Azure Data Factory, and Azure HDInsight.

## Scaling your processing application (see MS Learn)
https://docs.microsoft.com/en-us/learn/modules/azure-event-hubs/4-event-processing and https://learn.microsoft.com/en-us/azure/event-hubs/event-processor-balance-partition-load
- To scale an event processing application you can run multiple instances of the app and have them load balanced against each other.
- When designing a consumer application you should handle the following requirements:
  - Scale: Create multiple consumers and have each one take ownership of a few partitions
  - Load balance: Increase or reduce the amount of consumers dynamically and have them balance the number of partitions each consumer owns.
  - Seamless resume on failures: If a consumer fails other consumers can pick up the partitions that the failed consumer was handling and continue. The continuation point (the checkpoint or offset) should be at the exact point at which the failed consumer failed, or slightly before.
  - Consume events: Write code to actually do something useful with your events.
- Microsoft provides SDKs for processing events.
  - Their SDK can be used to let clients work cooperatively within a consumer group.
  - Clients can automatically manage distribution and balancing of work as instances become available or unavailable for the group.
- Event processors typically own and process events from one or more partitions.
- Event processors are given unique identifiers and claim ownership of partitions by adding or updating an entry in a checkpoint store.
- Receiving events are done by functions that you specify to process events and errors. Each call will deliver a single event from the partition to your application. Generally you'll want to process events fast, so if you need to do more than one thing you should create multiple consumer groups and event processors.
- Checkpointing is the process that an event processor uses to mark or commit the position of the last successfully processed event within a partition.
  - Other instances can resume from the checkpoint if the processor goes down.
  - Users need to decide how often to update the checkpoint. Updating after even successfully processed event can have performance and cost implications as it triggers a write operation to the underlying checkpoint store. Checkpointing after each event also means that you should consider using a queued messaging pattern, so Service Bus may be a better option for you than Event Hubs.
- While somewhat old, https://devblogs.microsoft.com/azure-sdk/eventhubs-clients/ has information about the classes used for consuming, producing, and processing events. Short overview:
  - EventHubProducerClient: This class is responsible for publishing event data to Event Hubs.
  - EventHubConsumerClient: This class is really an onboarding point for consuming events from all partitions without the rigor and complexity of a production application. It's intended to help get developers up to speed. For production scenarios the recommendation is to look into the EventProcessClient or EventProcess`<TPartition>` classes over EventHubConsumerClient.
  - EventProcessorClient: This class is intended to be used in production scenarios. It's used to read and process events from all partitions of an Event Hub and it collaborates with other EventProcessorClient instances using the same Event Hub and consumer group to balance work between them. It has a high degree of fault tolerance built-in. It allows the application to track progress through checkpointing events that have been processed.

## Access Control
Access to Event Hubs can be managed through AAD and shared access signatures. 
- AAD: Access to the data in Event Hubs can be managed through AAD and OAuth with these roles
  - Azure Event Hubs Data Owner: Use this role to give complete access to Event Hubs resources.
  - Azure Event Hubs Data Sender: Use this role to give send access to Event Hubs resources.
  - Azure Event Hubs Data Receiver: Use this role to give receiving access to Event Hubs resources.
  - AAD and OAuth 2.0 provides superior security and ease of use over SAS.
- SAS:
  - SAS lets you provide limited access to Event Hubs resources.
  - You can add time constraints or only grant access to certain permissions.
  - AAD offers similar capabilities to SAS without needing to worry about managing SAS tokens or revoking a compromised SAS.

# Studying from Youtube Event Grid [Link](https://www.youtube.com/watch?v=ekJFp3TJN14&list=PLLc2nQDXYMHpekgrToMrDpVtFtvmRSqVt&index=17)
## Setting up Event Grid
1. Create a new resource group with a storage account in it
2. Create a new container in the storage account that will be used to send events
3. Create a new Azure Function and set up an Event Grid trigger
4. Navigate to the Events section on the storage account and create a new subscription for blob events that sends events to the Azure Function in step 3
5. Upload or delete a file in the container created in step 2 while viewing the Azure Function's log stream and note the information output by the function

## Event Grid Filters
- Filters can be added while creating an event subscription or afterwards by navigating to the subscription and clicking on the filter button
- To add simple filtering on things like file type you can enable subject filtering and specify what the subject ends with
- Similarly, you can apply filtering for data stored in specific containers by using the subject begins with filter

## Events at the Resource Group level
- Event subscriptions can also be created on Resource Groups
- These events are for writing, deleting, and actions taken at the Resource Group level

## Custom Topics
[Code here](Code/Visual%20Studio%20Projects/EventGridCustomTopicPublisher/)
1. Custom topics can be created by creating a Event Grid Topic resource
2. Generate a console application as usual in Visual Studio
3. Add the Azure.Messaging.EventGrid dependency
4. Create a Uri with the endpoint of the custom topic that you created for step 1
5. Create an AzureKeyCredential using an access key from the custom topic you created for step 1
6. Create a new POJO style class
7. Create an instance of your class
8. Store it in a List of EventGridEvents
   1. Add a subject, an event type, an event version, and a serialized form of your object to the list
9. Use EventGridPublisherClient to send the list of events
10. Create a subscription on the event and send it somewhere (ideally an Azure Function that you can create directly in the portal, see [Azure Function for logging event data](#azure-function-for-logging-event-data))


## Azure Function for logging event data
This Function can be created in the UI and be used as a target for events to be sent to.
``` C#
#r "Microsoft.Azure.EventGrid"
using Microsoft.Azure.EventGrid.Models;

public static void Run(EventGridEvent eventGridEvent, ILogger log)
{
    log.LogInformation(eventGridEvent.Data.ToString());
    log.LogInformation($"Event type: {eventGridEvent.EventType}");
    log.LogInformation($"Event ID: {eventGridEvent.Id}");
    log.LogInformation($"Event subject: {eventGridEvent.Subject}");
}
```

# Studying from Youtube Event Hubs [Link](https://www.youtube.com/watch?v=HwZldR8KlKM&list=PLLc2nQDXYMHpekgrToMrDpVtFtvmRSqVt&index=18)
##  Creating a new Event Hubs resource (this is a somewhat expensive resource so I would recommend removing it when you're done using it)
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
[Code here](Code/Visual%20Studio%20Projects/EventHub/)
1. Generate a console application as usual in Visual Studio
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
[Code here](Code/Visual%20Studio%20Projects/EventHub/)
1. Using the existing Event Hub add another Shared access policy that allows for listening and store it in a variable in the program
2. To receive events you need to use a consumer group and Event Hubs comes with a default consumer group of $Default that can be used
3. Create an EventHubConsumerClient using the consumer group and connection string from step 1 and 2
4. Receive the events using the client and print out information about them
5. Run the program (and note that it can be run multiple times)
   1. Events are not deleted from Event Hubs as the point is to allow multiple readers to read the data at the same time. Events are instead deleted after the retention period expires.

## Interact with the Event Hub in an Azure Function
[Code here](Code/Visual%20Studio%20Projects/EventHubFunction/)
1. Create a new Azure Function in Visual Studio using the Event Hub trigger
   1. Specify a connection string name and the name of the Event Hub that you created earlier
2. Create a new shared access policy for the function and give it the ability to manage the Event Hub
3. Store the connection string in local.settings.json using the connection string name you configured in 1.1 (note that in the program file the connection string is referenced in the function method)
4. Update the AzureWebJobsStorage setting in local.settings.json to the following: "AzureWebJobsStorage": "UseDevelopmentStorage=true". This will allow you to avoid setting up storage when running locally. Note that this isn't set by default and will cause errors until updated.
5. Run the function and note the events sent earlier.

## Interact with Event Hub using the Event Hub Processor library
[Code here](Code/Visual%20Studio%20Projects/EventHub_Processor/)

The Event Hub Processor is a dependency that gives you the ability to read events and keep track of what your program has read. It requires a storage account to save checkpoints in.

1. Create a new console application in Visual Studio and add the Azure.Messaging.EventHubs.Processor and Azure.Storage.Blobs dependencies to it
2. Create a Storage Account in Azure and create a container named eventhub inside it
3. Store the connection strings for the listen policy from the Event Hub and the connection string for the storage account in the console app
4. Use BlobContainerClient and EventProcessorClient to interact with Azure resources
5. Run the program and note the events that are logged. You can view the container in the storage account to determine the checkpoint that the program got to. Running the program again without sending more events will not log anything.

## Capturing events in Event Hub (note: capture gives extremely pricey estimates even though it's supposed to be free)
1. Navigate to an Event Hub (specifically one using the standard pricing tier) and go to the capture section
2. Enable capture and select a container to store the events in
3. You can browse the files that were captured inside the container

# Udemy Notes (Section 12)
## Azure Event Hubs
- Big data streaming platform
- Can receive and process millions of events per second
- Can stream log data, telemetry data, etc. to Event Hubs
- Event producers can be configured to send events to Event Hubs using various protocols
- Internally Event Hubs uses partitions to increase throughput and provide parallelism
- Event receivers get data from Event Hubs and process the data
- Consumer groups are views of an entire event hub at a point in time

### Concepts
- The consumer app needs to keep running to process events in real time from the Event Hub
- Events don't get deleted after being consumed
  - There isn't a method to delete events from Event Hubs
  - Other consumers may need the events
  - Event Hubs isn't a queue
  - Messages aren't retained indefinitely but are instead governed by the message retention period setting on the Event Hub
    - Data can be sent to a different data store for long term storage
- Consumer programs need to keep track of the events that they've read since by default they will start at the beginning
- Throughput capacity is specified by you
  - Ingress - Up to 1 MB/s or 1000 events per second
  - Egress - Up to 2 MB/s or 4096 events per second
  - Updatable by navigating to the "Scale" blade in the Event Hubs Namespace resource
  - You may receive ServerBusyExceptions when the ingress traffic goes beyond the limit
- Partitions are used for throughput
  - Data is consumed across partitions
  - The number of partitions can't be changed after the Event Hub is created except when using the dedicated cluster and premium tier options
  - Recommended throughput of 1 MB/s per partition
  - You can specify which property of your data can be the partition key. Event Hub will hash the value and map the event to the relevant partition.
- Consumer groups
  - By default there is a $Default consumer group
  - Other groups can be created for other receivers to use
  - Recommended to have one receiver per partition, though you can have 5 readers per partition per consumer group
    - Need to be careful to not duplicate the process of reading the same message
- Some resources in Azure can send events to Event Hubs by specifying it in their "Diagnostic settings" blade (like specifying a Log Analytics Workspace).
- Comparison with Azure Service Bus
  - Service Bus = Message Broker. Event Hubs = Big data ingestion service.

### Creating an Event Hub
1. Search for Event Hubs in the marketplace
2. Give the resource a name
3. Change the location
4. Choose the standard pricing tier (note: you should delete this resource when done)
5. Create the resource
6. Navigate to the resource
7. Click on "+ Event Hub"
8. Give the Event Hub a name
9. Change the partition count to 2
10. Create the Event Hub
11. Navigate to the Event Hub using the "Event Hubs" blade

### Using .NET with Event Hub
#### Sending data
- Code is available in [Code/Visual Studio Projects/UdemyEventHubSender](Code/Visual%20Studio%20Projects/UdemyEventHubSender/).
- Install the Azure.Messaging.EventHubs dependency
- Create a shared access policy (similar to service bus) with the send permission and copy the connection string for your program

#### Receiving data
- Code is available in [Code/Visual Studio Projects/UdemyEventHubReceiver](Code/Visual%20Studio%20Projects/UdemyEventHubReceiver/).
- Install the Azure.Messaging.EventHubs dependency
- Create a shared access policy (similar to service bus) with the listen permission and copy the connection string for your program
- Your program can read data from a particular partition and position using the EventHubConsumerClient.ReadEventsFromPartitionAsync method

#### Event Processor Lab
- Code is available in [Code/Visual Studio Projects/UdemyEventHubProcessor](Code/Visual%20Studio%20Projects/UdemyEventHubProcessor/).
- The Event Processor stores a checkpoint for events that it has read in a storage account, so make one for it to use
- Install the Azure.Messaging.EventHubs.Processor and Azure.Storage.Blobs dependencies
- Use the listen SAS from the receiving data section


## Azure Event Grid
### Event Grid event schema
- The Event Grid event schema is a common format that events can follow. It's JSON data that contains a topic, subject, ID, event type, event time, data version, metadata version, and data unique to the event that's being handled.

### Event Grid Labs
#### Azure Functions - Azure Blob Storage
1. Create a new function app
2. Create a new function that uses the "Azure Event Grid trigger" template
3. Go to a storage account
4. Click the "Events" blade
5. Click "+ Event Subscription"
6. Fill out the requested values
7. Select an Azure Function as your endpoint and choose the function that you just created as the endpoint
8. Click "Create"
9. Navigate to your function
10. View the filesystem logs
11. Navigate to your storage account
12. Upload a blob
13. Verify that information was logged when the blob was uploaded. You can also try deleting the blob.

#### Debugging Azure Event Grid locally
- This can be done using ngrok.

#### Azure Storage Queue as an event consumer
1. Create a new queue in a storage account
2. Create a new event subscription
3. Select a storage queue as the endpoint
4. Create the subscription
5. Verify that messages are received in the queue

#### Custom Topics
1. Create an "Event Grid Topic" resource from the marketplace
2. Navigate to a function app
3. Create an "Azure Event Grid trigger" function
4. Navigate to the topic you created in step 1
5. Click "+ Event Subscription"
6. Give the subscription a name
7. Give a custom event type of "app.neworder"
8. Select your function as the endpoint
9. Create the subscription
10. Open the logs of your function
11. Run the code in [Code/Visual Studio Projects/UdemyPublishEvents](Code/Visual%20Studio%20Projects/UdemyPublishEvents/) to send events to your custom topic.
12. Verify that the function is logging information about the custom events

# When should I use Event Grid or Event Hubs? [MS Link](https://learn.microsoft.com/en-us/azure/event-grid/compare-messaging-services)
- Event Grid is an eventing backplane that gives you the ability to receive and distribute events to registered subscriber endpoints. It allows you to react to changes in your services and applications.
  - Event Grid is not a data pipeline. It delivers events, not the actual object that was updated.
- Event Hubs is a big data streaming platform and event ingestion service. It enables you to process data streams or bundled event batches.
  - It provides a single solution for rapid data retrieval for real-time processing and repeated replay of stored raw data. Data can also be streamed into a file for processing and analysis.

# Misc
- See https://learn.microsoft.com/en-us/shows/exam-readiness-zone/preparing-for-az-204-connect-to-and-consume-azure-services-and-third-party-services-segment-5-of-5 for information on what may appear on the exam.
  - Be comfortable choosing Event Grid, Event Hubs, or Service Bus to solve a problem
    | Service     | Purpose                         | Type                          | When to use                                 |
    | ----------- | ------------------------------- | ----------------------------- | ------------------------------------------- |
    | Event Grid  | Reactive Programming            | Event distribution (discrete) | React to status changes                     |
    | Event Hubs  | Big Data Pipeline               | Event streaming (series)      | Telemetry and distributed data streaming    |
    | Service Bus | High-value enterprise messaging | Message                       | Order processing and financial transactions |
  - Event Grid
    - Comes in to play when a client needs to be connected one on one with another process (ie: a VM getting created inside a resource group could cause an event that Event Grid forwards off to a number of different event handlers)
    - Be comfortable with:
      - Events
      - Event sources
      - Topics
      - Event subscriptions
      - Event handlers
    - When you're thinking about Event Grid on the exam you want to be thinking about distinct, individual occurrences and a series of handlers who might be responding to them
  - Event Hubs
    - Use this when there aren't individual events that are occurring, but thousands or millions of events which need to be ingested into Azure
    - Key scenarios where Event Hubs makes the most sense:
      - You have a large amount of information coming in
      - You need the ability to capture data into blob storage so that it can be processed later
  - Service Bus
    - Service Bus is a more robust option than Storage Queues
    - Understand common messaging scenarios:
      - Messaging: Transfer business data (sales, purchases, journals, inventory, etc)
      - Decouple applications: Improve reliability and scalability of applications
      - Topics and subscriptions: Enable 1:n relationships between publishers and subscribers
      - Message sessions: Implement workflows that require message ordering or message deferral
    - Topics and subscriptions
      - Pub-sub model
      - One sender can create one message that gets delivered to several receivers
  - Storage Queues
    - Simplistic
    - Only supports HTTP
    - Thin layer sitting over the top of the file system
    - A single storage account can have multiple storage queues
    - You'll want to be familiar with the SDKs to communicate with Queue storage
  - Deciding between message queue solutions:
    - Generally go with Service Bus
    - If you have to decide between them for the exam it'll likely come down to features, and really the only feature that can't be done on the Service Bus side is that Storage Queues allow you to store over 80 GB(!!!) of messages in a queue.