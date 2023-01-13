# Azure Functions [MS Documentation on Azure Functions](https://learn.microsoft.com/en-us/azure/azure-functions/functions-overview)
- Azure Functions are serverless apps
- Functions are useful for simple APIs and microservices as well as scheduled tasks
- Three hosting plans are available
  - Consumption plan - Default, autoscaling and pay for what you use.
  - Functions Premium plan - Autoscaling based on demand with pre-warmed workers. More powerful instances and connects to VNets.
  - App service (Dedicated) plan - Runs within an App Service plan at the plan rate. Best for long-running scenarios where you can't use a Durable Function.
- A function app requires an Azure Storage account with support for Blob, Queue, Files, and Table storage. Function code files are stored on Azure Files shares on the function's main storage account. When you delete the main storage account of the function app, the function code files are deleted and cannot be recovered.
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

## Developing Azure Functions (Todo: Add notes on setting up a Function locally)
- Functions contain two important pieces: your code and a config (a function.json file). The function.json file defines the trigger, bindings, and other config settings. 
- The bindings property in the functions.json file are used to configure both triggers and bindings. Each binding requires a type, direction, and name.
- Triggers are what cause a function to run. There can only be one. They define how a function is invoked. They can have associated data which can be provided as the payload of the function. A function must have a trigger.
- Bindings are a way of declaratively connecting another resource to the function and are input or output bindings (or both, values could be "in" or "out"). Note that bindings aren't required for a function to run.
- Developing a function app locally requires a number of tools to be installed. The documentation can be found here: https://docs.microsoft.com/en-us/learn/modules/develop-azure-functions/5-create-function-visual-studio-code. The code is available in [Code/Azure Functions](Code/Azure%20Functions/) and uses .NET 6 instead of the version noted in the docs.

## Durable Functions
- Durable Functions are an extension of Azure Functions that allow for stateful functions.
  - Stateful workflows are supported by orchestrator functions
  - Stateful entities are supported by entity functions
- Used for the following patterns (see [this link](https://learn.microsoft.com/en-us/training/modules/implement-durable-functions/2-durable-functions-overview) for code examples of these patterns)
  - Function chaining - Sequence of functions which execute in a specific order with the output of one function being supplied as the input of another function.
  - Fan out/fan in - Execute multiple functions in parallel and then wait for them to finish.
  - Async HTTP APIs - Used to kick off a long running operation and redirect the client to a status endpoint. There is built-in support for this.
  - Monitor - Recurring process in a workflow (example: polling until specific conditions are met).
  - Human interaction - Orchestrator functions can be used to wait for human input or to perform some other action after a timeout.
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


# Studying from Youtube [video here](https://www.youtube.com/watch?v=Mo8dYQBx5ic&list=PLLc2nQDXYMHpekgrToMrDpVtFtvmRSqVt&index=5)
- I added a few functions to [Code/Azure Functions](Code/Azure%20Functions/) which connect to a SQL database and retrieve or add data.
- I updated my [Code/Azure App Service](Code/Azure%20App%20Service/) code to call a function to retrieve values from the database.


# Studying from Udemy
- Code is located in [Code/Visual Studio Projects/UdemyAzureFunction](Code/Visual%20Studio%20Projects/UdemyAzureFunction/)
- Note that Azure Functions that need to be deployed that don't use a custom language must have a publish type of code and use a custom handler (see  https://docs.microsoft.com/en-us/azure/azure-functions/functions-custom-handlers).

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