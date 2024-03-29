# API Management
API Management helps organizations publish APIs to external, partner, and internal developers to unlock the potential of their data and services.

## Features
Made of the following components
- API gateway
  - Accepts API calls and routes them
  - Verifies API keys, JWT tokens, certs, and more
  - Enforces usage quotas and rate limits
  - Transforms your API on the fly without code modifications
  - Caches backend responses
  - Logs call metadata for analytics purposes
- Azure portal as an administrative interface to do the following
  - Define or import API schema
  - Package APIs into products
  - Set up policies like quotas or transformations on the APIs
  - Get insights from analytics
  - Manage users
- Developer portal for devs to do the following
  - Read API documentation
  - Try the API via the interactive console
  - Create an account and subscribe to get API keys
  - Access analytics on their own usage

### Products
APIs are given to devs in the form of products. Products in API Management have one or more APIs and can be open or protected. Protected products must be subscribed to before they can be used while open products can be used without a subscription. Subscription approval is configured at the product level.

### Groups
Groups are used to manage the visibility of products to devs. API Management has the following groups
- Administrators - Azure subscription admins. Manage the APIM instance, create APIs, operations, and products used by devs.
- Devs - Authenticated developer portal users. Devs are the customers that build apps using your APIs.
- Guests - Unauthenticated developer portal users. Can be granted read-only access.
- Custom groups can be created
- External groups in AAD can be used

## Benefits of an API Gateway
- API gateways sit between clients and services and act as a reverse proxy (it routes requests from clients to services).
- API gateways can solve various issues which occur when you expose your services directly to clients:
  - Complex client code as the client must keep track of multiple endpoints and handle failures
  - Coupling between the client and backend since it needs to understand how the service is set up. It makes it harder to refactor both the client and the service.
  - One operation may need to call multiple services (so multiple network round trips adding latency)
  - Each public-facing service must handle things like authentication, SSL, and client rate limiting
  - Services must limit their choice of communication protocols to something friendly like HTTP or WebSocket
  - Services need to harden their public endpoints and they are a potential attack surface
- These problems are solved by an API gateway since it decouples your clients from your services
- Gateways can perform a number of additional functions grouped by design patterns:
  - Gateway routing: The gateway acts as a reverse proxy to route requests to one or more backend services using layer 7 routing. The gateway is a single endpoint for clients (decoupling the client and service).
  - Gateway aggregation: The gateway can aggregate multiple individual requests into a single request. This pattern is used when a single operation calls multiple backend services. The client sends one request, the gateway sends requests to various backend services, and then the gateway aggregates the results and sends them back to the client (reducing chattiness).
  - Gateway offloading: The gateway can handle functionality that multiple services need (especially cross-cutting concerns). Some examples include:
    - SSL termination
    - Authentication
    - IP allow/block list
    - Client rate limiting (throttling)
    - Logging and monitoring
    - Response caching
    - GZIP compression
    - Servicing static content

## Policies
- Policies allow APIM to change the behavior of an API through configuration. Policies are a collection of statements that are executed sequentially on the request or response of an API. Popular statements include format conversion and call rate limiting.
- Policies are defined through XML files that describe a sequence of inbound and outbound statements. The configuration is divided into inbound, backend, outbound, and on-error. The configuration looks like this:
  ``` xml
  <policies>
    <inbound>
      <!-- statements to be applied to the request go here -->
    </inbound>
    <backend>
      <!-- statements to be applied before the request is forwarded to the backend service go here -->
    </backend>
    <outbound>
      <!-- statements to be applied to the response go here -->
    </outbound>
    <on-error>
      <!-- statements to be applied if there is an error condition go here -->
    </on-error>
  </policies>
  ```
- Advanced policies can be created using this documentation https://docs.microsoft.com/en-us/learn/modules/explore-api-management/5-create-advanced-policies. Here are their examples:
  - Control flow: Conditionally applies policy statements based on the results of the evaluation of Boolean expressions.
    ``` xml
    <choose>
        <when condition="Boolean expression | Boolean constant">
            <!— one or more policy statements to be applied if the above condition is true  -->
        </when>
        <when condition="Boolean expression | Boolean constant">
            <!— one or more policy statements to be applied if the above condition is true  -->
        </when>
        <otherwise>
            <!— one or more policy statements to be applied if none of the above conditions are true  -->
      </otherwise>
    </choose>
    ```
  - Forward request: Forwards the request to the backend service.
    ``` xml
    <forward-request timeout="time in seconds" follow-redirects="true | false"/>
    ```
  - Limit concurrency: Prevents enclosed policies from executing by more than than the specified number of requests at a time.
    ``` xml
    <limit-concurrency key="expression" max-count="number">
      <!-- nested policy statements -->
    </limit-concurrency>
    ```
  - Log to Event Hub: Sends messages in the specified format to an Event Hub defined by a Logger entity.
    ``` xml
    <log-to-eventhub logger-id="id of the logger entity" partition-id="index of the partition where messages are sent" partition-key="value used for partition assignment">
      Expression returning a string to be logged
    </log-to-eventhub>
    ```
  - Mock response: Aborts pipeline execution and returns a mocked response directly to the caller.
    ``` xml
    <mock-response status-code="code" content-type="media type"/>
    ```
  - Retry: Retries execution of the enclosed policy statements if and until the condition is met. Execution will repeat at the specified time intervals and up to the specified retry count.
    ``` xml
    <retry
        condition="boolean expression or literal"
        count="number of retry attempts"
        interval="retry interval in seconds"
        max-interval="maximum retry interval in seconds"
        delta="retry interval delta in seconds"
        first-fast-retry="boolean expression or literal">
            <!-- One or more child policies. No restrictions -->
    </retry>
    ```
  - Return response: Aborts pipeline execution and returns either a default or custom response to the caller. Default = 200 OK with no body. Custom = can be specified via a context variable or policy statements (when both are provided the response contained within the context variable is modified by the policy statements before being returned to the caller).
    ``` xml
    <return-response response-variable-name="existing context variable">
      <set-header/>
      <set-body/>
      <set-status/>
    </return-response>
    ```
  - IP filtering: This isn't mentioned on the page but was noted in the exam prep video. IPs can be filtered using the following policy:
    ``` xml
    <ip-filter action="allow | forbid">
        <address>address</address>
        <address-range from="address" to="address" />
    </ip-filter>
    ```
    - One address or address-range element is required, but both can be used.
    - This can be defined as an inbound policy or at any of the following policy scopes (global, product, API, or operation).

## Securing APIs through subscriptions
- APIs can be secured by using subscription keys. Requests sent to consume the API must include a valid subscription key in HTTP request headers or they will be rejected by the APIM gateway.
- Subscription keys are generated through subscriptions, which are named containers for a pair of subscription keys.
- Subscriptions give granular control over permissions and policies. The three main scopes for subscriptions are the following:
  - All APIs - Applies to every API accessible from the gateway
  - Single API - Applies to a single imported API and its endpoints
  - Product - A collection of one or more APIs. The products can have different access rules, usage quotas, and terms of use.
- Subscriptions include a primary and secondary key. The keys can be regenerated independently of each other.
- Subscription keys can be sent through the default header of Ocp-Apim-Subscription-Key or through a query parameter of subscription-key.
- Subscriptions can be created from the Subscriptions blade from the APIM instance.

## Securing APIs through certificates
- Certs can be used to provide TLS mutual authentication between the client and API gateway with properties like the following
  - Only allow certs signed by a particular Certificate Authority (CA)
  - The API gateway can be configured to allow only requests with certs containing a certain thumbprint
  - Only allow certs with a specified subject
  - Only allow certs that have not expired
  - These properties can be mixed together through policy requirements and are not mutually exclusive
- The Consumption tier of APIM needs to explicitly enable the use of client certificates. This is done through the "Custom domains" blade on the APIM instance.
- Certs can be validated using the following code examples:
  - Checking that the thumbprint of the cert passed in the request matches the one issued by the certificate authority:
    ``` xml
    <choose>
        <when condition="@(context.Request.Certificate == null || context.Request.Certificate.Thumbprint != "desired-thumbprint")" >
            <return-response>
                <set-status code="403" reason="Invalid client certificate" />
            </return-response>
        </when>
    </choose>
    ```
  - Checking the thumbprint against certs uploaded to the APIM instance (the previous example only supported one cert, this will allow you to use multiple certs from different partners):
    ``` xml
    <choose>
        <when condition="@(context.Request.Certificate == null || !context.Request.Certificate.Verify()  || !context.Deployment.Certificates.Any(c => c.Value.Thumbprint == context.Request.Certificate.Thumbprint))" >
            <return-response>
                <set-status code="403" reason="Invalid client certificate" />
            </return-response>
        </when>
    </choose>
    ```
  - Check the issuer and subject of a client certificate:
    ``` xml
    <choose>
        <when condition="@(context.Request.Certificate == null || context.Request.Certificate.Issuer != "trusted-issuer" || context.Request.Certificate.SubjectName.Name != "expected-subject-name")" >
            <return-response>
                <set-status code="403" reason="Invalid client certificate" />
            </return-response>
        </when>
    </choose>
    ```
## Creating a backend API with APIM
1. Create a resource group and APIM instance using an environment variable for your email (to avoid publishing my email to Github)
``` bash
myEmail=<myEmail>

az group create --name az204-apim-patrick-rg --location eastus

az apim create -n az204-apim-patrick-426 \
    --location eastus \
    --publisher-email $myEmail \
    --resource-group az204-apim-patrick-rg \
    --publisher-name az204-apim-exercise-patrick \
    --sku-name Consumption
```
2. Navigate to the APIM instance that you created in the Azure portal
3. Click APIs
4. Click OpenAPI
5. Click Full
6. Specify the following values:
   1. OpenAPI Specification - https://conferenceapi.azurewebsites.net?format=json
   2. Display name - Demo Conference API
   3. Name - demo-conference-api
   4. Description - Automatically populated
   5. API URL suffix - conference
7. Click Create
8. Click the Settings tab
9. Modify the following settings
   1.  Web service URL - https://conferenceapi.azurewebsites.net/
   2.  Uncheck Subscription required
10. Click Save
11. Click the Test tab
12. Click GetSpeakers
    1.  Note that the page shown after clicking shows query parameters and headers. Note that the Ocp-Apim-Subscription-Key header is filled in automatically.
13. Click Send
14. Examine the 200 OK backend response
15. Clean up the APIM instance by deleting the resource group
``` bash
az group delete --name az204-apim-patrick-rg --no-wait
```


# Additional API Management Resources
https://docs.microsoft.com/en-us/azure/api-management/api-management-policies

https://docs.microsoft.com/en-us/azure/api-management/api-management-error-handling-policies


# Studying from Udemy (Section 12)
## Background work
- This uses the [Code/Visual Studio Projects/UdemyProductApi](Code/Visual%20Studio%20Projects/UdemyProductApi/) project (which is a copy of UdemyAuthApi with the authorization removed).
  - Remember that this is accessed at /api/<method name minus things like Get>. You can run the project locally to bring up the Swagger interface to determine endpoints as well.

## Creating an API Management Instance
1. Search for API Management in the marketplace
2. Click "Create"
3. Fill out the required details
4. Select the Developer pricing tier (note that this has a cost per hour)
5. Leave the other settings as their defaults
6. Create the resource (this may take a while)

## Publishing an API
1. Click on the "API" blade in the API Management resource
2. Click "HTTP"
3. Provide the name of the API
4. Provide the URL of the API
5. Click "Create"

## Adding Operations to an API (follow the previous steps to create the API)
1. Navigate to the API
2. Click "+ Add operation"
3. Give the operation a name (for our API a suitable one would be "Get Products")
4. Give the URL for the operation (continuing our example we would use "/api/Product")
5. Click "Save"
6. This can be tested by clicking on the operation and then clicking the "Test" option near the top
   1. Calling the API from Postman requires a header (Ocp-Apim-Subscription-Key) whose value can be retrieved from the "Subscriptions" blade. The key is in the "Built-in all-access subscription" key.

## Only allow access to your API from the API Management instance
1. Navigate to the web app that is hosting your API (UdemyProductApi for me)
2. Click on the "Networking" blade
3. Click on "Access restriction"
4. Click "+ Add rule"
5. Give the rule the following properties
   1. Priority: 200
   2. Name: AllowApiManagement
   3. Source: <Public IP from API management resource>/32
   4. Action: Allow
6. Create another rule with the following properties
   1. Priority: 300
   2. Name: BlockOtherTraffic
   3. Source: 0.0.0.0/0
   4. Action: Deny
7. These can be removed if you don't want to keep using them, it's just for demonstration

## Publishing an API using Swagger/OpenAPI
1. Delete the API you created earlier in the "Publishing an API" section
2. Launch the UdemyProductApi you created earlier locally
3. Click on the link to the swagger.json file in the browser
4. Click on the OpenAPI button in the "API" blade of the API Management resource
5. Select the swagger.json file you just downloaded
6. Click on "Create"
7. Get the URL of the service from your web app
8. Click on one of the operations and update the "Web service URL" setting with the URL
9. Note that the URL was updated in all of your operations in the API

## API management policies
- Policies can control how your users interact with APIs through your APIM instance
- You can do things like filter calls by IP, limit calls by keys or subscriptions, set usage quotas, and more
- Policies use XML to define their capabilities with expressions to extend their functionality
  - Expressions can be written in C#
- Policies have different sections:
  - Inbound - Changes the request before it reaches the API management internal service
  - Backend - Manipulates the request before it reaches the backend service
  - Outbound - Modifies the response sent to the user
  - On-error - What happens if any kind of error (timeout, authorization, etc.) occurs

### Rewrite URL Policy
This is a policy that rewrites requests sent to /api/Product with a query parameter like ?id=1 to their expected form of /api/Product/1
1. Navigate to the API
2. Under "Inbound Processing" open the code editor
3. In the <inbound> section add this line: `<set-variable name="id" value="@(context.Request.Url.Query.GetValueOrDefault("id"))" />`
   1. This line sets a variable with the ID query parameter stored so that we can use it in another part of the policy
4. After the set-variable block add this line: `<rewrite-uri template="@{ return "/api/product/" + context.Variables.GetValueOrDefault<string>("id");}" />`
   1. This rewrites the URL in the form that our backend web service expects

### Caching
- Your APIM instance can send requests to backend services which will end up retrieving data to return from a database or some other data store. APIM has caching options based on the SKU that can be enabled.
- APIM supports an internal cache based on SKU
  - All tiers aside from consumption have caching available. Consumption only supports external caches.
- APIM supports external caches like Azure Cache for Redis
- Caching can be varied based on the developer, developer groups, or by headers.
- Caching can be applied based on keys that include user defined variables.
  - APIM can also be configured with various policies to send requests to other resources if needed and then cache the values.
- Caching can be applied to an endpoint with the following policies:
  - Inbound: `<cache-lookup vary-by-developer="false" vary-by-developer-groups="false" allow-private-response-caching="true" must-revalidate="true" downstream-caching-type="none" caching-type="internal" />`
  - Outbound: `<cache-store duration="60" />`

### OAuth
- OAuth can be implemented for our product API using the following steps:
1. Navigate to AAD
2. Click on the "App registrations" blade
3. Click "+ New Registration"
4. Give the registration a name (UdemyProductApi)
5. Click "Register"
6. Click on the "Manifest" blade
7. Change the "accessTokenAcceptedVersion" value to 2
8. Click "Save"
9. Click on the "Expose an API" blade
10. Click "+ Add a scope"
11. Click "Save and continue"
12. Give the scope a name (Products)
13. Fill out the other required fields
14. Click "Add scope"
15. Navigate to the APIM instance
16. Click on the "OAuth 2.0 + OpenID Connect" blade
17. Click "+ Add"
18. Enter a name (Product API)
19. Set the client registration page to http://localhost
20. Select the "Authorization code" and "Client credentials" (client credentials for Postman) grant types
21. Navigate to the app registration in AAD for the OAuth 2.0 endpoint
22. Click on "Endpoints"
23. Copy the "OAuth 2.0 authorization endpoint (v2)" endpoint
24. Provide the endpoint in the "Authorization endpoint URL" field in APIM
25. Enable POST requests for the authorization endpoint
26. Copy the "OAuth 2.0 token endpoint (v2)" endpoint from AAD
27. Provide the endpoint in the "Token endpoint URL" field in APIM
28. Navigate to the "Expose an API" blade in AAD from your app registration
29. Copy the "Application ID URI"
30. Paste the URI in the "Default scope" field in APIM
31. Navigate to the "Certificates & secrets" blade in AAD from your app registration
32. Click "+ New client secret"
33. Give it a description and create it
34. Copy the value
35. Paste it into the "Client secret" field in APIM
36. Navigate to the "Overview" blade in AAD from your app registration
37. Copy the "Application (client) ID"
38. Paste it into the "Client ID" field in APIM
39. Copy the "Authorization code grant flow" URI from APIM
40. Navigate to the "Authentication" blade in AAD from your app registration
41. Click "+ Add a platform"
42. Click "Web"
43. Paste the URI in the "Redirect URIs" field
44. Allow access tokens and ID tokens
45. Click "Configure"
46. In APIM click on "Create"
47. Navigate to the Product API in the "APIs" blade in APIM
48. Click on "Settings"
49. Uncheck "Subscription required"
50. Enable "OAuth 2.0" for user authorization
51. Select the Product API server that you just created for the "OAuth 2.0 server" field
52. Click "Save"

The steps above were required to setup OAuth, but there's still more work to be done inside APIM with policies.
1. Navigate to the product API in APIM
2. Bring up the policy editor
3. Add the following inbound policy
``` xml
<validate-jwt header-name="Authorization" failed-validation-httpcode="401" failed-validation-error-message="Unauthorized" require-expiration-time="true" require-scheme="Bearer" require-signed-tokens="true" clock-skew="0">
  <!-- This URL can be retrieved from the app registration in AAD. Specifically the "OpenID Connect metadata document" endpoint. -->
  <openid-config url="https://login.microsoftonline.com/<tenant ID>/v2.0/.well-known/openid-configuration" />
  <required-claims>
    <!-- This is the audience claim -->
    <claim name="aud">
      <value>Application (client) ID from app registration</value>
    </claim>
  </required-claims>
</validate-jwt>
```
4. Click "Save"

Using this from code will require something similar to the [Code/Visual Studio Projects/UdemyAuthApp](Code/Visual%20Studio%20Projects/UdemyAuthApp/) project. The section 12 resources contain an application (in the API Management folder) that can be used for this.
1. Create a new app registration (apim product api caller)
2. Add a new client secret to the app registration
3. Copy the client secret and put it in the "ClientSecret" value in the appsettings.json file
4. Copy the tenant and client IDs and put them in the appsettings.json file as well
5. Add authentication to the app registration and give it https://localhost:7046/signin-oidc and https://localhost:7046/signout-oidc as URLs.
6. Allow access tokens and ID tokens for authentication
7. Grant the app registration permission to use the API through the "API permissions" blade
8. In the program update the scope[] array in the index page class and the Program.cs class with the scopes URI from the "Expose an API" blade in the app registration for the product API itself (not the new registration)
9. Update the apiUrl value in the index page class with the URL appropriate for your APIM instance
10. Run the program and verify that it was able to retrieve products


### Virtual Networks
- We would use a VNet if we wanted to do something like expose our APIM instance publicly while not exposing our backend services.

# Misc
- See https://learn.microsoft.com/en-us/shows/exam-readiness-zone/preparing-for-az-204-connect-to-and-consume-azure-services-and-third-party-services-segment-5-of-5 for information on what may appear on the exam.
  - Be aware of how APIM fits in your broader architecture (it sits between your client apps and the services that they want to call)
  - Be aware of terminology:
    - Backend API: A HTTP service that you implement with your business logic
    - Frontend API: A HTTP service facade hosted by API Management to obfuscate your back-end API
    - Operation: A specific operation in the front-end API that correlates to a specific request/response from the backend API
    - Product: One or more APIs, along with a usage quota and terms of use
    - Subscription: Is a way for consumers to access an API using keys
      - Subscription key scopes:
        - All APIs - Applies to every API accessible from the gateway
        - Single API - Applies to a single imported API and all of its endpoints
        - Product - A collection of one or more APIs. Can have different access rules, usage quotas, and terms of use.
      - Securing APIs by using subscriptions
  - Be aware of various mechanisms for securing APIs [MS docs with multiple examples for securing APIs](https://learn.microsoft.com/en-us/azure/api-management/api-management-howto-protect-backend-with-aad):
    - OAuth 2.0
    - Subscriptions
    - Certificates
    - IP allow listing
  - Be aware of policies
    - Be familiar with where policies are applied and how they look in XML:
      ``` xml
      <policies>
        <inbound>
          <!-- statements to be applied to the request go here -->
        </inbound>
        <backend>
          <!-- statements to be applied before the request is forwarded to 
              the backend service go here -->
        </backend>
        <outbound>
          <!-- statements to be applied to the response go here -->
        </outbound>
        <on-error>
          <!-- statements to be applied if there is an error condition go here -->
        </on-error>
      </policies>
      ```

## Important Terminology [MS Docs](https://learn.microsoft.com/en-us/azure/api-management/api-management-terminology)
- Backend API - A service that implements an API and its operations.
- Frontend API - APIM serves as a mediation layer over the backend APIs. A Frontend API is an API that is exposed to API consumers from APIM.
- Product - A product is a bundle of frontend APIs that can be made available to a specified group of API consumers for self-service onboarding under a single access credential and set of usage limits. An API can be part of multiple products.
- API operation - A frontend API in APIM can define multiple operations. An operation is a combination of an HTTP verb and a URL template uniquely resolvable within the frontend API. Often operations map one-to-one to backend API endpoints.
- Version - A version is a distinct variant of an existing frontend API that differs in shape or behavior from the original. Versions give customers the ability to stick with an original API or upgrade at a time that they choose. Versions are a mechanism for releasing breaking changes without impacting API consumers.
- Revision - A copy of an existing API that can be changed without impacting API consumers and swapped with the version currently in use by consumers (usually after validation and testing). Revisions provide a mechanism for safely implementing nonbreaking changes.
- Policy - A policy is a reusable and composable component that implements common API-related functions.
- Developer portal - A component of APIM that provides for API discovery and self-service onboarding for API consumers.