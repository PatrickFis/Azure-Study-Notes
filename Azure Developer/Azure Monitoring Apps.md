# Monitoring App Performance

## Azure Monitor
- Collects and analyzes telemetry from cloud and on-prem
- Ingests a variety of logs, creates metrics from the stored data, and then provides various functions on the collected data
- Collected data
  - Application monitoring data - Data about the performance of your code regardless of the platform that it's on
  - Guest OS monitoring data - Data about the OS on which your app is running. Can be Azure, another cloud, or on-prem
  - Azure resource monitoring data - Data about the operation of an Azure resource.
  - Azure subscription monitoring data: Data about the operation and management of an Azure subscription, as well as data about the health and operation of Azure itself.
  - Azure tenant monitoring data: Data about the operation of tenant-level Azure services, such as Azure Active Directory.
- Data collected by Azure Monitor fits into two types: metrics and logs
  - Metrics are numerical values that describe some aspect of a system at a particular time. They can support near real-time scenarios. 
  - Logs contain data organized into records with different sets of properties
- Various tools like Application Insights, Container Insights, and VM Insights leverage Azure Monitor

## Application Insights
- Application Insights is a feature of Azure Monitor
- Application Performance Management (APM) service
- Automatically detects performance anomalies
- Added to your app through an SDK or through an Application Insights Agent where supported (https://docs.microsoft.com/en-us/azure/azure-monitor/app/platforms)
- Aside from instrumenting a web app, you can also instrument background components and JavaScript
- Application Insights monitors the following
  - Request rates, response times, and failure rates - Find out which pages are most popular, at what times of day, and where your users are. See which pages perform best. If your response times and failure rates go high when there are more requests, then perhaps you have a resourcing problem.
  - Dependency rates, response times, and failure rates - Find out whether external services are slowing you down.
  - Exceptions - Analyze the aggregated statistics, or pick specific instances and drill into the stack trace and related requests. Both server and browser exceptions are reported.
  - Page views and load performance - reported by your users' browsers.
  - AJAX calls from web pages - rates, response times, and failure rates.
  - User and session counts.
  - Performance counters from your Windows or Linux server machines, such as CPU, memory, and network usage.
  - Host diagnostics from Docker or Azure.
  - Diagnostic trace logs from your app - so that you can correlate trace events with requests.
  - Custom events and metrics that you write yourself in the client or server code, to track business events such as items sold or games won.
- Application Insights supports monitoring/analyzing app performance in the following ways
  - At run time
  - At development time
  - Instrument your web pages
  - Analyze mobile app usage
  - Availability tests

### Log-based Metrics
- Application Insights has two kinds of log-based metrics
  - Log-based metrics - Translated into Kusto queries from stored events (more dimensions than standard metrics, useful for data analysis and ad-hoc diagnostics)
    - Log-based metrics can be added using the SDK or from automatic collection from auto-instrumentation
    - Can enable analysis through diagnostic traces
    - Supports volume reduction techniques if apps create too much telemetry data
  - Standard metrics - Stored as per-aggregated time series data (better performance than log-based metrics as they're pre-aggregated during collection, useful for dashboards and real-time alerting)
    - Newer SDKs pre-aggregate metrics during collection
    - Application Insights will aggregate your metrics for you if your SDK doesn't support it, though you won't benefit from reduced volume of data transmission.

### Availability Tests
- Application Insights can set up recurring tests to monitor app availability and responsiveness
- Requests are sent at regular intervals from points around the world
- Application Insights supports three types of availability tests (and up to 100 availability tests per Application Insights resource)
  - URL ping test (classic) - Validates whether an endpoint is responding and measures performance. Supports custom success criteria. This test relies on the DNS infrastructure of the public internet.
  - Standard test (preview) - Similar to the URL ping test. Includes SSL cert validation, proactive lifetime check, HTTP request verbs (for example GET, HEAD, POST), custom headers, and custom data.
  - Custom TrackAvailability test - A custom application can be created to run availability tests. See https://docs.microsoft.com/en-us/dotnet/api/microsoft.applicationinsights.telemetryclient.trackavailability?view=azure-dotnet for information from the SDK.

### Application Map
- Application Map lets you spot performance bottlenecks or failure hotspots across components in distributed apps
- Clicking through visual data shows more detailed diagnostics from things like Application Insights events or Azure diagnostics


# Studying from Youtube [video here](https://www.youtube.com/watch?v=7uZ8-_CsgkU&list=PLLc2nQDXYMHpekgrToMrDpVtFtvmRSqVt&index=15)


# Udemy Notes (Section 11)

## Azure Monitor
- Some resources in Azure have a monitoring tab available in the overview. These metrics are available from the monitoring service.
- Searching for Monitor in the portal will take you to the Azure Monitor.
- Azure Monitor will allow you to view metrics for your resources.
- Azure Monitor makes an activity log available that shows the administrative actions that happen as part of your subscription. These actions include things like creating roles, listing storage account keys, etc. Clicking on an event shows more information (and exposes data in a JSON format) for you.

### Alerts in Azure Monitor
- Alerts can be created in Azure Monitor by going to the Alerts blade.
- Creating an alert rule can be done by following these steps:
  1. Navigate to Azure Monitor
  2. Click on Alerts
  3. Click on Create -> Alert rule
  4. Change the "Filter by resource type" filter to All
  5. Select a resource (for example, a storage account)
  6. Click Done
  7. Click on "Next: Condition >" to pick a condition for the alert
  8. Select an available signal (for example: "Ingress")
  9. Configure the threshold for the alert
  10. Click on "Next: Actions >"
  11. Click on "Create action group" to create a group that defines what should happen when the alert is triggered
  12. Give the action group a resource group and a name
  13. Click on "Next: Notifications >"
  14. Select a notification type (Either "Email Azure Resource Manager Role" or "Email/SMS message/Push/Voice")
  15. Click "Next: Actions >" (this will allow you to pick something like an Azure Function or Logic App to get triggered by the alert)
  16. Create the action group and get taken back to the alert rule
  17. Give the alert rule a name
  18. Click Create
  19. Get the alert to fire and verify that you received a notification
  20. (Optional) Delete the alert and the action group when you're done with them
- Azure Monitor's Alerts also support dynamic thresholds instead of static thresholds. The difference is that static thresholds use a user-defined value to evaluate the rule while dynamic thresholds use machine learning algorithms to learn the metric's behavior pattern and calculate the thresholds automatically.
  - Dynamic alerts let you specify the sensitivity of the threshold. The following sensitivities are available:
    - High: The thresholds will be tight and close to the metric series pattern. An alert rule will be triggered on the smallest deviation, resulting in more alerts.
    - Medium: The thresholds will be less tight and more balanced. There will be fewer alerts than with high sensitivity (default).
    - Low: The thresholds will be loose with more distance from metric series pattern. An alert rule will only trigger on large deviations, resulting in fewer alerts.
  - See [MS Documentation on Dynamic Thresholds](https://learn.microsoft.com/en-us/azure/azure-monitor/alerts/alerts-dynamic-thresholds).

### ARM Labs
- See ActionGroup.json, AzureMonitorDynamicVMCPUMetrics.json, and AzureMonitorVMCPUMetrics.json for ARM templates for deploying alerts/action groups to Azure Monitor. There needs to be a VM named "appvm" for the alerts to work. [Code/Azure Arm](Code/Azure%20ARM/).

## Log Analytics Workspace
- Log Analytics Workspace is a central solution for all of your logs.
  - Can accept logs from Azure or on-prem
  - Kusto query language is used to query the workspace
  - Marketplace solutions are available to extend Log Analytics

### Creating a Log Analytics Workspace
1. Search the marketplace for Log Analytics Workspace
2. Give the workspace a resource group, name, and region
3. Create the resource

After creating the workspace you'll need to configure resources to send logs to it by following these steps:
1. Navigate to any of the options under "Workspace Data Sources" in the Log Analytics Workspace
2. Click Add/Connect
3. Walk through the process of setting up the logs

Things like web apps can send their logs to Log Analytics as well with the following steps:
1. Open the resource
2. Navigate to Diagnostic Settings
3. Check "Send to Log Analytics workspace"
4. Check the categories that you'd like to send to Log Analytics
5. Click Save

The logs can take a decent bit of time to start showing up in Log Analytics.

## Application Insights
- Provides performance monitoring for web apps
- Supports apps in Azure, on-prem, or other cloud platforms
- Uses an instrumentation SDK for your app. Or an app insights agent.
  - JavaScript is available for web pages.

### Working with App Insights
#### Local Usage
1. Open a project in VS (in my case: UdemyWebApp)
2. Right click the project
3. Click "Configure Application Insights..."
4. For local testing pick the "Application Insights Sdk (Local)" option
5. Click Next
6. Click Finish
7. Note that a NuGet package was added as well as a line to Program.cs: builder.Services.AddApplicationInsightsTelemetry();
8. Save and run the app
9. Access more information from View -> Other Windows -> Application Insights Search
10. Click the debug telemetry option and view the events that were picked up

#### Usage in Azure
1. Navigate to an App Service(App Insights can be used for more, but this is just convenient. I'm using my UdemyWebApp service in my fischerpl18_rg_1227 resource group)
2. Click the "Application Insights" blade
3. Click "Turn on Application Insights"
4. Click Enable
5. Configure the options you're interested in
6. Click Apply
7. Open UdemyWebApp in VS
8. Click on "Connected Services"
9. On "Application Insights Sdk (Local)" click on "Edit dependency"
10. Click "Disconnect" (this is just so that we can rely on App Insights in Azure)
11. Publish the application 

#### Available Features
- SQL queries can be captured from your application with code like this: builder.Services.ConfigureTelemetryModule<DependencyTrackingTelemetryModule>((module, o) => { module.EnableSqlCommandTextInstrumentation = true; });
- Users, Sessions and Events
  - Users - You can see how many people are using your app and its features
  - Session - You can see sessions of user activity including pages and features of your app
  - Events - Shows how often certain pages and features have been used in your app
- Funnels - Here you can see multiple stages like a pipeline. This will show you how users are progressing through your app as an entire process.
- Cohorts - A set of users, sessions, events or operations that have something in common. This can be used to analyze a particular set of users or events.
- Impact - You can see how load times and other aspects of your app affect the conversion rate of your app.
- Retention - See how many users return to your app.
- User flows - Helps answer these questions:
  1. What do users click on a page within an app?
  2. Where are the places within the app that users churn the most from the site?
  3. Are there places in the app where the users repeat the same action over and over again?
- Availability Tests [MS Documentation](https://learn.microsoft.com/en-us/azure/azure-monitor/app/availability-overview)
  - Tests can be defined which monitor the availability and responsiveness of an app
  - Can be used to send web requests to your app from different points across the world
  - Can run against HTTP or HTTPS endpoints which are available over the public internet
  - Can be used to test REST APIs
  - URL ping test (classic): You can create this test through the Azure portal to validate whether an endpoint is responding and measure performance associated with that response. You can also set custom success criteria coupled with more advanced features, like parsing dependent requests and allowing for retries.
  - Standard test: This single request test is similar to the URL ping test. It includes TLS/SSL certificate validity, proactive lifetime check, HTTP request verb (for example, GET, HEAD, or POST), custom headers, and custom data associated with your HTTP request.
  - Multi-step web test (classic): You can play back this recording of a sequence of web requests to test more complex scenarios. Multi-step web tests are created in Visual Studio Enterprise and uploaded to the portal, where you can run them.
  - Custom TrackAvailability test: If you decide to create a custom application to run availability tests, you can use the TrackAvailability() method to send the results to Application Insights.