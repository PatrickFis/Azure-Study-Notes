# Microsoft Identity Platform
The Microsoft identity platform for developers is a set of tools which includes authentication service, open-source libraries, and application management tools. The platform is made of several components
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
- Helps troubleshoot
- Supports multiple authentication flows

## Client Applications
- Public and confidential client applications are supported through application builders in the library.

## Implementing Interactive Authentication
https://docs.microsoft.com/en-us/learn/modules/implement-authentication-by-using-microsoft-authentication-library/4-interactive-authentication-msal

1. Sign in to the Azure portal
2. Search for Azure Active Directory (don't create a new one, just use the default directory)
3. Under Manage select App registrations
4. Select New registration
5. Fill in the details
6. Select Register
7. Create a new application as specified in the above link


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
- Helps authenticate users and authorize them use Azure resources
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