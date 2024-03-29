# Overview
The exam will require knowledge of C# or Python

The Azure Developer certification covers a number of different areas (https://query.prod.cms.rt.microsoft.com/cms/api/am/binary/RE4oZ7B):

## Develop Azure compute solutions (25-30%)
- Implement IaaS solutions
  - VMs [Link](Azure%20IaaS%20-%20VMs.md)
  - ARM [Link](Azure%20IaaS%20-%20ARM.md)
  - Containers (Note: ACI does not have a free tier) [Link](Azure%20IaaS%20-%20Containers.md)
- Azure App Service Web Apps [Link](Azure%20App%20Service.md)
  - Creation
  - Diagnostics logging
  - Deployment
  - Configuration
  - Autoscaling
- Azure Functions [Link](Azure%20Functions.md)
  - Creation
  - Deployment
  - Input/output bindings
  - Triggers with data operations, timers, and webhooks
  - Durable Functions

## Develop for Azure storage (15-20%)
- Cosmos DB [Link](Azure%20Cosmos%20DB.md)
  - API/SDK usage
  - Partitioning
  - Data operations
  - Consistency level
  - Change feed notifications
- Blob storage [Link](Azure%20Blob%20Storage.md)
  - Move items between storage accounts or containers
  - Set and retrieve properties and metadata
  - Data operations using the SDK
  - Storage policies, data archiving, retention

## Implement Azure security (20-25%)
- User authN and authZ [Link](Azure%20User%20Authentication%20and%20Authorization.md)
  - AuthN and AuthZ using Microsoft Identity platform
  - AuthN and AuthZ using AAD
  - Shared access signatures
  - Microsoft Graph
- Secure cloud solutions [Link](Azure%20Secure%20Cloud%20Solutions.md)
  - Secure app config data with App Configuration or Azure Key Vault
  - Use keys, secrets, and certs in Azure Key Vault
  - Managed Identities

## Monitor, troubleshoot, and optimize Azure solutions (15-20%)
- Caching (Note: Redis does not have a free tier) [Link](Azure%20Caching.md)
  - Cache and expiration policies for Azure Cache for Redis
  - Data sizing, connections, encryption, and expiration
- Troubleshooting with metrics and log data [Link](Azure%20Message%20Based%20Solutions.md)
  - Application Insights
  - Analysis of metrics and log data

## Connect to and consume Azure services and third-party services (15-20%)
- API Management [Link](Azure%20API%20Management.md) (Note: APIM allows for 1 million calls for free per month)
  - APIM instances
  - Create and document APIs
  - API authN
  - Policies for APIs
- Event-based solutions [Link](Azure%20Event%20Based%20Solutions.md)
  - Azure Event Grid
  - Azure Event Hub (Note: Event Hub does not have a free tier)
- Message-based solutions [Link](Azure%20Message%20Based%20Solutions.md)
  - Azure Service Bus
  - Azure Queue Storage

# Resources/Misc
- Azure Cloud Shell can be accessed directly at https://shell.azure.com/
- YouTube series recommended by coworkers: https://www.youtube.com/playlist?list=PLLc2nQDXYMHpekgrToMrDpVtFtvmRSqVt
- Fixing Intellisense with OmniSharp in VS Code (solution: Ctrl+Shift+P -> OmniSharp: Select Project -> select the Azure Study Notes project): https://stackoverflow.com/questions/29975152/intellisense-not-automatically-working-vscode
- Tables can be easily copied from the documentation using https://www.tablesgenerator.com/markdown_tables
- There are a variety of samples available on Github: https://github.com/Azure-Samples
- Labs are available here: https://microsoftlearning.github.io/AZ-204-DevelopingSolutionsforMicrosoftAzure/
- Exam prep videos from Microsoft: https://learn.microsoft.com/en-us/shows/exam-readiness-zone/preparing-for-az-204-develop-azure-compute-solutions-1-of-5

Various things to install:
- VS Code
- Visual Studio 2022 Community Edition (just install this instead of the SDK and VS Code Extensions below)
- Azure Data Studio
- Azure Storage Explorer
- .NET SDK 5.0/6.0
- .NET Core Tools
- Azure CLI
- VS Code Extensions
  - Azure Functions
  - Azure Tools
  - C#
  - Azure Account
  - Azure CLI Tools
    - Sign out of this with Ctrl+Shift+P -> Azure: Sign Out


# Udemy Progress
- Complete: Sections 1-12
- Complete but maybe revisit: Section 6 (probably easier to look up Powershell, Azure CLI, and ARM on your own), Section 11 on Azure Front Door


# Review Topics
- This is a section of areas that I'd like to review since I've finished the Udemy course.
- Course material
  - Azure Front Door
    - [Azure Caching - Azure Front Door](Azure%20Caching.md#azure-front-door)
  - When to use a Service Bus, Storage Account Queue, Event Grid, or Event Hubs 
    - Service Bus vs Storage Queue: See [Azure Message Based Solutions](Azure%20Message%20Based%20Solutions.md#azure-message-queues-ms-link)
    - Event Grid vs Event Hubs: See [Azure Event Based Solutions](Azure%20Event%20Based%20Solutions.md#when-should-i-use-event-grid-or-event-hubs-ms-link)
    - https://learn.microsoft.com/en-us/azure/event-grid/compare-messaging-services
  - Cosmos DB APIs
    - [Azure Cosmos DB APIs](Azure%20Cosmos%20DB.md#supported-apis)
- Practice test 1 areas (test #1):
  - Accessing blobs through managed identities (specifically code snippets for getting an access token)
    - See [MS Link](https://learn.microsoft.com/en-us/rest/api/storageservices/authorize-with-azure-active-directory).
  - Permissions for interacting with blobs
    - See [MS Link](https://learn.microsoft.com/en-us/rest/api/storageservices/authorize-with-azure-active-directory).
    - Azure Storage accepts access tokens from users, service principals, managed identities, and from applications using permissions delegated by users. Azure Storage exposes a single delegation scope named user_impersonation that permits applications to take any action allowed by the user.
  - Service Bus Topic Filters
    - See [Azure Message Based Solutions - Topic Filters](Azure%20Message%20Based%20Solutions.md#topic-filters)
  - Azure Function bindings
    - See [Azure Functions - Azure Function Bindings for Queue Storage](Azure%20Functions.md#azure-function-bindings-for-queue-storage-ms-link])
  - Claim types for ID tokens
    - See [Azure User Authentication and Authorization - Claims in an ID Token](Azure%20User%20Authentication%20and%20Authorization.md#claims-in-an-id-token-ms-link)
- Practice test 2 areas (test #1):
  - Azure Container Registry permissions
    - See [Azure Iaas - Containers - Azure Container Registry Permissions](Azure%20IaaS%20-%20Containers.md#azure-container-registry-permissions)
  - Microsoft Authentication Library
    - See https://learn.microsoft.com/en-us/azure/active-directory/develop/msal-overview and locate more resources for this?
  - Revoking SAS for a storage account
    - [Azure Blob Storage - Shared Access Signatures](Azure%20Blob%20Storage.md#shared-access-signatures)
  - Application Insights test features (standard vs multi-step)
    - See [Azure Monitoring Apps - Available Features](Azure%20Monitoring%20Apps.md#available-features)
  - Azure Cache for Redis (specifically the commands)
    - See [Azure Caching - Redis Data Types](Azure%20Caching.md#redis-data-types-redis-documentation-and-more-redis-documentation)
  - Remember the difference and use case for each different API for Cosmos DB