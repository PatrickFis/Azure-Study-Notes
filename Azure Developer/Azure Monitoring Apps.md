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