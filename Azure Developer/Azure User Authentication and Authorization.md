# Microsoft Identity Platform
The Microsoft identity platform for developers is a set of tools which includes authentication service, open-source libraries, and application management tools. The platform is make of several components
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
