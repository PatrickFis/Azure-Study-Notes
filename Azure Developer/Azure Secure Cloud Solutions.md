# Azure Key Vault
Azure Key Vault is a cloud service for securely storing and accessing secrets (API keys, passwords, certificates, cryptographic keys).

## Uses and Benefits
Azure Key Vault helps solve the following problems:
- Secrets Management (see above)
- Key Management - Can create and control encryption keys
- Certificate Management - Can provision, manage, and deploy public and private SSL/TLS certificates for use with Azure and your internal connected resources.

Azure Key Vault has two pricing tiers
- Standard - Uses a software key for encryption
- Premium - Includes hardware security module (HSM) protected keys

Azure Key Vault has the following key benefits
- Centralized application secrets - Useful information like connection strings can be stored in Azure Key Vault instead of your app's code.
- Securely store secrets and keys - Access to Azure Key Vault is controlled by AAD (authentication) and authorized by Azure RBAC or Key Vault access policies.
  - Azure RBAC is used when attempting to access data stored in the vault.
- Monitor access and use - Logging can be enabled to monitor activity. The logs can be sent to a few different places
  - Archived in a storage account
  - Streamed to an event hub
  - Sent to Azure Monitor Logs
- Simplified administration of application secrets - This process is simplified via the following
  - Removes the need for in-house knowledge of HSMs
  - Allows scaling up to meet an org's usage spikes
  - Replication within a region and to a secondary region
  - Standard Azure administration options
  - Automating tasks for certs

## Best Practices
### Authentication
There are three ways to authenticate to Key Vault
- Managed identities for Azure resources - Identities with access to Key Vault can be assigned to one or more resources. Azure automatically rotates the service principal client secret for the identity, and this is the recommended approach as a best practice.
- Service principal and certificate - These can be used to access Key Vault, but it isn't recommended because the app owner or dev must rotate the cert.
- Service principal and secret - Not recommended because it's hard to automatically rotate the bootstrap secret that's used for authentication to Key Vault.

### Encryption of Data in Transit
Azure Key Vault enforces TLS to protect data while it's traveling to the client. Perfect Forward Secrecy (PFS) is used to protect connections between customer's client systems and Microsoft cloud services by unique keys.

### Best Practices
- Use separate key vaults (a vault is a logical group of secrets) - Recommended approach is to use a vault per app per environment. This helps you not share secrets across environments and reduces the threat in case of a breach.
- Control access to your vault - Only allow authorized apps and users access to the vault.
- Backups
- Logging
- Recovery options - Enable soft-delete and purge protection

## Authentication
- See the above authentication options
- Authentication from app code can be done using the Key Vault SDK in the Azure Identity client library
- Authentication through REST must include a bearer token sent in the HTTP Authorization header

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

## Additional Azure Key Vault Resources
https://docs.microsoft.com/en-us/azure/key-vault/general/developers-guide

https://docs.microsoft.com/en-us/azure/key-vault/general/disaster-recovery-guidance


# Managed Identities (this may need more fleshing out)
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


# Azure App Configuration
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

## Feature Flags
Feature flags are used to decouple feature releases from code deployment. The basic concepts are the following
- Feature flag - A boolean indicating if a feature is on or off
- Feature manager - An application that handles the lifecycle of all the feature flags in an application. Typically provides additional functionality like caching and updating statuses.
- Filter - A rule for evaluating the state of a feature flag (user group, device or browser type, geographic location, time window, etc).

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

## Creating and using App Config
There are examples of this in [Code/Azure App Service](Code/Azure%20App%20Service/).