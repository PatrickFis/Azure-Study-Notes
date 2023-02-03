# Microsoft Identity Platform
The Microsoft identity platform for developers is a set of tools which includes authentication service, open-source libraries, and application management tools. The platform is made of several components:
- OAuth 2.0 and OpenID Connect standard-compliant authentication service
- Open-source libraries
- Application management portal for registration, configuration, and management
- Application configuration API and PowerShell
- Applications must be registered in an AAD tenant to use identity and access management functions

## Service Principals
https://docs.microsoft.com/en-us/learn/modules/explore-microsoft-identity-platform/3-app-service-principals

Service principals are automatically created when an app is registered. Accessing resources secured by AAD requires that the entity requesting access be represented by a security principal (users = user principal, apps = service principal). There are three types of service principal
- Application - The type of service principal is the local representation, or application instance, of a global application object in a single tenant or directory. A service principal is created in each tenant where the application is used and references the globally unique app object. The service principal object defines what the app can actually do in the specific tenant, who can access the app, and what resources the app can access.
- Managed identity - This type of service principal is used to represent a managed identity. Managed identities provide an identity for applications to use when connecting to resources that support Azure Active Directory authentication. When a managed identity is enabled, a service principal representing that managed identity is created in your tenant. Service principals representing managed identities can be granted access and permissions, but cannot be updated or modified directly.
- Legacy - This type of service principal represents a legacy app, which is an app created before app registrations were introduced or an app created through legacy experiences. A legacy service principal can have credentials, service principal names, reply URLs, and other properties that an authorized user can edit, but does not have an associated app registration. The service principal can only be used in the tenant where it was created.

## Permissions and Consent (There's a lot of info in the linked docs, come back and summarize it later)
https://docs.microsoft.com/en-us/learn/modules/explore-microsoft-identity-platform/4-permission-consent

Two permission types
- Delegated - Used by apps that have signed-in users. Users or admins consent to the permissions that an app requests.
- Application - Used by apps without a signed-in user present. Only admins can consent to application permissions.

Consent types
- Static user consent
  - All permissions specified in the Azure portal
  - Admins can consent on behalf of all users in an org
  - Can lead to development issues
    - Long lists of permissions discourage people from granting access
    - App needs to know all of the resources it would ever access ahead of time
- Incremental and dynamic user consent
  - Minimum amount of permissions can be asked for during app registration with more requests coming over time for additional features
  - Can present challenges for permissions that require admin consent
- Admin consent
  - Used for access to certain high-privilege permissions
  - When done on behalf of an org it still requires static permissions registered for the app

## Conditional Access (Summarize later)
See SC-900 and https://docs.microsoft.com/en-us/learn/modules/explore-microsoft-identity-platform/5-conditional-access.


# Microsoft Authentication Library (MSAL)
MSAL is used by devs to acquire tokens from the Microsoft identity platform to authenticate users and access secured web APIs.

## Library Features
- Supports a number of different application architectures and platforms
- Handles tokens for you
  - The library can acquire tokens on behalf of a user or an application
  - The library can maintain a token cache and refresh tokens for you when they're close to expiring
- Helps troubleshoot
- Supports multiple authentication flows
  | Flow               | Description                                                                    |
  | ------------------ | ------------------------------------------------------------------------------ |
  | Authorization code | Native and web apps securely obtain tokens in the name of the user             |
  | Client credentials | Service applications run without user interaction                              |
  | On-behalf-of       | The application calls a service/web API, which in turns calls Microsoft Graph  |
  | Implicit           | Used in browser-based applications                                             |
  | Device code        | Enables sign-in to a device by using another device that has a browser         |
  | Integrated Windows | Windows computers silently acquire an access token when they are domain joined |
  | Interactive        | Mobile and desktops applications call Microsoft Graph in the name of a user    |
  | Username/password  | The application signs in a user by using their username and password           |

## Client Applications
- Public and confidential client applications are supported through application builders in the library.
  - Public client applications: Apps which run on devices or desktop computers or in a web browser. These apps aren't trusted to safely keep app secrets, so they only access APIs on behalf of the user (ie, they support only public client flows). Public clients can't hold configuration-time secrets, so they don't have client secrets.
  - Confidential client applications: Apps that run on servers (web apps, web API apps, or even service/daemon apps). Considered difficult to access and are able to keep an app secret. Can hold configuration time secrets.
  - https://learn.microsoft.com/en-us/training/modules/implement-authentication-by-using-microsoft-authentication-library/3-initialize-client-applications

### Code Samples for MSAL Client Applications
- A public client application instantiates itself with the following code:
  ``` c#
  IPublicClientApplication app = PublicClientApplicationBuilder.Create(clientId).Build();
  ```
- There are various other methods available for these builders like the following:
  ``` c#
  // WithAuthority sets the application default authority to an Azure AD authority or another value
  var clientApp = PublicClientApplicationBuilder.Create(client_id)
      .WithAuthority(AzureCloudInstance.AzurePublic, tenant_id)
      .Build();

  // WithRedirectUri overrides the default redirect URI
  var clientApp = PublicClientApplicationBuilder.Create(client_id)
    .WithAuthority(AzureCloudInstance.AzurePublic, tenant_id)
    .WithRedirectUri("http://localhost")
    .Build();
  ```
- A confidential application (in this case: a web app located at https://myapp.azurewebsites.net) is instantiated using the following code:
  ``` c#
  string redirectUri = "https://myapp.azurewebsites.net";
  IConfidentialClientApplication app = ConfidentialClientApplicationBuilder.Create(clientId)
      .WithClientSecret(clientSecret)
      .WithRedirectUri(redirectUri)
      .Build();
  ```
- Modifiers that both public and confidential client apps can use are noted below:
  | Modifier                                            | Description                                                                                                                                                                                                                    |
  | --------------------------------------------------- | ------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------ |
  | .WithAuthority()                                    | Sets the application default authority to an Azure Active Directory authority, with the possibility of choosing the Azure Cloud, the audience, the tenant (tenant ID or domain name), or providing directly the authority URI. |
  | .WithTenantId(string tenantId)                      | Overrides the tenant ID, or the tenant description.                                                                                                                                                                            |
  | .WithClientId(string)                               | Overrides the client ID.                                                                                                                                                                                                       |
  | .WithRedirectUri(string redirectUri)                | Overrides the default redirect URI. In the case of public client applications, this will be useful for scenarios requiring a broker.                                                                                           |
  | .WithComponent(string)                              | Sets the name of the library using MSAL.NET (for telemetry reasons).                                                                                                                                                           |
  | .WithDebugLoggingCallback()                         | If called, the application will call Debug.Write simply enabling debugging traces.                                                                                                                                             |
  | .WithLogging()                                      | If called, the application will call a callback with debugging traces.                                                                                                                                                         |
  | .WithTelemetry(TelemetryCallback telemetryCallback) | Sets the delegate used to send telemetry.                                                                                                                                                                                      |
- Modifiers that are specific to confidential client applications are noted below:
  | Modifier                                       | Description                                                                                    |
  | ---------------------------------------------- | ---------------------------------------------------------------------------------------------- |
  | .WithCertificate(X509Certificate2 certificate) | Sets the certificate identifying the application with Azure Active Directory.                  |
  | .WithClientSecret(string clientSecret)         | Sets the client secret (app password) identifying the application with Azure Active Directory. |

## Implementing Interactive Authentication
https://docs.microsoft.com/en-us/learn/modules/implement-authentication-by-using-microsoft-authentication-library/4-interactive-authentication-msal

1. Sign in to the Azure portal
2. Search for Azure Active Directory (don't create a new one, just use the default directory)
3. Under Manage select App registrations
4. Select New registration
5. Fill in the details
6. Select Register
7. Create a new application as specified in the above link
8. Implement the code sample noted below (note that it has a dependency on the Microsoft.Identity.Client package)

``` c#
using System;
using System.Threading.Tasks;
using Microsoft.Identity.Client;

namespace az204_auth
{
    class Program
    {
        private const string _clientId = "APPLICATION_CLIENT_ID";
        private const string _tenantId = "DIRECTORY_TENANT_ID";

        public static async Task Main(string[] args)
        {
            var app = PublicClientApplicationBuilder
                .Create(_clientId)
                .WithAuthority(AzureCloudInstance.AzurePublic, _tenantId)
                .WithRedirectUri("http://localhost")
                .Build(); 
            string[] scopes = { "user.read" };
            AuthenticationResult result = await app.AcquireTokenInteractive(scopes).ExecuteAsync();

            Console.WriteLine($"Token:\t{result.AccessToken}");
        }
    }
}
```

# Shared Access Signatures (SAS)
A SAS is a URI that grants restricted access rights to Azure Storage resources.

## Information
- SAS points to one or more storage resources and includes a token that contains a special set of query parameters

There are three types of SAS
- User delegation SAS - Secured by AAD and by permissions specified for the SAS. Applies only to Blob storage.
- Service SAS - Secured with the storage account key. Delegates access to an Azure Storage resource (Blob, Queue, Table, or Files).
- Account SAS - Secured with the storage account key. Delegates access to resources in one or more of the storage services. All actions available to the above two SAS are available to this one.

SAS tokens are made of several components
- The URI of the resource
- A parameter indicating the access rights
- The date the access starts and ends
- The version of the storage API to use
- The kind of storage being accessed
- The cryptographic signature

## When to use SAS
- Use when a client who wouldn't have access to a resource needs access
- Commonly used for users reading and writing their own data into your storage account

## Stored Access Policies
- Stored access policies provide an additional level of control over SAS
- Allows for additional restrictions to be placed on SAS
  - Can change start time, expiry time, permissions for a signature, or revoke it after it has been issued


# Microsoft Graph (Re-review this module)
Microsoft Graph is used to retrieve data from Microsoft 365, Windows 10, and Enterprise Mobility + Security.

## REST Access
- Resources can be accessed using REST
  - Supports GET, POST, PATCH, PUT, and DELETE
  - Has a v1.0 and beta version
  - Resources are included in the URL you use to access the API
  - Query parameters can be OData system query options or other strings that a method accepts to customize its response

## SDK Usage
There are a variety of SDKs available to query Microsoft Graph.


# Studying from Youtube [video here](https://www.youtube.com/watch?v=gb5yU0Gh9Kg&list=PLLc2nQDXYMHpekgrToMrDpVtFtvmRSqVt&index=15)
## Implementing OAuth in App Service
[Github](https://github.com/Azure-Samples/active-directory-aspnetcore-webapp-openidconnect-v2/tree/master/1-WebApp-OIDC) has a number of OAuth samples.

1. Install two dependencies: Microsoft.Identity.Web and Microsoft.Identity.Web.UI
2. Create a new app registration in AAD
3. Add the following to your appsettings.json file (note: AzureAD can be whatever name you want):
``` json
  "AzureAD": {
    "Instance": "https://login.microsoft.com/",
    "TenantID": "replace me",
    "ClientID": "replace me",
    "CallbackPath": "/signin-oidc",
    "SignedOutCallbackPath": "/signout-oidc"
  }
```
4. In AAD go to the Authentication section of the app registration
5. Click Add a platform and select Web
6. Enter https://localhost:<port your app is listening on>/signin-oidc in the redirect URI input (can retrieve these values from launchSettings.json), and https://localhost:<port your app is listening on>/signout-oidc in the logout URL
7. Check the ID tokens checkbox
8. Follow along using the video or the sample code at https://github.com/Azure-Samples/active-directory-aspnetcore-webapp-openidconnect-v2/tree/master/1-WebApp-OIDC/1-1-MyOrg to get your application running and using OIDC

# Udemy Notes (Section 9 - Split with Secure Cloud Solutions)

## Azure AD
- Identity provider in Azure
- Helps authenticate users and authorize them to use Azure resources
- Can be used as an identity provider for Azure, Microsoft 365, and other SaaS products
- Can manage identities of users, groups, and apps as well as their security aspects
- 3 Tiers (see security fundamentals)
  - Free: User and group management
  - Premium P1: Dynamic groups, hybrid identities, self-service password reset for on-prem users
  - Premium P2: AAD Identity Protection and Privileged Identity Management

### Creating a user in AAD and using them
- AAD can be found in the portal by navigating to Azure Active Directory
1. Under the Manage section of AAD click on the Users blade
   1. This will take you to a page showing you all the users in your directory
2. Click the New user button
3. Give the user a username and name
4. Make a password or keep the auto generated one (by default new users must change their password anyway)
5. Note the username and password
6. Click on Create
7. You'll be taken back to the page noted in step 1. The user should appear fairly quickly (if it hasn't just click refresh a few times)
8. After creating the account you can sign in to Azure with the new account
9. Change the password and note it

When you get into Azure you'll see the portal but none of your resources. By default new users are given no access to resources in your subscription.

### RBAC
- Role-based access control provides a mechanism to control access to your resources
- Azure includes a number of built in roles that can be found at https://learn.microsoft.com/en-us/azure/role-based-access-control/built-in-roles. Some of the important ones are these:
  - Contributor - Access to manage resources, but can't assign RBAC roles, manage assignments in Azure Blueprints, or share image galleries.
  - Owner - Full access to everything.
  - Reader - View all resources but make no changes.
  - User Access Administrator - Manage user access to Azure resources.
- Custom roles can also be defined.

The following steps show you how to assign RBAC roles to the user you created in the notes above.
1. Navigate to a resource that you'd like to give the user access to (for example: a storage account)
2. Click on the Access Control (IAM) blade
3. Click on Add
4. Click on Add role assignment
5. Select a role (for example: Reader)
6. Keep the "Assign access to" setting as "User, group, or service principal"
7. Click Select members
8. Select the account you created
9. Click Select
10. Click Next
11. Click Review + assign
12. Wait for a couple of minutes for the role to take effect
13. Navigate to all resources on the new account
14. Verify that you can get to the resource you granted it access to and that the role works as expected

# Udemy Notes (Section 10)

## Authentication and Authorization
- AuthN - The process where you prove you are who you say you are
- AuthZ - The process of granting access to perform an action

## OAuth 2.0
- OAuth 2.0 is the industry standard protocol for authorization
- We'll be looking at the authorization code flow

### Authorization Code Flow
- The authorization code grant is used when an app needs to exchange an authorization code for an access token
- The following components are involved in the flow that we're looking at:
  - A user who has been granted access to a resource
  - A client which requests access to the protected resource (in our case a web app)
  - The resource server which grants access to a protected resource (for example: a storage account)
  - An authorization server (the MS Identity Platform for Azure) which manges end-user's information, their access, and also issues access tokens
- Steps in the flow:
  1. The application makes a call to the authZ server with a redirect URI for the authZ server to send a response to
  2. The authZ server sends an authorization code to the app (the app will need to use this code to get an access token)
  3. The app requests an access token which will have all the permissions of the user
  4. The app will ask the resource server for access to the resource

## Authentication Lab
- A new ASP.NET Web App was created for this. The code is in [Code/Visual Studio Projects/UdemyAuthApp](Code/Visual%20Studio%20Projects/UdemyAuthApp/).
- A new app registration was created in AAD for this.
  - A new platform had to be added to this with the following steps
      1. Open the app registration in AAD
      2. Go to the Authentication blade
      3. Click Add a platform
      4. Select Web
      5. Enter the redirect URI for your application (https://localhost:7144/signin-oidc for mine, see launchSettings.json for the port) and the logout URL (https://localhost:7144/signout-oidc)
      6. Check ID tokens
      7. Click Configure
- Information about AAD was stored in appsettings.json to tell the app where to send requests to for authentication as well as the client ID and tenant ID of the app registration.
- The Microsoft.Identity.Web package was installed for this.
- After configuring your app registration and platform and adding code to support authentication you will be taken to a login page where you're asked to grant permissions to the application. This is because the app registration has the User.Read Delegated permission by default.
  - The first time the app is run afterwards you will get an error as you need to configure a callback path and a sign out callback path as well to your appsettings.json file and the platform in your app registration.
- The Microsoft.Identity.Web.UI package was installed to add a UI for login.
- The Identity scaffolded item was added to make use of builtin resources. Account\Login and Account\Logout were overriden, a data context class was created, and SQLite was used instead of SQL Server.
- To demonstrate group claims a new group was made in AAD with the following steps:
  1. Navigate to AAD
  2. Click on the Groups blade
  3. Click New group
  4. Leave the Group type as Security
  5. Give the group a name
  6. Put a member in the group
  7. Create the group
- The app registration was also updated to retrieve these claims:
  1. Navigate to the app registration
  2. Click on the Token configuration blade
  3. Click Add groups claim
  4. Check Security groups
  5. Click Add
- After signing in to your application again you should be able to see group claims (claims appear to be cached so logging in again is required - https://stackoverflow.com/questions/73730123/unable-to-get-azure-ad-groups-to-appear-in-user-claims).
- Optional claims where also added to show the user's email and given name. They require similar steps as adding group claims above as well as allowing Azure to add some Microsoft Graph permissions to the app (specifically email and profile).

## Authorization Lab
- The code for this is in [Code/Visual Studio Projects/UdemyAuthApp](Code/Visual%20Studio%20Projects/UdemyAuthApp/).
- A test account was given the reader and storage blob data reader roles in my az204patrickstorage storage account.
- The app registration made for the previous section was updated and given the user_impersonation permission for Azure Storage. Note that this is a delegated permission and that the interface tells you the scope (in this case https://storage.azure.com/user_impersonation) of the permission so that you can use it in your code. The Authentication blade was also updated: Access tokens were enabled.
  - I had some issues with the code used in this lab to retrieve access tokens. I had to navigate to https://localhost:7144/signout-oidc and then sign back in before it would start working for me.

## API Lab
- The code for this is in [Code/Visual Studio Projects/UdemyAuthApi](Code/Visual%20Studio%20Projects/UdemyAuthApi/).
  - This reuses the code set up in [Code/Visual Studio Projects/UdemyAzureFunction](Code/Visual%20Studio%20Projects/UdemyAzureFunction/) for connecting to a SQL database and retrieving results.
  - The Microsoft.Identity.Web package was installed so that authentication and authorization could be required.
- When publishing to a web app in Azure you'll need to set up a connection string named "SQLConnectionString" with SQLAzure as the type (in appsettings.json or launchSettings.json the environment variable is SQLAZURECONNSTR_SQLConnectionString).
- A new app registration called UdemyAuthApi was created for this.
  - In the app registration's manifest the accessTokenAcceptedVersion property was updated from null to 2 to have it use Oauth 2.0.
  - A new app role was added to UdemyAuthApi to allow requests from Postman.
  - The Postman app registration's API permissions were updated: a new permission was added to access the UdemyAuthApi with the following steps:
      1. Navigate to the app registration
      2. Click API permissions
      3. Click Add a permission
      4. Click My APIs
      5. Click UdemyAuthApi
      6. Select the ProductAccess permission
      7. Click Add permissions
      8. Click Grant admin consent for the Default Directory


# Claims in an ID Token [MS Link](https://learn.microsoft.com/en-us/azure/active-directory/develop/id-tokens)
- ID tokens are JSON web tokens (JWT)
  - Consists of a header, payload, and signature
  - The header and signature are used to verify the authenticity of the token
  - The payload contains the information about the user requested by your client

## Header claims
| Claim | Format                | Description                                                                                                                               |     |     |
| ----- | --------------------- | ----------------------------------------------------------------------------------------------------------------------------------------- | --- | --- |
| typ   | String - always "JWT" | Indicates that the token is a JWT token.                                                                                                  |     |     |
| alg   | String                | Indicates the algorithm that was used to sign the token. Example: "RS256"                                                                 |     |     |
| kid   | String                | Specifies the thumbprint for the public key that can be used to validate this token's signature. Emitted in both v1.0 and v2.0 ID tokens. |     |     |
| x5t   | String                | Functions the same (in use and value) as kid. x5t is a legacy claim emitted only in v1.0 ID tokens for compatibility purposes.            |     |     |

## Payload claims
| Claim              | Format                     | Description                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                            |
| ------------------ | -------------------------- | ------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------ |
| aud                | String, an App ID GUID     | Identifies the intended recipient of the token. In id_tokens, the audience is your app's Application ID, assigned to your app in the Azure portal. This value should be validated. The token should be rejected if it fails to match your app's Application ID.                                                                                                                                                                                                                                                                                                                                                                                                                                                                                        |
| iss                | String, an issuer URI      | Identifies the issuer, or "authorization server" that constructs and returns the token. It also identifies the Azure AD tenant for which the user was authenticated. If the token was issued by the v2.0 endpoint, the URI will end in /v2.0. The GUID that indicates that the user is a consumer user from a Microsoft account is 9188040d-6c67-4c5b-b112-36a304b66dad. Your app should use the GUID portion of the claim to restrict the set of tenants that can sign in to the app, if applicable.                                                                                                                                                                                                                                                  |
| iat                | int, a Unix timestamp      | "Issued At" indicates when the authentication for this token occurred.                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                 |
| idp                | String, usually an STS URI | Records the identity provider that authenticated the subject of the token. This value is identical to the value of the Issuer claim unless the user account not in the same tenant as the issuer - guests, for instance. If the claim isn't present, it means that the value of iss can be used instead. For personal accounts being used in an organizational context (for instance, a personal account invited to an Azure AD tenant), the idp claim may be 'live.com' or an STS URI containing the Microsoft account tenant 9188040d-6c67-4c5b-b112-36a304b66dad.                                                                                                                                                                                   |
| nbf                | int, a Unix timestamp      | The "nbf" (not before) claim identifies the time before which the JWT MUST NOT be accepted for processing.                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                             |
| exp                | int, a Unix timestamp      | The "exp" (expiration time) claim identifies the expiration time on or after which the JWT must not be accepted for processing. It's important to note that in certain circumstances, a resource may reject the token before this time. For example, if a change in authentication is required or a token revocation has been detected.                                                                                                                                                                                                                                                                                                                                                                                                                |
| c_hash             | String                     | The code hash is included in ID tokens only when the ID token is issued with an OAuth 2.0 authorization code. It can be used to validate the authenticity of an authorization code. To understand how to do this validation, see the OpenID Connect specification.                                                                                                                                                                                                                                                                                                                                                                                                                                                                                     |
| at_hash            | String                     | The access token hash is included in ID tokens only when the ID token is issued from the /authorize endpoint with an OAuth 2.0 access token. It can be used to validate the authenticity of an access token. To understand how to do this validation, see the OpenID Connect specification. This is not returned on ID tokens from the /token endpoint.                                                                                                                                                                                                                                                                                                                                                                                                |
| aio                | Opaque String              | An internal claim used by Azure AD to record data for token reuse. Should be ignored.                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                  |
| preferred_username | String                     | The primary username that represents the user. It could be an email address, phone number, or a generic username without a specified format. Its value is mutable and might change over time. Since it is mutable, this value must not be used to make authorization decisions. It can be used for username hints, however, and in human-readable UI as a username. The profile scope is required in order to receive this claim. Present only in v2.0 tokens.                                                                                                                                                                                                                                                                                         |
| email              | String                     | The email claim is present by default for guest accounts that have an email address. Your app can request the email claim for managed users (those from the same tenant as the resource) using the email optional claim. On the v2.0 endpoint, your app can also request the email OpenID Connect scope - you don't need to request both the optional claim and the scope to get the claim.                                                                                                                                                                                                                                                                                                                                                            |
| name               | String                     | The name claim provides a human-readable value that identifies the subject of the token. The value isn't guaranteed to be unique, it can be changed, and it's designed to be used only for display purposes. The profile scope is required to receive this claim.                                                                                                                                                                                                                                                                                                                                                                                                                                                                                      |
| nonce              | String                     | The nonce matches the parameter included in the original /authorize request to the IDP. If it does not match, your application should reject the token.                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                |
| oid                | String, a GUID             | The immutable identifier for an object in the Microsoft identity system, in this case, a user account. This ID uniquely identifies the user across applications - two different applications signing in the same user will receive the same value in the oid claim. The Microsoft Graph will return this ID as the id property for a given user account. Because the oid allows multiple apps to correlate users, the profile scope is required to receive this claim. Note that if a single user exists in multiple tenants, the user will contain a different object ID in each tenant - they're considered different accounts, even though the user logs into each account with the same credentials. The oid claim is a GUID and cannot be reused. |
| roles              | Array of strings           | The set of roles that were assigned to the user who is logging in.                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                     |
| rh                 | Opaque String              | An internal claim used by Azure to revalidate tokens. Should be ignored.                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                               |
| sub                | String                     | The principal about which the token asserts information, such as the user of an app. This value is immutable and cannot be reassigned or reused. The subject is a pairwise identifier - it is unique to a particular application ID. If a single user signs into two different apps using two different client IDs, those apps will receive two different values for the subject claim. This may or may not be wanted depending on your architecture and privacy requirements.                                                                                                                                                                                                                                                                         |
| tid                | String, a GUID             | Represents the tenant that the user is signing in to. For work and school accounts, the GUID is the immutable tenant ID of the organization that the user is signing in to. For sign-ins to the personal Microsoft account tenant (services like Xbox, Teams for Life, or Outlook), the value is 9188040d-6c67-4c5b-b112-36a304b66dad.                                                                                                                                                                                                                                                                                                                                                                                                                 |
| unique_name        | String                     | Only present in v1.0 tokens. Provides a human readable value that identifies the subject of the token. This value is not guaranteed to be unique within a tenant and should be used only for display purposes.                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                         |
| uti                | String                     | Token identifier claim, equivalent to jti in the JWT specification. Unique, per-token identifier that is case-sensitive.                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                               |
| ver                | String, either 1.0 or 2.0  | Indicates the version of the id_token.                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                 |
| hasgroups          | Boolean                    | If present, always true, denoting the user is in at least one group. Used in place of the groups claim for JWTs in implicit grant flows if the full groups claim would extend the URI fragment beyond the URL length limits (currently 6 or more groups). Indicates that the client should use the Microsoft Graph API to determine the user's groups (https://graph.microsoft.com/v1.0/users/{userID}/getMemberObjects).                                                                                                                                                                                                                                                                                                                              |
| groups:src1        | JSON object                | For token requests that are not limited in length (see hasgroups above) but still too large for the token, a link to the full groups list for the user will be included. For JWTs as a distributed claim, for SAML as a new claim in place of the groups claim.  Example JWT Value: "groups":"src1" "_claim_sources: "src1" : { "endpoint" : "https://graph.microsoft.com/v1.0/users/{userID}/getMemberObjects" }  For more info, see Groups overage claim.                                                                                                                                                                                                                                                                                            |

# Misc
- See https://learn.microsoft.com/en-us/shows/exam-readiness-zone/preparing-for-az-204-implement-azure-security-segment-3-of-5 for information on what may appear on the exam.
  - The libraries used to interact with AAD and the registering applications to the identity platform
    - Be familiar with steps you'd go through in the portal to register an application, what's created when you register it, and what libraries you'd use to interface with it. Concentrate on the MSAL library as the main SDK.
    - Service principals
    - Application objects
  - Be aware of the two types of permissions
    - Delegated: Those assigned to a user
    - Application: Those for the app itself
  - Be aware of consent types
    - Static, incremental, dynamic, or admin consent
  - You may have to look at an OAuth HTTP request
    - Be aware of scopes and what permissions they give (look at the graph API for this)
  - Shared access signatures (SAS)
    - Be aware of the types (user delegation, service, or account)
    - Be aware of some of the best practices
      - Always use HTTPs
      - Apply minimum-required privileges
      - Set expiration time to smallest useful time
      - The most secure SAS is a user delegation SAS
      - SAS isn't always the correct solution
    - Understand the URI for a SAS and what they mean
      - sp = permissions
      - se = end time for signature
      - And more
  - Key Vault 
    - Secrets, keys, and cert management
    - Accessing Key Vault using service principals and managed identities
      - Be aware of how a service principal may use a secret or certificate for access
      - Be aware of the following best practices:
        - How to enable logging for Key Vaults
        - Use separate key vaults per application per environment
        - Backup and recovery options
        - Access control
    - Be aware of how to establish a managed identity for a web app or VM and how to give it permissions
    - Understand the differences between a system assigned managed identity and a user assigned identity
  - App Configuration
    - Be aware of how secrets in a Key Vault can be referenced from App Config 
  - Be aware of private endpoints
    - Doesn't allow direct access from the outside world. You have to go through your virtual networks to get to the App Config
  - Microsoft Graph
    - Be aware of HTTP access and what an HTTP request may look like
      - Can be accessed by using a token retrieved from MS. The request looks like this (assumes you're using an app registration from AAD):
        - POST to https://login.microsoftonline.com/tenant-id-here/oauth2/v2.0/token
        - Headers:
          - grant_type: client_credentials
          - client_id: client ID from app registration
          - client_secret: generated secret from app registration
          - scope: https://graph.microsoft.com/.default
        - This will return a response containing an access_token field which can be set in an Authorization header on other requests to the Graph API. For example, the token could be used to send requests to https://graph.microsoft.com/v1.0/users to retrieve information about users.
          - Note: If you're accessed on the exam about the Authorization header, then the header itself is named "Authorization" and the value is "Bearer `<token here>`"
    - Be aware of the Microsoft Graph library and using it with MSAL
  - Key points to review
    - Difference between AuthN and AuthZ
    - Service principals and RBAC
    - Registering an application
    - Security mechanisms like SAS, SSL certs and encryption
    - Azure Key Vault
    - Managed Identities

# Code Samples
## Managed Identities
- This code is from [Code/Visual Studio Projects/UdemyKeyVault](Code/Visual%20Studio%20Projects/UdemyKeyVault/UdemyKeyVault/Program.cs)
### Retrieve a token for interacting with a resource
- This code assumes that it's being run on a resource with a managed identity (like a VM or a web app).
- It requires the identity to have access to read secrets in a Key Vault and uses the identity to retrieve the value of a secret.
- DefaultAzureCredential is used to automatically get a token using the managed identity.
``` c#
async Task GetSecretUsingManagedIdentity()
{
    TokenCredential tokenCredential = new DefaultAzureCredential();
    SecretClient secretClient = new(new Uri(keyVaultUri), tokenCredential);

    var secret = await secretClient.GetSecretAsync(secretName);

    Console.WriteLine($"The secret is {secret.Value.Value}");
}
```

### Retrieve a token manually for interacting with a resource
- Like the code above this assumes that it's being run from a resource with a managed identity.
- The tokenUri is only accessible from a resource in Azure.
- The resource parameter in tokenUri is used to specify the type of resource that you need access to.
``` c#
async Task UseManagedIdentityManually()
{
    // Get an access token
    string tokenUri = "http://169.254.169.254/metadata/identity/oauth2/token?api-version=2018-02-01&resource=https://vault.azure.net";

    HttpClient client = new HttpClient();
    client.DefaultRequestHeaders.Add("Metadata", "true");

    HttpResponseMessage response = await client.GetAsync(tokenUri);

    string content = await response.Content.ReadAsStringAsync();

    Dictionary<string, string> values = JsonConvert.DeserializeObject<Dictionary<string, string>>(content);

    foreach(KeyValuePair<string, string> pair in values)
    {
        Console.WriteLine($"{pair.Key}:, {pair.Value}");
    }

    // Access the secret using the token
    string secretUri = $"{keyVaultUri}/secrets/{secretName}/{secretVersion}?api-version=7.3";
    HttpClient secretClient = new HttpClient();
    secretClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", values["access_token"]);
    
    HttpResponseMessage secretResponse = await secretClient.GetAsync(secretUri);
    string secret = await secretResponse.Content.ReadAsStringAsync();
    Console.WriteLine($"The secret is {secret}");
}
```