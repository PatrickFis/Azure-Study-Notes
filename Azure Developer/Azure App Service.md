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
- App settings can be swapped, though this requires adding the WEBSITE_OVERRIDE_PRESERVE_DEFAULT_STICKY_SLOT_SETTINGS with a value of 0 or false to every slot of the app. Managed identities are never swapped. Settings and connection strings won't be swapped if they are set as a "Deployment slot setting."
- By default traffic is routed to the prod slot when using the prod URL. This can be configured to send traffic to another slot. A percentage of traffic can be sent to the other slot. Sessions will be pinned to the slot using cookies (specifically the x-ms-routing-name cookie with a value of self for prod and a name of the environment for another slot). This can also be routed manually with a query parameter of the same name.

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

## Other notes (things I couldn't deal with like custom domains and SSL)
- Custom domains can be added through the Azure portal's Custom domains section. You can add DNS records to verify that you own the domain, and then Azure will allow you use your domain.
- TLS/SSL can be added through the TLS/SSL settings menu. You can provide your own certificates or you can have Azure generate some for you.