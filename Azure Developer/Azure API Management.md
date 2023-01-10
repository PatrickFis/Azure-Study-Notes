# API Management
API Management helps organizations publish APIs to external, partner, and internal developers to unlock the potential of their data and services.

## Features
Made of the following components
- API gateway
  - Accepts API calls and routes them
  - Verifies API keys, JWT tokens, certs, and more
  - Enforces usage quotes and rate limits
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
- Devs - Authenticated developer portal users. Devs are the customers thatbulid apps using your APIs.
- Guests - Unauthenticated developer portal users. Can be granted read-only access.
- Custom groups can be created
- External groups in AAD can be used

## Policies
- Policies allow APIM to change the behavior of an API through configuration. Policies are a collection of statements that are executed sequentially on the request or response of an API. Popular statements include format conversion and call rate limiting.
- Policies are defined through XML files that describe a sequence of inbound and outbound statements. The configuration is divided into inbound, backend, outbound, and on-error.
- Advanced policies can be created using this documentation https://docs.microsoft.com/en-us/learn/modules/explore-api-management/5-create-advanced-policies.

## Securing APIs through subscriptions
- APIs can be secured by using subscription keys. Requests sent to consume the API must include a valid subscription key in HTTP request headers or they will be rejected by the APIM gateway.
- Subscription keys are generated through subscriptions, which are named containers for a pair of subscription keys.
- Subscriptions give granular control over permissions and policies. The three main scopes for subscriptions are the following
  - All APIs - Applies to every API accessible from the gateway
  - Single API - Applies to a single imported API and its endpoints
  - Product - A collection of one or more APIs. The products can have different access rules, usage quotas, and terms of use.
- Subscriptions include a primary and secondary key. The keys can be regenerated independently of each other.
- Subscription keys can be sent through the default header of Ocp-Apim-Subscription-Key or through a query parameter of subscription-key

## Securing APIs through certificates
- Certs can be used to provide TLS mutual authentication between the client and API gateway with properties like the following
  - Only allow certs signed by a particular Certificate Authority (CA)
  - The API gateway can be configured to allow only requests with certs containing a certain thumbprint
  - Only allow certs with a specified subject
  - Only allow certs that have not expired
  - These properties can be mixed together through policy requirements and are not mutually exclusive

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