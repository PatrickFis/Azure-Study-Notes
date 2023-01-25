# Azure Key Vault [MS Documentation on Azure Key Vault](https://learn.microsoft.com/en-us/azure/key-vault/general/overview)
Azure Key Vault is a cloud service for securely storing and accessing secrets (API keys, passwords, certificates, cryptographic keys).

## Uses and Benefits
Azure Key Vault helps solve the following problems:
- Secrets Management - Used to securely store and control access to tokens, passwords, certs, API keys, other secrets
- Key Management - Can create and control encryption keys
- Certificate Management - Can provision, manage, and deploy public and private SSL/TLS certificates for use with Azure and your internal connected resources.

Azure Key Vault has two pricing tiers
- Standard - Uses a software key for encryption
- Premium - Includes hardware security module (HSM) protected keys

Azure Key Vault has the following key benefits
- Centralized application secrets 
  - Key Vault allows you to control the distribution of secrets.
  - Useful information like connection strings can be stored in Azure Key Vault instead of your app's code. Apps can access it using URIs, and the URIs allow for specific versions of secrets to be retrieved.
- Securely store secrets and keys - Access to Azure Key Vault is controlled by AAD (authentication) and authorized by Azure RBAC or Key Vault access policies.
  - Azure RBAC is used when attempting to access data stored in the vault.
  - Key Vaults can be software-protected or they can use hardware security modules (HSMs) if you use a premium plan.
- Monitor access and use - Logging can be enabled to monitor activity. The logs can be sent to a few different places
  - Archived in a storage account
  - Streamed to an event hub
  - Sent to Azure Monitor Logs
- Simplified administration of application secrets - This process is simplified via the following
  - Removes the need for in-house knowledge of HSMs
  - Allows scaling up to meet an org's usage spikes
  - Replication within a region and to a secondary region for high availability. Does not require an admin to trigger the failover.
  - Standard Azure administration options (portal, CLI, PowerShell)
  - Automating tasks for certs that are purchased from public CAs (ex: enrollment and renewal)

## Best Practices
### Authentication [MS Documentation on Authentication](https://learn.microsoft.com/en-us/azure/key-vault/general/authentication)
There are three ways to authenticate to Key Vault
1. Managed identities for Azure resources - Identities with access to Key Vault can be assigned to one or more resources. Azure automatically rotates the service principal client secret for the identity, and this is the recommended approach as a best practice.
   1. Applications are recommended to use system-assigned managed identities for obtaining service principals. Azure will manage the service principal for you and automatically authenticate the app with other Azure services.
2. Service principal and certificate - These can be used to access Key Vault, but it isn't recommended because the app owner or dev must rotate the cert.
3. Service principal and secret - Not recommended because it's hard to automatically rotate the bootstrap secret that's used for authentication to Key Vault. 
- Authentication from app code can be done using the Key Vault SDK in the Azure Identity client library
- Authentication through REST must include a bearer token sent in the HTTP Authorization header

This is the request operation flow for authentication with a Key Vault:
1. A token is requested by authenticating with AAD by a resource or a user
2. The security principal is granted an OAuth token if authentication was successful
3. The Key Vault's REST API is called through its endpoint
4. The Key Vault Firewall checks if the call should be allowed by checking the following criteria. If any criteria are met the call is allowed, otherwise a forbidden response is returned.
   1. Firewall is disabled and the Key Vault is reachable over the public internet
   2. Caller is a Key Vault Trusted Service which is allowed to bypass the firewall
   3. Caller is listed in the firewall by IP address, virtual network, or service endpoint
   4. Caller can reach Key Vault over a configured private link connection
5. The Key Vault calls AAD to validate the security principal's access token if the firewall allowed the call
6. The Key Vault checks if the security principal has the necessary permissions for the requested operation. If it doesn't a forbidden response is returned.
7. The Key Vault performs the requested operation and returns the result

### Encryption of Data in Transit
Azure Key Vault enforces TLS to protect data while it's traveling to the client. Perfect Forward Secrecy (PFS) is used to protect connections between customer's client systems and Microsoft cloud services by unique keys.

### Best Practices
- Use separate key vaults (a vault is a logical group of secrets) - Recommended approach is to use a vault per app per environment. This helps you not share secrets across environments and reduces the threat in case of a breach.
- Control access to your vault - Only allow authorized apps and users access to the vault.
- Backups
- Logging
- Recovery options - Enable soft-delete and purge protection


## Creating an Azure Key Vault and Using it
### CLI
1. Login to the Azure portal and open a Cloud Shell
2. Create a resource group and Key Vault
``` bash
az group create --name az204-patrick-vault-rg --location eastus

az keyvault create --name az204vault-patrick-425 --resource-group az204-patrick-vault-rg --location eastus
```
3. Create a secret
``` bash
az keyvault secret set --vault-name az204vault-patrick-425 --name "ExamplePassword" --value "asdfa78bvcx"
```
4. Retrieve the secret using the CLI (the first command will show JSON and data about the secret while the second will just show a table containing the name and the value)
``` bash
az keyvault secret show --name "ExamplePassword" --vault-name az204vault-patrick-425

az keyvault secret show --name "ExamplePassword" --vault-name az204vault-patrick-425 --output table
```
5. Clean up the resources when you're done
``` bash
az group delete --name az204-patrick-vault-rg --no-wait
```

### REST
This assumes that you have an app registered in AAD. I have one registered for an Azure App Service that I used. I used https://anoopt.medium.com/accessing-azure-key-vault-secret-through-azure-key-vault-rest-api-using-an-azure-ad-app-4d837fed747 for setting this up.
1. Follow the above steps to create a key vault but don't delete it
2. Navigate to the app you've created and add a new client secret and copy the value and secret ID
3. Navigate to the key vault in the Azure portal
4. Click on Access policies
5. Add the Get Secret permission
6. Select an app you've created in AAD for the principal
7. Click Add
8. Click Save
9. Navigate to the Overview page for the key vault
10. Copy the Vault URI (https://az204vault-patrick-425.vault.azure.net/)
11. Open Postman
12. Create a new POST for https://login.microsoftonline.com/directoryId/oauth2/v2.0/token 
    1.  Replace directoryId with the directory ID of your app from AAD
    2.  Add a body with type form-data
        1.  Add a grant_type of client_credentials
        2.  Add a client_id of the app from AAD
        3.  Add a client_secret of the value of the secret you made in step 2
        4.  Add a scope of https://vault.azure.net/.default
13. Send the POST to retrieve a Bearer token and copy it
14. Create a GET request for https://az204vault-patrick-425.vault.azure.net/secrets/ExamplePassword?api-version=7.3
    1.  Set the Authorization to Bearer Token and paste the token you retrieved earlier into it
15. Send the request and verify that the value was retrieved

Misc:
1.  oAo8Q~pYcSWcqokl~PkDoAuYBIVGaSBk4-kKfalO secret value
2.  3f62c591-d1f0-4488-90d0-7f8bf3a5c276 secret ID

### API/SDK
I added some of the Key Vault packages to [this](Code/Azure%20App%20Service/) and used it to retrieve a connection string.

API documentation for the Azure Identity library that I used https://docs.microsoft.com/en-us/dotnet/api/overview/azure/identity-readme.


## Azure Key Vault Important Terms [MS Documentation](https://learn.microsoft.com/en-us/azure/key-vault/general/basic-concepts)
- Tenant: A tenant is the organization that owns and manages a specific instance of Microsoft cloud services. It's most often used to refer to the set of Azure and Microsoft 365 services for an organization.
- Vault owner: A vault owner can create a key vault and gain full access and control over it. The vault owner can also set up auditing to log who accesses secrets and keys. Administrators can control the key lifecycle. They can roll to a new version of the key, back it up, and do related tasks.
- Vault consumer: A vault consumer can perform actions on the assets inside the key vault when the vault owner grants the consumer access. The available actions depend on the permissions granted.
- Managed HSM Administrators: Users who are assigned the Administrator role have complete control over a Managed HSM pool. They can create more role assignments to delegate controlled access to other users.
- Managed HSM Crypto Officer/User: Built-in roles that are usually assigned to users or service principals that will perform cryptographic operations using keys in Managed HSM. Crypto User can create new keys, but cannot delete keys.
- Managed HSM Crypto Service Encryption User: Built-in role that is usually assigned to a service accounts managed service identity (e.g. Storage account) for encryption of data at rest with customer managed key.
- Resource: A resource is a manageable item that's available through Azure. Common examples are virtual machine, storage account, web app, database, and virtual network. There are many more.
- Resource group: A resource group is a container that holds related resources for an Azure solution. The resource group can include all the resources for the solution, or only those resources that you want to manage as a group. You decide how you want to allocate resources to resource groups, based on what makes the most sense for your organization.
- Security principal: An Azure security principal is a security identity that user-created apps, services, and automation tools use to access specific Azure resources. Think of it as a "user identity" (username and password or certificate) with a specific role, and tightly controlled permissions. A security principal should only need to do specific things, unlike a general user identity. It improves security if you grant it only the minimum permission level that it needs to perform its management tasks. A security principal used with an application or service is specifically called a service principal.
- Azure Active Directory (Azure AD): Azure AD is the Active Directory service for a tenant. Each directory has one or more domains. A directory can have many subscriptions associated with it, but only one tenant.
- Azure tenant ID: A tenant ID is a unique way to identify an Azure AD instance within an Azure subscription.
- Managed identities: Azure Key Vault provides a way to securely store credentials and other keys and secrets, but your code needs to authenticate to Key Vault to retrieve them. Using a managed identity makes solving this problem simpler by giving Azure services an automatically managed identity in Azure AD. You can use this identity to authenticate to Key Vault or any service that supports Azure AD authentication, without having any credentials in your code. For more information, see the following image and the overview of managed identities for Azure resources.

## Additional Azure Key Vault Resources
https://docs.microsoft.com/en-us/azure/key-vault/general/developers-guide

https://docs.microsoft.com/en-us/azure/key-vault/general/disaster-recovery-guidance


# Managed Identities (this may need more fleshing out) [MS Documentation on Managed Identities](https://learn.microsoft.com/en-us/azure/active-directory/managed-identities-azure-resources/overview)
## Types
- System-assigned
  - Created as part of an Azure resource
  - Shared lifecycle with the resource that the managed identity was created with. When the parent resource is deleted the managed identity is cleaned up as well.
  - Can't be shared across resources, it can only be associated with a single resource
- User-assigned
  - Created as a stand-alone Azure resource
  - Independent lifecycle that must be explicitly deleted
  - Can be shared with more than one Azure resource
## Authentication Flow
### System-assigned managed identity
1. Azure Resource Manager receives a request to enable the system-assigned managed identity on a virtual machine.
2. Azure Resource Manager creates a service principal in Azure Active Directory for the identity of the virtual machine. The service principal is created in the Azure Active Directory tenant that's trusted by the subscription.
3. Azure Resource Manager configures the identity on the virtual machine by updating the Azure Instance Metadata Service identity endpoint with the service principal client ID and certificate.
4. After the virtual machine has an identity, use the service principal information to grant the virtual machine access to Azure resources. To call Azure Resource Manager, use role-based access control in Azure Active Directory to assign the appropriate role to the virtual machine service principal. To call Key Vault, grant your code access to the specific secret or key in Key Vault.
5. Your code that's running on the virtual machine can request a token from the Azure Instance Metadata service endpoint, accessible only from within the virtual machine: http://169.254.169.254/metadata/identity/oauth2/token
6. A call is made to Azure Active Directory to request an access token (as specified in step 5) by using the client ID and certificate configured in step 3. Azure Active Directory returns a JSON Web Token (JWT) access token.
7. Your code sends the access token on a call to a service that supports Azure Active Directory authentication.

### User-assigned managed identity
1. Azure Resource Manager receives a request to create a user-assigned managed identity.
2. Azure Resource Manager creates a service principal in Azure Active Directory for the user-assigned managed identity. The service principal is created in the Azure Active Directory tenant that's trusted by the subscription.
3. Azure Resource Manager receives a request to configure the user-assigned managed identity on a virtual machine and updates the Azure Instance Metadata Service identity endpoint with the user-assigned managed identity service principal client ID and certificate.
4. After the user-assigned managed identity is created, use the service principal information to grant the identity access to Azure resources. To call Azure Resource Manager, use role-based access control in Azure Active Directory to assign the appropriate role to the service principal of the user-assigned identity. To call Key Vault, grant your code access to the specific secret or key in Key Vault. Note: You can also do this before step 3.
5. Your code that's running on the virtual machine can request a token from the Azure Instance Metadata Service identity endpoint, accessible only from within the virtual machine: http://169.254.169.254/metadata/identity/oauth2/token
6. A call is made to Azure Active Directory to request an access token (as specified in step 5) by using the client ID and certificate configured in step 3. Azure Active Directory returns a JSON Web Token (JWT) access token.
7. Your code sends the access token on a call to a service that supports Azure Active Directory authentication.


# Azure App Configuration [MS Documentation on Azure App Configuration](https://learn.microsoft.com/en-us/azure/azure-app-configuration/overview)
Azure App Configuration is used to store application specific settings and secure their access in one place. It offers the following features
- Flexible key representations and mappings
- Tagging with labels
- Point in time replay of settings
- Dedicated UI for feature flag management
- Comparison of two sets of configs
- Enhanced security through Azure managed identities
- Data encryptions (at rest and in transit)
- Native integration with popular frameworks

## Creating keys and values
- Keys are the name used for key-value pairs
- Keys are case sensitive and support unicode
- Keys are generally named using either a flat or hierarchical structure. Hierarchical naming offers some advantages
  - Easier to read as the keys are delimited by a character
  - Easier to manage as they're already grouped logically
  - Easier to use as you can write pattern matching
- Key values can be labeled to differentiate key values with the same key (useful for things that differ between environments)
- Key values are uniquely identified by their key plus a nullable label. You can query for key values by specifying a pattern. The App Configuration will return all key values and attributes that match the pattern.
- Key values are unicode strings
  - Values have an optional user-defined content type which can be used to specify things like encoding schemes
- Configuration data in an App Configuration is encrypted at rest and in transit
- Point-in-time snapshots are used by App Configuration to record changes made to key values
  - Free tiers have 7 days of data while standard has 30 days
  - Can be used to recover settings so that a rollback can occur
  - Can be used to browse historical values

## Feature Flags
Feature flags are used to decouple feature releases from code deployment. The basic concepts are the following
- Feature flag - A boolean indicating if a feature is on or off
- Feature manager - An application that handles the lifecycle of all the feature flags in an application. Typically provides additional functionality like caching and updating statuses.
- Filter - A rule for evaluating the state of a feature flag (user group, device or browser type, geographic location, time window, etc).

Feature flags are used to address the following problems
- Code branch management - Hide functionality that's currently in development and allow it to safely be deployed to production. This makes development easier to manage.
- Test in production - Feature flags can be used to grant users access to features before it's available to all users.
- Flighting - Used to incrementally roll out new features to users. Can target a percentage of users and be increased gradually over time.
- Instant kill switch - Provides a safety net for new features as they can be disabled without a rebuild and redeployment.
- Selective activation - Allows for user segmentation based on specific criteria (example: you can define a feature flag for users that use a certain browser so that functionality is only available for a supported browser which can be updated over time without making code changes).

Feature flags need to be externalized to be used effectively. This allows you to change the feature flags without modifying and deploying the application using them.

The feature manager supports appsettings.json as a config source for feature flags. The following JSON shows an example of setting up a feature flag.
``` json
"FeatureManagement": {
    "FeatureA": true, // Feature flag set to on
    "FeatureB": false, // Feature flag set to off
    "FeatureC": {
        "EnabledFor": [
            {
                "Name": "Percentage",
                "Parameters": {
                    "Value": 50
                }
            }
        ]
    }
}
```

## Securing App Configuration Data
App Configuration data can be secured using the following methods
- Customer-managed keys
  - Data is encrypted at rest using a 256-bit AES encryption key provided by MS by default. Providing your own key will change the behavior of App Configuration.
  - App Config will use a managed identity (which is assigned to the App Config and must have GET, WRAP, and UNWRAP permissions in the Key Vault's access policies) to authenticate with AAD and then call a Key Vault to wrap the App Config instance's encryption key. The wrapped encryption key is then stored, and the unwrapped encryption key is cached within the App Config for 1 hour (after which it is refreshed).
    - Note: Key wrapping is the process of encrypting one key using another key so that it can be securely stored or transmitted over an untrusted channel.
  - A customer managed key serves as a root encryption key.
  - Access can be revoked by changing a Key Vault access policy which will cause the App Config to lose the ability to decrypt data within one hour.
  - Requires the following
    - Standard tier App Config instance
    - Key Vault with soft-delete and purge-protection features enabled
    - An RSA or RSA-HSM key within the Key Vault (must not be expired, must be enabled, must have both wrap and unwrap capabilities enabled)
- Private endpoints
  - Private endpoints allow you to do the following
    - Secure app config details by configuring the firewall to block all connections to App Config on the public endpoint.
    - Increase security for your VNet by ensuring data doesn't escape from it.
    - Securely connect to the App Config from on-prem networks using a VPN or ExpressRoutes with private-peering to connect to the VNet.
  - Private endpoints allow clients on a VNet to securely access data over a private link. Network traffic is sent over the MS backbone network instead of the public internet.
- Managed identities
  - Managed identities allow App Config to access other resources protected by AAD.
  - Applications can use two types of identities
    - System-assigned identities - Tied to the App Config and deleted alongside it. Only one is allowed per App Config.
    - User-assigned identities - Standalone Azure resource that can be assigned to your App Config. An App Config can have multiple user-assigned identities.

## Creating and using App Config
There are examples of this in [Code/Azure App Service](Code/Azure%20App%20Service/).


# Udemy Notes (Section 9 - Split with User Authentication and Authorization)

## Application Objects [MS Documentation](https://learn.microsoft.com/en-us/azure/active-directory/develop/app-objects-and-service-principals#application-object)
- Instead of embedding access keys in an application an application object (like a managed identity) can be used instead.

Registering applications can be done with the following steps:
1. Navigate to AAD
2. Click on the App registrations blade
3. Click New registration
4. Give a name to the application
5. Leave the supported account types section alone
6. Leave the redirect URI alone
7. Click on Register

After registering an application you can give it access to a resource by following the steps given to assign RBAC roles to a user with one difference: search for the name of the app registration instead of the user.

### Using Application Objects in .NET
- I'm reusing the UdemyStorageAccountLab project for this.
- Your code will need the following values to use the app registration (which can be found on the app registration in the portal)
  - Client ID: Application (client) ID in the portal
  - Tenant ID: Directory (tenant) ID in the portal
  - Client secret: Made by going to Certificates & secrets -> Client secrets -> clicking New client secret -> creating the secret -> copying the value from the secret
- You'll need to install the Azure.Identity package and make use of the ClientSecretCredential class to use these values.

## Microsoft Graph
- The Graph API can be used to retrieve information about users.
- A new app registration is needed to interact with the API through Postman. The following steps were used to grant it access to use the Graph API.
1. Navigate to the app registration that you made for Postman in AAD
2. Click on the API permissions blade
3. By default new application objects have the User.Read permission to the Graph API. Remove this permission.
4. Click on Add a permission
5. Select Microsoft Graph
6. Select Application permissions
7. Find the User section
8. Select the User.Read.All permission
9. Click Add permissions
10. Click Grant admin consent for Default Directory

Postman will need to call MS's authentication service to get an access token. The following steps were performed for this:
1. Navigate to the app registration for Postman
2. Click on the Endpoints button
3. Copy the OAuth 2.0 token endpoint (v2) URL
4. Create a new POST request in Postman for the URL from step 3
5. Navigate to the body of the request
6. Set the body to x-www-form-urlencoded with the following key-value pairs
   1. grant_type:client_credentials
   2. client_id: Application (client) ID in the portal
   3. client_secret: The value of a new client secret generated using the same steps from the Using Application Objects in .NET section
   4. scope:https://graph.microsoft.com/.default
7. Click Send and verify that you received a bearer token

Postman can use the access token from the previous steps to make requests to the graph API using the following steps:
1. Create a new GET request in Postman
2. Make the URL https://graph.microsoft.com/v1.0/users
3. Add the bearer token from the previous steps to your request
4. Send the request and verify that you received details about users in Azure

You can also make changes to users using the Graph API using the following steps:
1. Add another permission to the Postman app registration: User.ReadWrite.All
2. Retrieve a new access token
3. Make a new PATCH request to https://graph.microsoft.com/v1.0/users/[user ID retrieved from the previous set of steps]
4. Add the bearer token to your request
5. Send a JSON body like this to update a field associated with the user:
``` json
{
    "givenName": "Test Update from Postman"
}
```
6. Verify that your field was updated for the specified user

## Key Vault
### Creating a Key Vault
1. Search for the Key Vault resource in the marketplace
2. Give the key vault a resource group, name, region, and pricing tier
3. Change the days to retain deleted vaults to 7 days
4. Grant yourself access to the key vault in the access policy section
5. Leave networking and tags alone
6. Create the key vault

### Creating an encryption key (the steps for secrets are fairly similar)
1. Navigate to the key vault
2. Go to the Keys blade
3. Click Generate/Import
4. Give the key a name
5. Click create

### Using the Key Vault in an application
- The code for this is available in [Code/Visual Studio Projects/UdemyKeyVault](Code/Visual%20Studio%20Projects/UdemyKeyVault/).
- Install the Azure.Security.KeyVault.Keys and Azure.Security.KeyVault.Secrets packages.
- Create a new app registration (see the application objects section for how to do this) and a secret to use in your application.
- Grant your app registration the ability to Get Key permission as well as Decrypt and Encrypt (under Access policies in the keyvault). Also grant it the Get permission for secrets.


## Managed Identities
- I created a temporary VM and enabled its managed identity to try using a managed identity with the key vault program I wrote earlier. I granted the managed identity access to the key vault and the program was able to successfully use it instead of the app registration. My VM was using Ubuntu 20.04 and I used these instructions for installing dotnet 6.0: https://learn.microsoft.com/en-us/dotnet/core/install/linux-ubuntu#2004.
- A Powershell script to turn on the managed identity on a VM is located in [Code/Azure Powershell/PowerShellManagedIdentity.ps1](Code/Azure%20Powershell/PowerShellManagedIdentity.ps1).

### User Assigned Identity
- User assigned managed identities are resources in Azure and are created by setting up the "User assigned managed identity" resource.
- The identity can be assigned to resources by going to the Identity blade and then the User assigned section.