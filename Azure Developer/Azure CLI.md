# Azure CLI
[MS Documentation](https://learn.microsoft.com/en-us/cli/azure/get-started-with-azure-cli) is a good place to reference CLI commands.

# Resource Groups
## Creating resource groups
``` bash
az group create --location (or -l) <location> --name (or -n) <name>
```

## Deleting resource groups
``` bash
az group delete --name (or -n) <name>
```
- Deleting resource groups accepts other optional parameters as well:
  - --force-deletion-f (or -f) <Microsoft.Compute/virtualMachineScaleSets> or <Microsoft.Compute/virtualMachines> - Deletes the specified type of resource. Note that there are only two allowed values.
  - --no-wait - Don't wait for the operation to finish.
  - --yes (or -y) - Don't prompt for confirmation.

## Check if resource group exists
``` bash
az group exists --name (or -n) <name>
```

## List resource groups
``` bash
az group list
```

## Resource group locks

``` bash
# Create a lock
az group lock create --lock-type (or -t) <CanNotDelete> or <ReadOnly> --name (or -n) <Lock Name> --resource-group (or -g) <resource group name>

# Delete a lock
az group lock delete --ids <resource IDs, optional> --name (or -n) <Lock Name> --resource-group (or -g) <resource group name>

# List locks on resource group
az group lock list --resource-group (or -g) <resource group name>

# Show info about lock
az group lock show --ids <resource IDs, optional> --name (or -n) <Lock Name> --resource-group (or -g) <resource group name>

# Update a lock
az group lock update --ids <resource IDs, optional> --name (or -n) <Lock Name> --resource-group (or -g) <resource group name>
```

# Virtual Machines (TODO)

# Storage Accounts, Storage Account Containers and Blobs
## Create a storage account
``` bash
az storage account create --name (or -n) <name> --resource-group (or -g) <resource group name> --kind <BlobStorage, BlockBlobStorage, FileStorage, Storage, StorageV2 (default)> --location (or -l) <location> --sku <Premium_LRS, Premium_ZRS, Standard_GRS, Standard_GZRS, Standard_LRS, Standard_RAGRS (default), Standard_RAGZRS, Standard_ZRS>
```
- This command has a bunch of options, but I've tried to isolate it to the ones that you're really likely to care about. 
- Example command: `az storage account create -n testazcliaccount -g CLI_Testing --kind StorageV2 -l eastus --sku Standard_LRS`

## Delete a storage account
``` bash
az storage account delete --ids <resource IDs, optional> --name (or -n) <name> --resource-group (or -g) <resource group name> --subscription <name or ID of Azure subscription, optional if default is set> --yes (or -y, to not prompt for confirmation)
```

## Generate a shared access signature
``` bash
az storage account generate-sas --expiry <Y-m-d'T'H:M'Z'> --permissions <(a)dd (c)reate (d)elete (f)ilter_by_tags (i)set_immutability_policy (l)ist (p)rocess (r)ead (t)ag (u)pdate (w)rite (x)delete_previous_version (y)permanent_delete. Can be combined> --resource-types <(s)ervice (c)ontainer (o)bject. Can be combined.> --services <(b)lob (f)ile (q)ueue (t)able. Can be combined.> --account-name <storage account name>

# Example command
end=`date -u -d "30 minutes" '+%Y-%m-%dT%H:%MZ'`
az storage account generate-sas --permissions acdfilprtuwxy --account-name testazcliaccount --resource-types sco --services bfqt --expiry $end
```

## Create a container
``` bash
az storage container create --name (or -n) <name of container> --account-name <storage account name>
```

## Delete a container
``` bash
az storage container delete --name (or -n) <name of container> --account-name <storage account name>
```

## Upload a blob to a container
``` bash
az storage blob upload -f <path to file> -c <container name> --account-name <storage account name> -n <blob name>
```

## Delete a blob from a container
``` bash
az storage blob delete -c <container name> --account-name <storage account name> -n <blob name>
```