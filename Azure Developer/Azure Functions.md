# Azure Functions [MS Documentation on Azure Functions](https://learn.microsoft.com/en-us/azure/azure-functions/functions-overview)
- Azure Functions are serverless apps
- Functions are useful for simple APIs and microservices as well as scheduled tasks
- Three hosting plans are available
  - Consumption plan - Default, autoscaling and pay for what you use.
  - Functions Premium plan - Autoscaling based on demand with pre-warmed workers. More powerful instances and connects to VNets.
  - App service (Dedicated) plan - Runs within an App Service plan at the plan rate. Best for long-running scenarios where you can't use a Durable Function.
- There are two other hosting options which provide the highest amount of control and isolation for your function apps:
  - ASE - App Service Environment (ASE) is an App Service feature that provides a fully isolated and dedicated environment for securely running App Service apps at high scale.
  - Kubernetes - K8s provides a fully isolated and dedicated environment running on top of the k8s platform.
- A function app requires an Azure Storage account with support for Blob, Queue, Files, and Table storage. Function code files are stored on Azure Files shares on the function's main storage account. When you delete the main storage account of the function app, the function code files are deleted and cannot be recovered.

## Common Scenarios [MS Docs](https://learn.microsoft.com/en-us/azure/azure-functions/functions-overview)
- Web APIs: Endpoints can be implemented using the HTTP trigger
- Process file uploads: Code can be run when files are uploaded or changed in blob storage
- Build a serverless workflow: Event-driven workflows can be created from a series of functions using durable functions
- Response to DB changes: Code can be run when a document is created or updated in Cosmos
- Run scheduled tasks: Code can be run on pre-defined timed intervals
- Create reliable message queue systems: Functions can process message queues using queue storage, service bus, or event hubs
- Analyze IoT data streams: Data can be processed from IoT devices
- Process data in real time: Functions and SignalR can respond to data in the moment

## Scale Azure Functions
- In consumption and premium plans Functions scale CPU and memory resources by adding additional instances of the Functions host. The number of instances is based on the number of events that trigger a function.
  - Each instance is limited to 1.5 GB of memory and one CPU
  - An instance of the host is the entire function app, so all functions within the function app share resources within an instance and scale at the same time
  - Function apps that share the same consumption plan scale independently
  - Premium plans determine available memory and CPUs for all apps in the plan on the instance
- A component called the scale controller monitors the rate of events and determines whether to scale out or in.
  - Uses heuristics for each trigger type 
- A single function app only scales out to a maximum of 200 instances (100 for premium plans). Note: not a limit on concurrent executions as a single instance may handle more than one message or request at a time. Functions in an App Service plan can be manually scaled by adding more VM instances or use autoscaling, though this will be slower than the elastic scaling offered by premium plans.
- HTTP triggers can allocate new instances at most once per second. Non-HTTP triggers can allocate new instances at most once every 30 seconds.
- The maximum number of instances can be restricted by modifying the functionAppScaleLimit value (0 or null for unrestricted or a value between 1 and the app maximum).

## Developing Azure Functions
- Functions contain two important pieces: your code and a config (a function.json file). The function.json file defines the trigger, bindings, and other config settings. 
- The bindings property in the functions.json file are used to configure both triggers and bindings. Each binding requires a type, direction, and name.
- Triggers are what cause a function to run. There can only be one. They define how a function is invoked. They can have associated data which can be provided as the payload of the function. A function must have a trigger.
- Bindings are a way of declaratively connecting another resource to the function and are input or output bindings (or both, values could be "in" or "out"). Note that bindings aren't required for a function to run.
- Developing a function app locally requires a number of tools to be installed. The documentation can be found here: https://docs.microsoft.com/en-us/learn/modules/develop-azure-functions/5-create-function-visual-studio-code. The code is available in [Code/Azure Functions](Code/Azure%20Functions/) and uses .NET 6 instead of the version noted in the docs.

### The function.json file
- The function.json file defines the function's trigger, bindings, and other config settings
- The file is generated automatically from annotations in code for compiled languages. Scripting languages do not have this benefit and the dev will need to provide this file.
- The runtime uses this file to determine the events to monitor, how to pass data to the function, and how to return data from a function execution.
- Bindings require the following properties:
  - type: A string representing the type of the binding (example: httpTrigger, table, etc.)
  - direction: A string indicating whether the binding is for receiving data into the function or returning data from the function (in or out are examples of direction)
  - name: A string that is used for the bound data in the function
- Example file:
``` json
{
    "disabled":false,
    "bindings":[
        // ... bindings here
        {
            "type": "bindingType",
            "direction": "in",
            "name": "myParamName",
            // ... more depending on binding
        }
    ]
}
```

### Triggers and Bindings
- Triggers are what cause a function to run
- A trigger defines how a function is invoked
- There can only be one trigger for a function
- Triggers have associated data (often provided as the payload of the function)
- Bindings are a way of declaratively connecting another resource to the function
- There can be input bindings, output bindings, or both
- Data from bindings is provided to the function as parameters
- Example function.json file for a function which writes a row to Azure Table storage whenever a message appears in Azure Queue storage:
``` json
{
  "bindings": [
    {
      "type": "queueTrigger",
      "direction": "in",
      "name": "order",
      "queueName": "myqueue-items",
      "connection": "MY_STORAGE_ACCT_APP_SETTING"
    },
    {
      "type": "table",
      "direction": "out",
      "name": "$return",
      "tableName": "outTable",
      "connection": "MY_TABLE_STORAGE_ACCT_APP_SETTING"
    }
  ]
}
```

### Common types of triggers/bindings
#### HTTP Trigger
- The trigger is defined by the HttpTrigger attribute in the method signature below.
- The output binding is defined by returning an IActionResult or Task`<IActionResult>`. An output isn't required.
``` c#
[FunctionName("HttpTriggerCSharp")]
public static async Task<IActionResult> Run(
    [HttpTrigger(AuthorizationLevel.Function, "get", "post", Route = null)] // Attribute defining the trigger
    HttpRequest req, ILogger log)
{
    // logic above
    return name != null
        ? (ActionResult)new OkObjectResult($"Hello, {name}")
        : new BadRequestObjectResult("Please pass a name on the query string or in the request body");
}
```

#### Timer Trigger 
- The timer trigger is defined by the TimerTrigger attribute. It uses a format similar to CRON to evaluate when the function should run. The format follows this pattern: {second} {minute} {hour} {day} {month} {day-of-week}.
- The example below runs every five minutes each hour.
``` c#
[FunctionName("TimerTriggerCSharp")]
public static void Run([TimerTrigger("0 */5 * * * *")]TimerInfo myTimer, ILogger log)
{
    if (myTimer.IsPastDue)
    {
        log.LogInformation("Timer is running late!");
    }
    log.LogInformation($"C# Timer trigger function executed at: {DateTime.Now}");
}
```

#### Blob Trigger
- The blob trigger is defined by the BlobTrigger attribute. The {name} portion of the expression creates a binding expression which will be made available as part of the name parameter in the method.
``` c#
[FunctionName("BlobTriggerCSharp")]        
public static void Run([BlobTrigger("samples-workitems/{name}")] Stream myBlob, string name, ILogger log)
{
    log.LogInformation($"C# Blob trigger function Processed blob\n Name:{name} \n Size: {myBlob.Length} Bytes");
}
```

#### Service Bus Trigger
- The Service Bus trigger is defined by the ServiceBusTrigger attribute. It expects the name of a queue or topic as well as a Connection property. The Connection property is a string value that's set to the name of an app setting or setting collection that specifies how to connect to Service Bus.
``` c#
[FunctionName("ServiceBusQueueTriggerCSharp")]                    
public static void Run(
    [ServiceBusTrigger("myqueue", Connection = "ServiceBusConnection")] 
    string myQueueItem,
    Int32 deliveryCount,
    DateTime enqueuedTimeUtc,
    string messageId,
    ILogger log)
{
    log.LogInformation($"C# ServiceBus queue trigger function processed message: {myQueueItem}");
    log.LogInformation($"EnqueuedTimeUtc={enqueuedTimeUtc}");
    log.LogInformation($"DeliveryCount={deliveryCount}");
    log.LogInformation($"MessageId={messageId}");
}
```

## Durable Functions
- Durable Functions are an extension of Azure Functions that allow for stateful functions.
  - Stateful workflows are supported by orchestrator functions
  - Stateful entities are supported by entity functions
- Used for the following patterns (see [this link](https://learn.microsoft.com/en-us/training/modules/implement-durable-functions/2-durable-functions-overview) for code examples of these patterns)
  - Function chaining - Sequence of functions which execute in a specific order with the output of one function being supplied as the input of another function.
    ``` c#
    [FunctionName("Chaining")]
    public static async Task<object> Run(
        [OrchestrationTrigger] IDurableOrchestrationContext context)
    {
        try
        {
            var x = await context.CallActivityAsync<object>("F1", null);
            var y = await context.CallActivityAsync<object>("F2", x);
            var z = await context.CallActivityAsync<object>("F3", y);
            return  await context.CallActivityAsync<object>("F4", z);
        }
        catch (Exception)
        {
            // Error handling or compensation goes here.
        }
    }
    ```
    - In the above example F1, F2, F3, and F4 are the names of other functions in the function app. 
  - Fan out/fan in - Execute multiple functions in parallel and then wait for them to finish.
    ``` c#
    [FunctionName("FanOutFanIn")]
    public static async Task Run(
        [OrchestrationTrigger] IDurableOrchestrationContext context)
    {
        var parallelTasks = new List<Task<int>>();

        // Get a list of N work items to process in parallel.
        object[] workBatch = await context.CallActivityAsync<object[]>("F1", null);
        for (int i = 0; i < workBatch.Length; i++)
        {
            Task<int> task = context.CallActivityAsync<int>("F2", workBatch[i]);
            parallelTasks.Add(task);
        }

        await Task.WhenAll(parallelTasks);

        // Aggregate all N outputs and send the result to F3.
        int sum = parallelTasks.Sum(t => t.Result);
        await context.CallActivityAsync("F3", sum);
    }
    ```
    - In the above example work is fanned out to multiple instances of F2 and then passed to F3 when all the functions finish running.
  - Async HTTP APIs - Used to kick off a long running operation and redirect the client to a status endpoint. There is built-in support for this.
    ``` c#
    public static class HttpStart
    {
        [FunctionName("HttpStart")]
        public static async Task<HttpResponseMessage> Run(
            [HttpTrigger(AuthorizationLevel.Function, methods: "post", Route = "orchestrators/{functionName}")] HttpRequestMessage req,
            [DurableClient] IDurableClient starter,
            string functionName,
            ILogger log)
        {
            // Function input comes from the request content.
            object eventData = await req.Content.ReadAsAsync<object>();
            string instanceId = await starter.StartNewAsync(functionName, eventData);

            log.LogInformation($"Started orchestration with ID = '{instanceId}'.");

            return starter.CreateCheckStatusResponse(req, instanceId);
        }
    }
    ```
    - An HttpTrigger (or others like queues or Event Hubs) can be used to manage long-running orchestrations if you don't want to use the built-in version. See the example above.
  - Monitor - Recurring process in a workflow (example: polling until specific conditions are met).
    ``` c#
    [FunctionName("MonitorJobStatus")]
    public static async Task Run(
        [OrchestrationTrigger] IDurableOrchestrationContext context)
    {
        int jobId = context.GetInput<int>();
        int pollingInterval = GetPollingInterval();
        DateTime expiryTime = GetExpiryTime();

        while (context.CurrentUtcDateTime < expiryTime)
        {
            var jobStatus = await context.CallActivityAsync<string>("GetJobStatus", jobId);
            if (jobStatus == "Completed")
            {
                // Perform an action when a condition is met.
                await context.CallActivityAsync("SendAlert", machineId);
                break;
            }

            // Orchestration sleeps until this time.
            var nextCheck = context.CurrentUtcDateTime.AddSeconds(pollingInterval);
            await context.CreateTimer(nextCheck, CancellationToken.None);
        }

        // Perform more work here, or let the orchestration end.
    }
    ```
  - Human interaction - Orchestrator functions can be used to wait for human input or to perform some other action after a timeout.
    ``` c#
    [FunctionName("ApprovalWorkflow")]
    public static async Task Run(
        [OrchestrationTrigger] IDurableOrchestrationContext context)
    {
        await context.CallActivityAsync("RequestApproval", null);
        using (var timeoutCts = new CancellationTokenSource())
        {
            DateTime dueTime = context.CurrentUtcDateTime.AddHours(72);
            Task durableTimeout = context.CreateTimer(dueTime, timeoutCts.Token);

            Task<bool> approvalEvent = context.WaitForExternalEvent<bool>("ApprovalEvent");
            if (approvalEvent == await Task.WhenAny(approvalEvent, durableTimeout))
            {
                timeoutCts.Cancel();
                await context.CallActivityAsync("ProcessApproval", approvalEvent.Result);
            }
            else
            {
                await context.CallActivityAsync("Escalate", null);
            }
        }
    }
    ```
- Durable function types:
  - Orchestrator - Functions which describe how actions are executed and their order. Can include other functions, sub-orchestrations, events, HTTP, and timers. The function must be deterministic.
  - Activity - Basic unit for work in orchestration. Can return data back to an orchestrator function.
  - Entity - Operations for updating small pieces of data. Stateful entities are durable entities. Accessed via a unique ID (entity ID) which is also required for operations on the entity.
  - Client - Primary way to deliver messages to an orchestrator function.
- Task hubs are logical containers for durable storage resources that are used for orchestrations and entities.
  - Orchestrator, activity, and entity functions can only directly interact with each other when they belong to the same task hub.
  - A single storage account can contain multiple task hubs; however, each function app must be configured with a separate task hub name.
  - Task hubs consist of the following resources which are created automatically in the storage account when orchestrator, entity, or activity functions run or are scheduled to run
    - 1+ control queues
    - 1 work-item queue
    - 1 history table
    - 1 instances table
    - 1 storage container containing 1+ lease blobs
    - 1 storage container containing large message payloads (if needed)

### Durable Orchestrations
- Orchestrator functions use procedural code
- Output from called functions can be saved to local variables in an orchestrator function. The orchestrator function can call other durable functions synchronously and asynchronously.
- Progress is checkpointed (when the function "awaits" or "yields") and not lost on a recycle or VM reboot
  - This is maintained by using the event sourcing design pattern to store the series of actions the function takes in an append-only store
  - In C# the await (in JS it's yield) operator yields control back to the Durable Task Framework dispatcher. The dispatcher then commits any new actions that the orchestrator function scheduled to storage. Afterwards the orchestrator function can be unloaded from memory.
- When given more work the orchestrator wakes up and re-executes the entire function from the start to rebuild the local state
- Can be long-running
- Has an instance ID (which is a guid)
- Orchestrator function patterns:
  - Sub-orchestrations - Orchestrator function which calls other orchestrator functions
  - Durable timers - Delays or timeout handling on async actions
    - These should be used over sleeps when writing code
  - External events - Can await external events
    - Often used for human interaction
  - Error handling - Can use error handling features from the programming language
  - Critical sections - Orchestration instances are single threaded, but there can be race conditions when interacting with external systems. Critical sections can be defined programmatically.
  - Calling HTTP endpoints - Use activity functions to make HTTP calls and return the result to the orchestration
  - Passing multiple parameters
- Durable Functions provide durable timers that should be used instead of putting threads to sleep.
- Orchestrator functions have the ability to wait and listen for external events.

## Durable Functions Deep Dive [MS Documentation with code examples!](https://learn.microsoft.com/en-us/azure/azure-functions/durable/durable-functions-overview?tabs=csharp)

### Function Types
A durable function is made of various functions: activity, orchestrator, entity, and client.
- Orchestrator
  - Orchestrator functions describe how actions are executed and the order in which actions are executed
  - Orchestrator code must be deterministic, otherwise the function can fail to run correctly
  - See https://learn.microsoft.com/en-us/azure/azure-functions/durable/durable-functions-orchestrations?tabs=csharp for more information
- Activity
  - Activity functions are the basic unit of work in a durable function orchestration
  - Unlike orchestrator functions activity functions aren't restricted in the type of work they can do
  - Frequently used to make network calls or run CPU intensive operations
  - May return data back to the orchestrator function
  - The Durable Task Framework guarantees that each called activity function will be executed at least once during an orchestration's execution
- Entity
  - Entity functions define operations for reading and updating small pieces of state
  - Often referred to as durable entities
  - Entity functions are functions with a special trigger type: entity trigger
  - Can be invoked from client functions or from orchestrator functions
  - No specific code constraints
- Client
  - A client function is any non-orchestrator function
  - A client function is made a client function because of the binding that it uses: a durable client output binding
  - Client functions are a way to deliver messages into a task hub (which are used to trigger orchestrator and entity functions)
  - Client functions can be used to interact with running orchestrations and entities (you could query them, terminate them, or have events raised to them)


# Studying from Youtube [video here](https://www.youtube.com/watch?v=Mo8dYQBx5ic&list=PLLc2nQDXYMHpekgrToMrDpVtFtvmRSqVt&index=5)
- I added a few functions to [Code/Azure Functions](Code/Azure%20Functions/) which connect to a SQL database and retrieve or add data.
- I updated my [Code/Azure App Service](Code/Azure%20App%20Service/) code to call a function to retrieve values from the database.


# Studying from Udemy
- Code is located in [Code/Visual Studio Projects/UdemyAzureFunction](Code/Visual%20Studio%20Projects/UdemyAzureFunction/)
- Note that Azure Functions that need to be deployed that don't use a custom language must have a publish type of code and use a custom handler (see https://docs.microsoft.com/en-us/azure/azure-functions/functions-custom-handlers).

# Studying from Udemy (Section 12)
## Copying blobs using a blob trigger
- Code is located in [Code/Visual Studio Projects/UdemyCopyBlobFunction](Code/Visual%20Studio%20Projects/UdemyCopyBlobFunction/).
- This is an Azure Function that uses a blob trigger. It expects two containers in a storage account: one named data and another named newdata.
- This function needs version 5.0.1 of the Microsoft.Azure.WebJobs.Extensions.Storage dependency.

## Azure Function Bindings for Queue Storage [MS Link](https://learn.microsoft.com/en-us/azure/azure-functions/functions-bindings-storage-queue-trigger?tabs=in-process%2Cextensionv5&pivots=programming-language-csharp)
- The queue storage trigger runs a function as messages are added to Azure Queue storage.
- The queue message is provided as input to the function.
- When there are multiple queue messages waiting, the queue trigger retrieves a batch of messages and invokes function instances concurrently to process them.
  - Default batch size: 16 (this is configurable in the host.json file)
  - When there are 8 messages remaining the runtime will get another batch of messages, so the the maximum amount being processed concurrently per function on one VM will be 24. If your function scales to multiple VMs then this limit will be increased proportionally.

# Misc
- See https://learn.microsoft.com/en-us/shows/exam-readiness-zone/preparing-for-az-204-develop-azure-compute-solutions-1-of-5 for information on what may appear on the exam.
  - Be familiar with triggers and bindings
    - Creation with JSON or annotations in code
    - Know about the function.json file
      - Know the difference between directly editing files in the portal vs doing it locally and publishing it
  - Be familiar with classes and interfaces you'll be using
    - It's not going to be about syntax, but you should know this and some of the larger methods
  - Be familiar with the Durable Functions SDK
    - Understand the patterns for using Durable Functions