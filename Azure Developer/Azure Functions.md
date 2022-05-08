# Azure Functions
- Azure Functions are serverless apps
- Functions are useful for simple APIs and microservices as well as scheduled tasks
- Three hosting plans are available
  - Consumption plan - Default, autoscaling and pay for what you use.
  - Functions Premium plan - Autoscaling based on demand with pre-warmed workers. More powerful instances and connects to VNets.
  - App service (Dedicated) plan - Runs within an App Service plan at the plan rate. Best for long-running scenarios where you can't use a Durable Function.
- A function app requires an Azure Storage account with support for Blob, Queue, Files, and Table storage.
- A component called the scale controller monitors the rate of events and determines whether to scale out or in.

## Developing Azure Functions (Todo: Add notes on setting up a Function locally)
- Functions contain two important pieces: your code and a config (a function.json file). The function.json file defines the trigger, bindings, and other config settings. 
- The bindings property in the functions.json file are used to configure both triggers and bindings. Each binding requires a type, direction, and name.
- Triggers are what cause a function to run. There can only be one. They define how a function is invoked. They can have associated data which can be provided as the payload of the function.
- Bindings are a way of declaratively connecting another resource to the function and are input or output bindings (or both).
- Developing a function app locally requires a number of tools to be installed. The documentation can be found here: https://docs.microsoft.com/en-us/learn/modules/develop-azure-functions/5-create-function-visual-studio-code. The code is available in [Code/Azure Functions](Code/Azure%20Functions/) and uses .NET 6 instead of the version noted in the docs.

## Durable Functions
- Durable Functions are an extension of Azure Functions that allow for stateful functions.
  - Stateful workflows are supported by orchestrator functions
  - Stateful entities are supported by entity functions
- Used for the following patterns
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

### Durable Orchestrations
- Orchestrator functions use procedural code
- Output from called functions can be saved to local variables
- Progress is checkpointed and not lost on a recycle or VM reboot
  - This is maintained by using the event sourcing design pattern to store the series of actions the function takes
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


# Studying from Youtube [video here](https://www.youtube.com/watch?v=Mo8dYQBx5ic&list=PLLc2nQDXYMHpekgrToMrDpVtFtvmRSqVt&index=5)
- I added a few functions to [Code/Azure Functions](Code/Azure%20Functions/) which connect to a SQL database and retrieve or add data.
- I updated my [Code/Azure App Service](Code/Azure%20App%20Service/) code to call a function to retrieve values from the database.