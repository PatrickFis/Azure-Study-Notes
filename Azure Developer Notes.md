# Azure App Service
- Service for hosting web apps, REST APIs, mobile back ends.
- Supports lots of languages
- Has built in support to scale up/down or out/in. Can scale CPU cores and RAM as well as machine instances running app.

## Azure App Service Plans
- A plan defines a set of compute resources for an app to run in. One or more apps can use the same resources/plan. Azure Functions can also run in a plan.
- Plans define the region, amount and size of VMs, and the pricing tier.
- Pricing tier categories
  - Shared compute - Free + Shared share resource pools with other customers.
  - Dedicated compute - Basic, Standard, Premium, PremiumV2, and PremiumV3 run on dedicated VMs. Higher tiers = more VMs to scale out.
  - Isolated - Runs on a dedicated VNet.
  - Consumption - Only available to function apps. Scales dynamically depending on workload.

## Authentication & Authorization
- App Service supports a number of identity providers: Microsoft Identity Platform, Facebook, Google, Twitter, and any OpenID Connect provider.
- The authN + authZ module runs alongside your app code. When enabled HTTP requests pass through it before being handled by your app. It'll handle the following for you:
  - Authenticate users
  - Validates, store, and refresh tokens
  - Manage the authenticated session
  - Inject identity information into request headers
- Supports authentication flows with or without a provider SDK.
- Allows you to specify behavior for unauthenticated requests: they can either be allowed or require authentication (which may not be desirable for SPAs).

## Networking
- App Service is a system where roles that handle HTTP(S) traffic is called front ends while roles hosting customer workloads are called workers.
- Roles exist in a multi-tenant network.
- There are various features used to handle application communication. These features differ between inbound and outbound calls, as each feature cannot be used to handle both types of connections.
- Free and Shared SKUs host workers on multitenant workers. Basic and higher hosts are dedicated to one App Service plan.
- The type of worker VM you use (which is dependent on your plan) determines what set of outbound addresses you receive.

Networking commands:
``` shell
# Show outbound IPs used by your app in the Additional Outbound IP Addresses field. 
az webapp show \
    --resource-group <group_name> \
    --name <app_name> \ 
    --query outboundIpAddresses \
    --output tsv

# Find all possible outbound IP addresses for your app
az webapp show \
    --resource-group <group_name> \ 
    --name <app_name> \ 
    --query possibleOutboundIpAddresses \
    --output tsv
```

## Creating a simple HTML web app using Cloud Shell
1. Login to the Azure portal
2. Open the Cloud Shell
3. Create a directory and navigate inside it
``` shell
mkdir htmlapp
cd htmlapp
```
4. Clone this git repo insode the directory: git clone https://github.com/Azure-Samples/html-docs-hello-world.git
5. CD into the cloned directory
6. Run the az webapp up command to set up the application
``` shell
az webapp up --location eastus --name patrickHtmlTestApp419 --html
```
7. The command will output JSON data and a URL for the web app. Navigate to the web app to check that it's up.
8. Modify the app by making a change to index.html (code index.html will open an editor, ctrl+s to save, ctrl+q to quit).
9. Run the command from step 6 again to redeploy the app.
10. Refresh the page in your browser.
11. Clean up the resources when you're done: az group delete --name <resource_group> --no-wait


## Configuring App Settings
- App Service passes app settings to the container using the --env flag to set environment variables in the container. These settings can be accessed through the Azure portal by navigating to your App Service -> Selecting Configuration -> Application Settings.
- App settings can be added one at a time as a key-value pair. They can also be edited in bulk by clicking on the advanced edit button on the application settings page (which will show the settings in a JSON format).
- Connection strings can be added as app settings. They are encrypted at rest and use specific formatting (see https://docs.microsoft.com/en-us/learn/modules/configure-web-app-settings/2-configure-application-settings).
- App Services have general settings under Configuration -> General settings. These include the following:
  - Stack settings - Software used to run the app
  - Platform settings - Hosting platform config
    - Bitness - 32/64 bit
    - WebSocket protocol
    - Always On - Keeys the app loaded even without traffic. By default apps are unloaded after 20 minutes of no incoming requests
    - Managed pipeline version - IIS pipeline mode
    - HTTP version
    - ARR affinity - Used in multi-instance deployments to ensure clients are routed to same instance throughout their session
  - Debugging - Remote debugging for ASP.NET, ASP.NET Core, or Node.js apps. Turns itself off after 48 hours.
  - Incoming client certificates - Requires client certs in mutual authN.
- Handler mapping and virtual application/directory mappings can be managed through Configuration -> Path mappings. These settings allow for custom script processors to be used to handle requests to specific file extensions.
- App service logs can be enabled through the Azure portal in the App Service logs section. The following log options are available:
  - Application logging - Available on Windows and Linux. Logs messages generated by application code using a logging pattern standard to your language.
    - Enabled by setting Application Logging (Filesystem or Blob for Windows and Filesystem for Linux) to on. On Windows Filesystem logging disables itself after 12 hours while blob is for long term storage. On Linux logs are given a quota and retained for a set number of days.
  - Web server logging - Available on Windows. Logs raw HTTP request data including the HTTP method, resource URI, client IP, client port, user agent, response code, etc.
  - Detailed error logging - Available on Windows. Copies of error pages that were sent to the client browser.
  - Failed request tracing - Available on Windows. Trace info on failed requests. One folder is generated for each request containing a XML log file and XSL stylesheet to view the log file with.
  - Deployment logging - Available on Windows and Linux. Helps determine why a deployment failed.
- Apps support certificates that can be managed through Azure or Azure Key Vault. Apps can enforce HTTPS by updating TLS/SSL settings and changing HTTPS Only to on.
- Apps support feature flags through app configuration. Recommendation is to externalize the feature flags to a separate repo so that you can modify them without redeploying the application itself. Note that App Configuration is a separate Azure resource.

## Scaling Apps
- Autoscaling is a process which adjusts available resources based on demand (or on a schedule). It scales in and out (not up and down) by adding and removing web servers.
- Autoscaling is based on rules that specify thresholds to trigger events. Note that this could cause issues in the event of a DoS.
  - The hardware used for autoscaling is defined in the plan.
  - A plan has an instance limit that cannot be crossed to prevent runaway autoscaling.
  - There are two options for autoscaling: based on a metric (like HTTP requests) or on a schedule. The available metrics:
    - CPU percentage across all instances
    - Memory percentage across all instances
    - Disk queue length across all instances
    - HTTP queue length across all instances
    - Data in
    - Data out
    - Scaling can also occur based on metrics for other Azure services
- Autoscaling rules analyze trends in metric values over time across all instances.
- Autoscale actions have a cool down period where the scale rule won't be triggered again. This allows the system to stabilize during between events. The minimum cool down period is 5 minutes.
- Note: Perhaps upgrade to a tier that supports scaling temporarily so that you can configure auto scaling: https://docs.microsoft.com/en-us/learn/modules/scale-apps-app-service/4-autoscale-app-service
- Scaling thresholds should be carefully considered. Apps autoscaling estimate what their final state will be after scaling, and this can cause rules to not be evaluated because of how closely they are defined.

## Deployment Slots
- Deployment slots allow you to preview, manage, test, and deploy your different development environments. Apps can be deployed to non-prod slots and apps can be swapped between two deployment slots (including the prod slot).
- App settings can be swaped, though this requires adding the WEBSITE_OVERRIDE_PRESERVE_DEFAULT_STICKY_SLOT_SETTINGS with a value of 0 or false to every slot of the app. Managed identities are never swapped. Settings and connection strings won't be swapped if they are set as a "Deployment slot setting."
- By default traffic is routed to the prod slot when using the prod URL. This can be configured to send traffic to another slot. A percentage of traffic can be sent to the other slot. Sessions will be pinned to the slot using cookies (specifically the x-ms-routing-name cookie with a value of self for prod and a name of the environment for another slot). This can also be routed manually with a query parameter of the same name.


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
- Developing a function app locally requires a number of tools to be installed. This section will be updated later.

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


# Blob Storage
- Object storage solution optimized for storing massive amounts of unstructured data (text or binary data, images, logs, backups, etc)
- Two performance levels for storage accounts
  - Standard - general purpose, recommended for most things
  - Premium - uses SSDs
- Three access tiers which you can switch between at any time
  - Hot - Optimized for frequent access. High storage cost, low access cost. Default tier.
  - Cool - Optimized for storing lots of data. Low storage cost, high access cost.
  - Archive - Available only for individual block blobs. Optimized for data that can tolerate several hours of retrieval latency. Most cost-effective option for storage, but accessing is more expensive than other tiers.
- Three resource types
  - Storage account - Unique namespace in Azure for your data. Provides an endpoint for the objects in the storage account.
  - Container in the storage account - Similar to a directory in a file system but used for organizing blobs.
  - Blob
    - Block blobs - Text and binary data
    - Append blobs - Made up of blocks and optimized for append operations. Ideal for things like logging data
    - Page blobs - Stores random access files. Used to store virtual hard drive (VHD) files and serve as disks for Azure VMs
- Security options
  - Automatically encrypted
    - Uses AES256 transparently
    - Encryption keys can be managed by Microsoft. You can also manage your own keys or provide a key.
  - Can be controled with AAD and RBAC
  - Can be secured in transit to apps
  - Can encrypt OS and data disks
  - Supports delegated access
- Redundancy options
  - Multiple copies of data are always stored
  - Primary region (3 replicated copies) support two options for replication
    - Locally redundant storage (LRS) - Copies data 3 times within the same physical location
    - Zone-redundant storage (ZRS) - Uses availability zones
  - Secondary region supports two options
    - Geo-redundant storage (GRS) - 3 copies made in primary region using LRS and copied to a single location in a secondary region (also LRS)
    - Geo-zone-redundant storage (GZRS) - Copies data across 3 availability zones in the primary region using ZRS and then copied to a single physical location in a secondary region using LRS

## Creating storage accounts
While this can be created in the Azure portal by searching for storage accounts, it may be more helpful to create a storage account through Azure Cloud Shell.
1. Open up a Cloud Shell
2. Create a new resource group
``` shell
az group create --name az204-blob-patrick-rg --location eastus
```
3. Create a blob block storage account
``` shell
az storage account create --resource-group az204-blob-patrick-rg --name \
az204blobpatrick --location eastus \
--kind BlockBlobStorage --sku Premium_LRS
```
4. Clean up the resource group when you're done
``` shell
az group delete --name az204-blob-patrick-rg --no-wait
```

## Storage Lifecycle
- General Purpose v2 and Blob storage accounts allow for rule based policies to transition data between access tiers. These rules can be used to manage expenses by moving data to cooler storage tiers when it's not used as frequently.
- Lifecycle policies are set using collections of rules in JSON
- Data in the archive tier is considered to be offline and can't be read or modified. In order to do anything with the data it must be rehydrated into the hot or cool tier. This can be done by copying to date into another tier (recommended for most scenarios) or by changing the blob's access tier.
  - A priority can be set for the rehydration operation by using the x-ms-rehydrate-priority header. Standard priority may take up to 15 hours and high priority will be prioritized over standard and may complete in under an hour for objects under 10 GB in size.
  - Note that changing a blob's tier doesn't affect last modified times and can result in data being archived after rehydration.

## Working with blob storage
https://docs.microsoft.com/en-us/learn/modules/work-azure-blob-storage/3-develop-blob-storage-dotnet has examples of using a .NET SDK for this.

# Resources/Misc
Azure Cloud Shell can be accessed directly at https://shell.azure.com/

Various things to install:
- .NET SDK 5.0
- .NET Core Tools
- Azure CLI
- VS Code Extensions
  - Azure Functions
  - C#
  - Azure Account
  - Azure CLI Tools
    - Sign out of this with Ctrl+Shift+P -> Azure: Sign Out