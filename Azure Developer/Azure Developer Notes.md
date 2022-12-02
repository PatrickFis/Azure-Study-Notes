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
- Message-based solutions [Link (note that you need to finish the exercises for this and write some C# code)](Azure%20Message%20Based%20Solutions.md)
  - Azure Service Bus
  - Azure Queue Storage

# Resources/Misc
Azure Cloud Shell can be accessed directly at https://shell.azure.com/

YouTube series recommended by coworkers: https://www.youtube.com/playlist?list=PLLc2nQDXYMHpekgrToMrDpVtFtvmRSqVt

Fixing Intellisense with OmniSharp in VS Code (solution: Ctrl+Shift+P -> OmniSharp: Select Project -> select the Azure Study Notes project): https://stackoverflow.com/questions/29975152/intellisense-not-automatically-working-vscode

Tables can be easily copied from the documentation using https://www.tablesgenerator.com/markdown_tables

There are a variety of samples available on Github: https://github.com/Azure-Samples

Labs are available here: https://microsoftlearning.github.io/AZ-204-DevelopingSolutionsforMicrosoftAzure/

Various things to install:
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