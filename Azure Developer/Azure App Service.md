# Azure App Service [MS Documentation for Azure App Service](https://learn.microsoft.com/en-us/azure/app-service/overview)
- Service for hosting web apps, REST APIs, mobile back ends.
- Supports lots of languages
- Has built in support to scale up/down or out/in. Can scale CPU cores and RAM as well as machine instances running app.
- Provides support for CI/CD out-of-the-box using Azure DevOps, GitHub, Bitbucket, FTP, or a local Git repo on a development machine.

## Azure App Service Plans [MS Documentation for App Service Plans](https://learn.microsoft.com/en-us/azure/app-service/overview-hosting-plans)
- A plan defines a set of compute resources for an app to run in. One or more apps can use the same resources/plan. Azure Functions can also run in a plan.
- Plans define the region, amount and size of VMs, and the pricing tier.
- Pricing tier categories
  - Shared compute
    - Free + Shared share resource pools with other customers.
    - The free tier comes with none of the extra features of the other plans
    - The shared plan is similar to the free tier but it supports custom domains.
  - Dedicated compute - Basic, Standard, Premium, PremiumV2, and PremiumV3 run on dedicated VMs. Higher tiers = more VMs to scale out.
    - You're charged based on each VM instance in the app service plan in this tier.
    - The dedicated tiers all support the following features (except for auto scale in the basic plan):
      - Custom domains, auto scale, hybrid connectivity, virtual network connectivity, and private endpoints
  - Isolated - Runs on a dedicated VNet.
    - You're charged based on the number of workers that run your apps. In addition there's a flat Stamp Fee for using an App Service Environment.
    - This supports all of the features noted in the dedicated plan
  - Consumption - Only available to function apps. Scales dynamically depending on workload.

### How does my app run and scale?
- In the free/shared tiers an app receives CPU minutes on a shared VM and cannot scale out. In other tiers it scales like this:
  - When apps run, they run on all the VM instances configured in the ASP. If multiple apps are in the same ASP then they all share the same VM instances. Example: If a plan is configured to run five VM instances, then all apps in the plan run on all five instances. If you use autoscaling, then all apps in the plan are scaled out together based on the autoscale settings.
  - If you have multiple deployment slots for an app, all deployment slots also run on the same VM instances.
  - If you enable diagnostic logs, perform backups, or run WebJobs, they also use CPU cycles and memory on these VM instances.
  - The ASP is the scale unit of App Service apps.

### How do I deploy apps?
- Automated (continuous) deployments can be configured from several sources: Azure DevOps, GitHub, and Bitbucket.
- Manual deployments can be done with a few different options:
  - Git: App Service web apps can use a Git URL that you add as a remote repository. Pushing to the remote repository will deploy your app.
  - CLI: `az webapp up` can be used to package and deploy your app. It can also make a new App Service web app for you if you don't have one already.
  - Zip deploy: You can use curl or a similar HTTP utility to send a ZIP of your application files to App Service.
  - FTP/S: FTP or FTPS can be used to push your code to a hosting environment like App Service.
- The general recommendation is to use deployment slots when deploying to production (though this will require a Standard ASP or better). You can deploy your app to a staging environment and then swap your staging and production slots. This'll help warm your app up and eliminate downtime.

## Authentication & Authorization
- App Service supports a number of identity providers: Microsoft Identity Platform, Facebook, Google, Twitter, and any OpenID Connect provider.
- The authN + authZ module runs alongside your app code. When enabled HTTP requests pass through it before being handled by your app. It'll handle the following for you:
  - Authenticate users
  - Validates, store, and refresh tokens
  - Manage the authenticated session
  - Inject identity information into request headers
- Supports authentication flows with or without a provider SDK.
- Allows you to specify behavior for unauthenticated requests: they can either be allowed or require authentication (which may not be desirable for SPAs).

## Networking [MS Docs](https://learn.microsoft.com/en-us/azure/app-service/networking-features)
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
4. Clone this git repo inside the directory: git clone https://github.com/Azure-Samples/html-docs-hello-world.git
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
  - Note for ASP.NET and ASP.NET Core applications: Settings in the Application Settings section are like setting things in `<appSettings>` in Web.config or appsettings.json, except the values in the App Service override the ones in those files.
  - The settings are always encrypted when stored (encrypted-at-rest).
- App settings can be added one at a time as a key-value pair. They can also be edited in bulk by clicking on the advanced edit button on the application settings page (which will show the settings in a JSON format).
- Connection strings can be added as app settings. They are encrypted at rest and use specific formatting (see https://docs.microsoft.com/en-us/learn/modules/configure-web-app-settings/2-configure-application-settings).
- App Services have general settings under Configuration -> General settings. These include the following:
  - Stack settings - Software used to run the app
  - Platform settings - Hosting platform config
    - Bitness - 32/64 bit
    - WebSocket protocol
    - Always On - Keeps the app loaded even without traffic. By default apps are unloaded after 20 minutes of no incoming requests
    - Managed pipeline version - IIS pipeline mode
    - HTTP version
    - ARR affinity - Used in multi-instance deployments to ensure clients are routed to same instance throughout their session
  - Debugging - Remote debugging for ASP.NET, ASP.NET Core, or Node.js apps. Turns itself off after 48 hours.
  - Incoming client certificates - Requires client certs in mutual authN.
- Handler mapping and virtual application/directory mappings can be managed through Configuration -> Path mappings. These settings allow for custom script processors to be used to handle requests to specific file extensions. The Path mappings page will show different options based on the OS type:
  - Windows apps (uncontainerized)
    - IIS handler mappings and virtual applications and directories can be customized
      - By default / is mapped to D:\home\site\wwwroot (since this is where your code is deployed by default). You can edit this if it's in a different folder or if you have more than one application in your repository by adding virtual applications and directories.
    - Custom script processors can be used to handle requests for specific file extensions by doing the following:
      - Select New handler
      - Specify the extension you want to handle (*.php, handler.fcgi, etc)
      - Give an absolute path to the script processor that will be used to process matching files (note: D:\home\site\wwwroot will refer to your app's root directory)
      - Optional command line arguments can be specified
  - Linux and containerized apps
    - Custom storage can be added for containerized apps. This setting is available under Path mappings by clicking on the "New Azure Storage Mount" button. It needs various options:
      - Name: The display name.
      - Configuration options: Basic or Advanced.
      - Storage accounts: The storage account with the container you want.
      - Storage type: Azure Blobs or Azure Files. Windows container apps only support Azure Files.
      - Storage container: For basic configuration, the container you want.
      - Share name: For advanced configuration, the file share name.
      - Access key: For advanced configuration, the access key.
      - Mount path: The absolute path in your container to mount the custom storage.
- App service logs can be enabled through the Azure portal in the App Service logs section. The following log options are available:
  - Application logging - Available on Windows and Linux. Logs messages generated by application code using a logging pattern standard to your language.
    - Enabled by setting Application Logging (Filesystem or Blob for Windows and Filesystem for Linux) to on. On Windows Filesystem logging disables itself after 12 hours while blob is for long term storage. On Linux logs are given a quota and retained for a set number of days.
  - Web server logging - Available on Windows. Logs raw HTTP request data including the HTTP method, resource URI, client IP, client port, user agent, response code, etc.
  - Detailed error logging - Available on Windows. Copies of error pages that were sent to the client browser.
  - Failed request tracing - Available on Windows. Trace info on failed requests. One folder is generated for each request containing a XML log file and XSL stylesheet to view the log file with.
  - Deployment logging - Available on Windows and Linux. Helps determine why a deployment failed.
  - https://learn.microsoft.com/en-us/azure/app-service/troubleshoot-diagnostic-logs is a good resource for learning more about logging.
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
- Autoscaling rules analyze trends in metric values over time across all instances. This analysis is a multi-step process:
  - First the rule aggregates values for a metric across all instances across a period of time (known as the time grain). Each metric has its own time grain, but in most cases this period is 1 minute. The aggregated value is known as the time aggregation. The options available are Average, Minimum, Maximum, Sum, Last, and Count.
  - Afterwards a second step is performed to aggregate data over a longer, user-specified period known as the Duration. The minimum duration is 5 minutes. Example: If you set the Duration to 10 minutes, then the autoscale rule would aggregate the 10 values calculated for the time grain.
    - The aggregation calculation for the Duration can be different from that of the time grain. For example, if the time aggregation is Average and the statistic gathered is CPU Percentage across a one-minute time grain, each minute the average CPU percentage utilization across all instances for that minute will be calculated. If the time grain statistic is set to Maximum, and the Duration of the rule is set to 10 minutes, the maximum of the 10 average values for the CPU percentage utilization will be used to determine whether the rule threshold has been crossed.
- Autoscale actions:
  - When an autoscale rule detects that a metric has crossed a threshold it can perform an autoscale action
  - The action be be either scale-out (increase the number of instances) or scale-in (decrease the number of instances)
  - The autoscale action uses an operator (less than, greater than, equal to, and so on) to determine how to react to the threshold
  - Autoscale actions have a cool down period where the scale rule won't be triggered again. This allows the system to stabilize between autoscale events. The minimum cool down period is 5 minutes.
  - Autoscale rules can be combined, and the autoscale conditions don't have to be directly related. When determining whether to scale out, the autoscale action will be performed in ANY of the scale-out rules are met. When scaling in, the autoscale action will run ONLY IF ALL the scale-in rules are met (so if you need to scale in if any of your scale-in rules are met then you must define the rules in separate autoscale conditions).
  - Best practices
    - Make sure your max/min values are different and have an adequate margin between them
    - Use the most appropriate metric (the most common statistic is Average)
    - Scaling thresholds should be carefully considered. Apps autoscaling estimate what their final state will be after scaling, and this can cause rules to not be evaluated because of how closely they are defined.
    - Consider what will happen if you have multiple rules defined (see the combined bullet point above)
    - Select a safe default instance count
    - Configure notifications: Autoscale will post things to the Activity Log (issues with a scale operation, successfully completing a scale action, failing to take a scale action, metrics aren't available for the autoscale service to make a scale decision, and metrics are available (recovery) again to make a scale decision). You can use this to monitor the health of the autoscale engine. You can also configure email or webhook notifications to get notified for successful scale actions via the notifications tab on the autoscale setting.
  - Note: Perhaps upgrade to a tier that supports scaling temporarily so that you can configure auto scaling: https://docs.microsoft.com/en-us/learn/modules/scale-apps-app-service/4-autoscale-app-service
  - The following links have more information about autoscaling
    - https://learn.microsoft.com/en-us/azure/azure-monitor/autoscale/autoscale-overview
    - https://learn.microsoft.com/en-us/azure/azure-monitor/autoscale/autoscale-understanding-settings
    - https://learn.microsoft.com/en-us/azure/azure-monitor/autoscale/autoscale-flapping

## Deployment Slots
- Deployment slots allow you to preview, manage, test, and deploy your different development environments. Apps can be deployed to non-prod slots and apps can be swapped between two deployment slots (including the prod slot).
- App settings can be swapped, though this requires adding the WEBSITE_OVERRIDE_PRESERVE_DEFAULT_STICKY_SLOT_SETTINGS with a value of 0 or false to every slot of the app. Managed identities are never swapped. Settings and connection strings won't be swapped if they are set as a "Deployment slot setting."
- By default traffic is routed to the prod slot when using the prod URL. This can be configured to send traffic to another slot. A percentage of traffic can be sent to the other slot. Sessions will be pinned to the slot using cookies (specifically the x-ms-routing-name cookie with a value of self for prod and a name of the environment for another slot). This can also be routed manually with a query parameter of the same name.

### How swapping slots works
1. Settings are applied from the target slot to all instances of the source slot
   1. Slot-specific app settings and connection strings
   2. Continuous deployment settings
   3. App Service authentication settings
   4. These all trigger instances in the source slot to restart
2. Wait for every source slot instance to restart (if any fail then the swap operation reverts all changes to the source slot and stops the operation)
3. If local cache is enabled then it is initialized by making an HTTP request to the application root ("/") on each instance of the source slot. It waits for each instance to respond, and it also triggers another restart on each instance.
4. If auto swap with custom warm-up is enabled, then an HTTP request is sent to the application root for each instance of the source slot to trigger Application Initiation. If an instance returns any HTTP response it's considered to be warmed up.
5. After warming up the source slots, swap the slots by switching the routing rules for the two slots. The production slot now has the app that's been warmed up in the source slot.
6. Perform the same operation by applying all settings and restarting the source slot instances.
- This table shows what happens with settings during a swap (* means they're planned to be unswapped):
  | Settings that are swapped                                           | Settings that aren't swapped                 |
  | ------------------------------------------------------------------- | -------------------------------------------- |
  | General settings, such as framework version, 32/64-bit, web sockets | Publishing endpoints                         |
  | App settings (can be configured to stick to a slot)                 | Custom domain names                          |
  | Connection strings (can be configured to stick to a slot)           | Non-public certificates and TLS/SSL settings |
  | Handler mappings                                                    | Scale settings                               |
  | Public certificates                                                 | WebJobs schedulers                           |
  | WebJobs content                                                     | IP restrictions                              |
  | Hybrid connections *                                                | Always On                                    |
  | Virtual network integration *                                       | Diagnostic log settings                      |
  | Service endpoints *                                                 | Cross-origin resource sharing (CORS)         |
  | Azure Content Delivery Network *                                    |                                              |

# Studying from Youtube [video here](https://www.youtube.com/watch?v=4ys2Y1rs4-I&list=PLLc2nQDXYMHpekgrToMrDpVtFtvmRSqVt&index=4)
## Development Notes
- I'm going to continue using the resource group that was created for the initial webapp that was deployed using documentation from Microsoft Learn.

### SQL Server & Managed Identity
- I'm spinning up a SQL database to utilize in the app because part of the exam expects you to understand connection strings. (Username: az204patrick)
- Note that I started looking into the built in dependency injection provided by the framework in Program.cs (https://docs.microsoft.com/en-us/aspnet/core/fundamentals/dependency-injection?view=aspnetcore-6.0 has more information about it).
- At first I added the connection string to appsettings.json, and then moved it to the app service configuration in Azure. This doesn't work locally and I don't really want to commit my database password into Github, so I'm going to replace it with Key Vault.
  - I enabled the system assigned identity for my Azure App Service and added the Key Vault Secrets User role to it. I had to create a new Key Vault int he same resource group so that I could use RBAC to access it, as the existing one that I had created while going through Microsoft Learn used a "Vault access policy" permission model and I was using it to retrieve values using Postman. I also had to grant myself the Key Vault Administrator role to work with the new Key Vault.
  - I added the Azure.Security.KeyVault.Secrets and Azure.Identity packages to the app.
  - I updated [this](Code/Azure%20App%20Service/TestWebApp/Services/CourseService.cs) with the code to retrieve secrets from the Key Vault.

### App Configuration
- I created a free App Configuration for the web app through the Azure portal.
- I added the Microsoft.Extensions.Configuration.AzureAppConfiguration and Microsoft.FeatureManagement packages to my application.
- I created a new [class](Code/Azure%20App%20Service/TestWebApp/Services/AppConfigService.cs) with the code needed to retrieve values from the config. I used Key Vault to store the connection string under the name AppConfigurationConnectionString.
- I added a config value and displayed it on the index page.
- I added a feature flag and used it to conditionally display a value on the index page.
- I added a feature flag to retrieve courses from the database directly or go through an Azure Function.

## Other notes (things I couldn't deal with like custom domains and SSL)
- Custom domains can be added through the Azure portal's Custom domains section. You can add DNS records to verify that you own the domain, and then Azure will allow you use your domain.
- TLS/SSL can be added through the TLS/SSL settings menu. You can provide your own certificates or you can have Azure generate some for you.


# Studying from Udemy
An App Service Plan was created using the following settings (note: the app service plan will be deleted when not in use since it's fairly expensive):
- OS: Windows
- Region: East US
- Pricing Plan: Standard S1

A web app was created with the following settings (note: you won't be able to publish from Visual Studio without this, even if you have an App Service Plan):
- Publish: Code
- Runtime stack: .NET 6 (LTS)
- OS: Windows
- Region: East US
- Application Insights: No

The application published to the app service is stored [here](Code/Visual%20Studio%20Projects/UdemyWebApp/).

## Connecting the app to a SQL Server instance
- An existing SQL database was reused for this. The password was reset and stored in a connection string to try out the App Service's configuration settings for connection strings.


## Logging
- Logs were temporarily enabled to test out log streaming. This was done through the Azure portal.
- The app service can log to storage associated with the managed service hosting the app service.

## Deployment Slots
- A deployment slot named staging was created from the portal
  - App Service -> Deployment section -> Deployment slots
- A small tweak was made to the app's index page and deployed to the staging deployment slot to try out the feature
  - The swapping feature was also tested. This let me swap my staging version into the production slot.
- A second SQL database was provisioned to test out deployment slot settings.
  - The connection string configured for the SQL database the app is using was updated to be a deployment slot setting. This allowed me to specify a specific connection string for my staging slot to use. Swapping staging and production showed that the app would display the correct data from the database each slot was supposed to connect to.

## App Configuration
- I already have a free app config in another resource group so I'm reusing that.
- Using this resource required updates to my application.
  - I needed to add a dependency on Microsoft.Extensions.Configuration.AzureAppConfiguration.
  - Features flags required Microsoft.FeatureManagement.AspNetCore.
  - Program.cs needed to be updated to tell the builder to use the app config as well as to enable feature flags.
    - I stored the connection string for the app config inside a connection string setting in the app service's configuration so that I could avoid hardcoding it.
  - The service the program uses for retrieving data was updated to get a connection string for the SQL server from the app config as well as a method to check a feature flag.

# Misc
- See https://learn.microsoft.com/en-us/shows/exam-readiness-zone/preparing-for-az-204-develop-azure-compute-solutions-1-of-5 for information on what may appear on the exam.
  - Be familiar with pricing plans and their differences and features, etc
  - Be familiar with the applications that can run inside a single ASP
  - Be familiar with scaling and autoscaling (enabling it, conditions, rules, metrics, monitoring)
  - Be aware of CI/CD options, though they typically won't be the focus of a question
    - Be familiar with deploying an application through the CLI and Powershell
  - Be familiar with deployment slots
  - Be familiar with logging and where they can be published
  - Be aware of configuration options in apps (know how to change the settings in the CLI and Powershell, understand deployment slot settings)
  - Be aware of app settings