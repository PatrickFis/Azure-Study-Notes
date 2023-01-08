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

Groups can be managed through AAD.

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