# Azure Message Queues
Azure has two types of queues
- Service Bus queues - Part of the broader messaging infrastructure that supports queuing, pub/sub, and more advanced integration patterns. Designed for apps that may use multiple communication protocols, data contracts, trust domains, or network environments. Service Bus should be used in the following situations:
  - Your solution needs to receive messages without having to poll the queue. With Service Bus, you can achieve it by using a long-polling receive operation using the TCP-based protocols that Service Bus supports.
  - Your solution requires the queue to provide a guaranteed first-in-first-out (FIFO) ordered delivery.
  - Your solution needs to support automatic duplicate detection.
  - You want your application to process messages as parallel long-running streams (messages are associated with a stream using the session ID property on the message). In this model, each node in the consuming application competes for streams, as opposed to messages. When a stream is given to a consuming node, the node can examine the state of the application stream state using transactions.
  - Your solution requires transactional behavior and atomicity when sending or receiving multiple messages from a queue.
  - Your application handles messages that can exceed 64 KB but won't likely approach the 256-KB limit.
- Storage queues - Part of the Azure Storage infrastructure and used to store a large number of messages. The messages can be accessed using HTTP or HTTPs. Messages are up to 64 KB in size and can contain messages up to the total capacity limit of the storage account. Commonly used to create a backlog of work to process asynchronously. Storage queues should be used in the following situations:
  - Your application must store over 80 gigabytes of messages in a queue.
  - Your application wants to track progress for processing a message in the queue. It's useful if the worker processing a message crashes. Another worker can then use that information to continue from where the prior worker left off.
  - You require server side logs of all of the transactions executed against your queues.

## Azure Service Bus
- Fully managed enterprise integration message broker.
- Data is transferred using messages (container decorated with metadata and contains data).

Common messaging scenarios
- Messaging - Transfer business data
- Decouple applications - Improves reliability/scalability as client and service don't have to be online at the same time
- Topics and subscriptions - Enable 1:n relationships between publishers and subscribers
- Message sessions - Implement workflows that require message ordering or message deferral

### Service Bus Tiers
Service Bus offers two tiers (the feature sets of the two tiers are nearly identical)
- Standard - Has variable throughput, variable latency, pay as you go variable pricing, no ability to scale up or down, and messages up to 256 KB in size.
- Premium - Recommended for production. Supports scale, performance, and availability. Has high throughput, predictable performance, fixed pricing, the ability to scale up and down, and messages up to 1 MB in size (with 100 MB in preview).

### Advanced Features
| Feature               | Description                                                                                                                                                                                                                                                 |
| --------------------- | ----------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------- |
| Message sessions      | To create a first-in, first-out (FIFO) guarantee in Service Bus, use sessions. Message sessions enable exclusive, ordered handling of unbounded sequences of related messages.                                                                              |
| Autoforwarding        | The autoforwarding feature chains a queue or subscription to another queue or topic that is in the same namespace.                                                                                                                                          |
| Dead-letter queue     | Service Bus supports a dead-letter queue (DLQ). A DLQ holds messages that can't be delivered to any receiver. Service Bus lets you remove messages from the DLQ and inspect them.                                                                           |
| Scheduled delivery    | You can submit messages to a queue or topic for delayed processing. You can schedule a job to become available for processing by a system at a certain time.                                                                                                |
| Message deferral      | A queue or subscription client can defer retrieval of a message until a later time. The message remains in the queue or subscription, but it's set aside.                                                                                                   |
| Batching              | Client-side batching enables a queue or topic client to delay sending a message for a certain period of time.                                                                                                                                               |
| Transactions          | A transaction groups two or more operations together into an execution scope. Service Bus supports grouping operations against a single messaging entity within the scope of a single transaction. A message entity can be a queue, topic, or subscription. |
| Filtering and actions | Subscribers can define which messages they want to receive from a topic. These messages are specified in the form of one or more named subscription rules.                                                                                                  |
| Autodelete on idle    | Autodelete on idle enables you to specify an idle interval after which a queue is automatically deleted. The minimum duration is 5 minutes.                                                                                                                 |
| Duplicate detection   | An error could cause the client to have a doubt about the outcome of a send operation. Duplicate detection enables the sender to resend the same message, or for the queue or topic to discard any duplicate copies.                                        |
| Security protocols    | Service Bus supports security protocols such as Shared Access Signatures (SAS), Role Based Access Control (RBAC) and Managed identities for Azure resources.                                                                                                |
| Geo-disaster recovery | When Azure regions or datacenters experience downtime, Geo-disaster recovery enables data processing to continue operating in a different region or datacenter.                                                                                             |
| Security              | Service Bus supports standard AMQP 1.0 and HTTP/REST protocols.                                                                                                                                                                                             |

### Standards and Protocols
Service Bus uses Advanced Messaging Queueing Protocol (AMQP) 1.0 which allows users to write apps that work against Service Bus and on-prem brokers like ActiveMQ or RabbitMQ. Service Bus Premium is fully compliant with the JMS 2.0 API.

### Queues, Topics, and Subscriptions
Queues
- FIFO message delivery.
- Load-leveling - producers and consumers send and receive messages at different rates. Allows consumers to only handle average load instead of peak load.
- Can be used to provide loose coupling between components as producers and consumers aren't aware of each other.
- Supports two different modes for receiving messages
  - Receive and delete - Service Bus receives a request from the consumer -> marks the message as being consumed -> returns it to the consumer. Works best for apps that can tolerate not processing a message if a failure occurs.
  - Peek lock - Two-stage receive operation for apps that can't miss messages: The message to be consumed is found and locked so other consumers can't receive it and then returned to the app. After the app is done with the message it requests the Service Bus to complete the second stage of the receive process, then the message is marked as consumed. If apps can't process a message it can request the message be abandoned which will unlock the message and allow it to be received again. The lock also has a timeout which will cause the message to be unlocked.

Topics and subscriptions
- Topics and subscriptions provide a one to many form of communication in a pub/sub pattern
- Useful for scaling to large numbers of recipients
- Publishers send messages to a topic and one or more subscribers receive a copy of the message depending on their filtering rules
- Consumers don't receive messages directly from the topic. They receive them from the topic subscription which resembles a virtual queue.
- Subscriptions can be configured to filter messages and modify their properties

### Service Bus message payloads and serialization
It may just be better to read the documentation on MS Learn for this: https://docs.microsoft.com/en-us/learn/modules/discover-azure-message-queue/5-messages-payloads-serialization.

### Interacting with Service Bus with C# (Documentation [here](https://docs.microsoft.com/en-us/learn/modules/discover-azure-message-queue/6-send-receive-messages-service-bus))
Create Azure resources using the following:
``` bash
myLocation=eastus
myNameSpaceName=az204svcbus$RANDOM

az group create --name az204-svcbus-patrick-rg --location $myLocation

az servicebus namespace create \
    --resource-group az204-svcbus-patrick-rg \
    --name $myNameSpaceName \
    --location $myLocation

az servicebus queue create --resource-group az204-svcbus-patrick-rg \
    --namespace-name $myNameSpaceName \
    --name az204-queue    
```
Navigate to the service bus in the Azure portal, select Shared access policies, select the RootManageSharedAccessKey policy, and then copy the Primary Connection String for use in your code.

The C# code for this can be found in [here](Code/Azure%20Service%20Bus/)

After finishing you can delete the resource group.
``` bash
az group delete --name az204-svcbus-patrick-rg --no-wait
```

## Azure Queue Storage
Queues contain the following components
- URL format: https://\<storage account\>.queue.core.windows.net/\<queue\>
- Storage account - All access to Azure Storage is done through a storage account
- Queue - A queue contains a set of messages. Note that the queue name must be lowercase.
- Message - A message, in any format, up to 64 KB. Can include a TTL of any positive number, -1 to indicate it doesn't expire, or be omitted to default to to 7 days.

### Interacting with Azure Queue Storage with C# (TODO)



# Studying from Youtube Service Bus [Link](https://www.youtube.com/watch?v=LbzZLkmO_WE&list=PLLc2nQDXYMHpekgrToMrDpVtFtvmRSqVt&index=16)
Create a standard tier Service Bus namespace so that topics and queues can be created. Also, Service Bus is similar to Event Hub but the messages are deleted after being received. Basically think of JMS.

## Using Queues from the Azure Portal
1. Create a queue using the Azure portal or the CLI inside your Service Bus namespace
2. Navigate to the queue
3. Click on Service Bus Explorer (preview feature, may or may not be on exam)
4. Use this tool to send, receive, and peek at messages

## Using Queues with C#
[Code here](Code/Visual%20Studio%20Projects/ServiceBusQueues/)
1. Create a new console application in Visual Studio as normal
2. Add the Azure.Messaging.ServiceBus dependency
3. Create a ServiceBusClient using connection strings retrieved from shared access policies that allow you to send and receive messages
4. Use ServiceBusSender to send messages and ServiceBusReceiver to receive them
   1. Note that ServiceBusSender has a constructor which takes ServiceBusReceiverOptions as a parameter. This parameter allows you to specify if the receiver should peek at messages or if it should receive and delete them.

# Udemy Notes (Section 12)
## Storage Queue Lab
- The code to interact with a storage queue from a console app is available in [Code/Visual Studio Projects/UdemyQueueStorage](Code/Visual%20Studio%20Projects/UdemyQueueStorage/).
  - Useful info:
    - Messages received from the queue must be deleted or they will be added back to the queue after 30 seconds.
- The code to interact with the queue from an Azure Function is available in [Code/Visual Studio Projects/UdemyQueueFunction](Code/Visual%20Studio%20Projects/UdemyQueueFunction/).
  - Useful info:
    - Messages need to be base 64 encoded. If they aren't then the function will try to dequeue the message over and over before putting the messages into a new queue named <queue name>-poison.
    - The function can receive messages as objects instead of strings.
- Both of these programs expect a queue named "appqueue" to be available.