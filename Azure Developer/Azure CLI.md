# Azure CLI
[MS Documentation](https://learn.microsoft.com/en-us/cli/azure/get-started-with-azure-cli) is a good place to reference CLI commands.

# Resource Groups
## Creating resource groups
``` bash
az group create --location (or -l) <location> \
--name (or -n) <name>
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
az group lock create --lock-type (or -t) <CanNotDelete> or <ReadOnly> \
--name (or -n) <Lock Name> \
--resource-group (or -g) <resource group name>

# Delete a lock
az group lock delete --ids <resource IDs, optional> \
--name (or -n) <Lock Name> \
--resource-group (or -g) <resource group name>

# List locks on resource group
az group lock list --resource-group (or -g) <resource group name>

# Show info about lock
az group lock show --ids <resource IDs, optional> \
--name (or -n) <Lock Name> \
--resource-group (or -g) <resource group name>

# Update a lock
az group lock update --ids <resource IDs, optional> \
--name (or -n) <Lock Name> \
--resource-group (or -g) <resource group name>
```

# Virtual Machines
[MS Quickstart for VMs](https://learn.microsoft.com/en-us/azure/virtual-machines/linux/quick-create-cli)
## Create a VM
``` bash
az vm create --resource-group (or -g) <resource group name> \
--name (or -n) <VM name> \
--image <Image name, like UbuntuLTS> \
--admin-username <username> \
--size <Size - Standard_B1s for the free tier, see az vm list-sizes for all available tiers> \
--location (or -l) <location>
--generate-ssh-keys
```

## Run a command on a VM
``` bash
az vm run-command invoke --resource-group (or -g) <resource group name> \
--name (or -n) <VM name> \
--command-id <Command, like RunShellScript> \
--scripts <Shell script to run, like "sudo apt-get update && sudo apt-get install -y nginx" (use quotation marks in the command)>
```
- The command above can be used to install Nginx on a VM to test things out.

## Open a port on a VM
``` bash
az vm open-port --port <port number> \
--resource-group (or -g) <resource group name> \
--name (or -n) <VM name>
```

## Delete a VM
``` bash
az vm delete --resource-group (or -g) <resource group name> \
--name (or -n) <VM name>
--yes (or -y, don't wait for the operation to complete)
```
- This deletes the VM but it would be easier to just delete the entire resource group since it won't clean up all of the associated resources that were created alongside the VM.

# Storage Accounts, Storage Account Containers and Blobs
## Create a storage account
``` bash
az storage account create --name (or -n) <name> \
--resource-group (or -g) <resource group name> \
--kind <BlobStorage, BlockBlobStorage, FileStorage, Storage, StorageV2 (default)> \
--location (or -l) <location> \
--sku <Premium_LRS, Premium_ZRS, Standard_GRS, Standard_GZRS, Standard_LRS, Standard_RAGRS (default), Standard_RAGZRS, Standard_ZRS>
```
- This command has a bunch of options, but I've tried to isolate it to the ones that you're really likely to care about. 
- Example command: `az storage account create -n testazcliaccount -g CLI_Testing --kind StorageV2 -l eastus --sku Standard_LRS`

## Delete a storage account
``` bash
az storage account delete --ids <resource IDs, optional> \
--name (or -n) <name> \
--resource-group (or -g) <resource group name> \
--subscription <name or ID of Azure subscription, optional if default is set> \
--yes (or -y, to not prompt for confirmation)
```

## Generate a shared access signature
``` bash
az storage account generate-sas --expiry <Y-m-d'T'H:M'Z'> \
--permissions <(a)dd (c)reate (d)elete (f)ilter_by_tags (i)set_immutability_policy (l)ist (p)rocess (r)ead (t)ag (u)pdate (w)rite (x)delete_previous_version (y)permanent_delete. Can be combined> \
--resource-types <(s)ervice (c)ontainer (o)bject. Can be combined.> \
--services <(b)lob (f)ile (q)ueue (t)able. Can be combined.> \
--account-name <storage account name>

# Example command
end=`date -u -d "30 minutes" '+%Y-%m-%dT%H:%MZ'`
az storage account generate-sas --permissions acdfilprtuwxy --account-name testazcliaccount --resource-types sco --services bfqt --expiry $end
```

## Create a container
``` bash
az storage container create --name (or -n) <name of container> \
--account-name <storage account name>
```

## Delete a container
``` bash
az storage container delete --name (or -n) <name of container> \
--account-name <storage account name>
```

## Upload a blob to a container
``` bash
az storage blob upload -f <path to file> \
-c <container name> \
--account-name <storage account name> \
-n <blob name> \
--tier (Archive, Cool, or Hot. Optional parameter)
```

## Change the access tier of a blob
``` bash
az storage blob set-tier --container-name (or -c) <container name> \
--name (or -n) <blob name> \
--tier (Archive, Cool, or Hot) \
--account-name <storage account name>
```

## Delete a blob from a container
``` bash
az storage blob delete -c <container name> \
--account-name <storage account name> \
-n <blob name>
```

# Key Vault
## Create a key vault
``` bash
az keyvault create --resource-group (or -g) <resource group name> \
--location (or -l) <location> \
--name (or -n) <key vault name>
```

## Delete a key vault
``` bash
az keyvault delete --resource-group (or -g) <resource group name> \
--name (or -n) <key vault name>
```

## Create a new key
``` bash
az keyvault key create --kty <EC, EC-HSM, RSA, RSA-HSM, oct, oct-HSM> \
--size <2048, 3072, or 4096 for RSA. 128, 192, or 256 for oct.> \
--name (or -n) <key name> \
--vault-name <vault name>
```

## Encrypt a string using a key
``` bash
az keyvault key encrypt --algorithmn <A128CBC, A128CBCPAD, A128GCM, A192CBC, A192CBCPAD, A192GCM, A256CBC, A256CBCPAD, A256GCM, RSA-OAEP, RSA-OAEP-256, RSA1_5> \
--value <unencrypted value> \
--data-type <base64 or plaintext> \
--name (or -n) <name of the key to encrypt the value with> \
--vault-name <vault name>

# Example command
az keyvault key encrypt --algorithm RSA-OAEP --value "encrypt me" --data-type plaintext --name testkey --vault-name clivaultaz204
```

## Decrypt a string using a key
``` bash
az keyvault key decrypt --algorithm <algorithm used during encryption> \
--value <encrypted value> \
--data-type <data type of the original value> \
--name (or -n) <name of the key used for encryption> \
--vault-name <vault name>
```
- This command can be used to decrypt the value from the previous example.

## Delete a key
``` bash
az keyvault key delete --name (or -n) <name of the key> \
--vault-name <vault name>
```

## Create (or update) a secret
``` bash
az keyvault secret set --name (or -n) <name of the secret> \
--vault-name <vault name> \
--value <secret value> \
--content-type (or --description) <description of the secret contents, optional>
```

## Show the value of a secret
``` bash
az keyvault secret show --name (or -n) <name of the secret> \
--vault-name <vault name>
```

## Delete a secret
``` bash
az keyvault secret delete --name (or -n) <name of the secret> \
--vault-name <vault name>
```

# Web applications
## Create a new web application
``` bash
az webapp create --name (or -n) <name of the web app> \
--plan <name of the app service plan to host the web app> \
--resource-group (or -g) <resource group name> \
# Optional parameters below
--assign-identity <'[system]' for a system assigned identity or the ID of a user assigned identity> \
--runtime (or -r) <runtime for the webapp like dotnet:6. See az webapp list-runtimes for all available runtimes.>
--deployment-container-image-name (or -i) <container image name from Docker Hub>

## Example command to make an nginx web app
az webapp create --name nginxaz204test --plan fischerpl18_asp_9748 --resource-group fischerpl18_rg_1227 --assign-identity '[system]' --deployment-container-image-name nginx
```

## Create a new web app from local files (like from cloud shell)
``` bash
az webapp up --name (or -n) <name of the web app> \
--plan <name of the app service plan to host the web app> \
--resource-group (or -g) <resource group name> \
--location (or -l) <location> \
--html (use if deploying a static HTML app)

# Example from https://learn.microsoft.com/en-us/training/modules/introduction-to-azure-app-service/7-create-html-web-app?ns-enrollment-type=learningpath&ns-enrollment-id=learn.wwl.create-azure-app-service-web-apps

az webapp up -g fischerpl18_rg_1227 -n testwebappaz204 --plan fischerpl18_asp_9748 -l eastus --html
```

## Delete a web app
``` bash
az webapp delete --name (or -n) <name of the web app> \
--resource-group (or -g) <resource group name>
```

# SQL Server
## Create a SQL server
``` bash
az sql server create --name (or -n) <name of the SQL server> \
--resource-group (or -g) <resource group name> \
--location (or -l) <location> \
--admin-user (or -u) <admin username> \
--admin-password (or -p) <admin password>
```

## Delete a SQL server
``` bash
az sql server delete --name (or -n) <name of the SQL server> \
--resource-group (or -g) <resource group name> \
--yes (or -y) <skip confirmation, optional>
```

## Create a database in a SQL server instance
``` bash
az sql db create --name (or -n) <database name> \
--resource-group (or -g) <resource group name> \
--server (or -s) <SQL server name> \
--compute-model <Provisioned or Serverless> \
--edition (or --tier or -e) <Basic, Standard, Premium, GeneralPurpose, BusinessCritical, or Hyperscale> \
--capacity (or -c) <integer number of DTUs or vcores>
```

## Delete a database
``` bash
az sql db delete --name (or -n) <database name> \
--resource-group (or -g) <resource group name> \
--server (or -s) <SQL server name>
--yes (or -y) <skip confirmation, optional>
```

# Cosmos DB
## Create a new Cosmos DB
``` bash
az cosmosdb create --name (or -n) <Cosmos DB account name> \
--resource-group (or -g) <resource group name> \
--enable-free-tier <true or false> \
--locations regionName=<location> (note that this is different than the other commands that accept --location or -l)
```

## Delete a Cosmos DB
``` bash
az cosmosdb delete --name or (-n) <Cosmos DB account name> \
--resource-group (or -g) <resource group name> \
--yes (or -y) <skip confirmation, optional>
```

## Create a SQL database under Cosmos (note that the other databases all have their own commands)
``` bash
az cosmosdb sql database create --account-name (or -a) <Cosmos DB account name> \
--name (or -n) <database name> \
--resource-group (or -g) <resource group name> \
--throughput <Integer value for RU/s for throughput. The default value is 400.>
```

## Delete a SQL database under Cosmos
``` bash
az cosmosdb sql database delete --account-name (or -a) <Cosmos DB account name> \
--name (or -n) <database name> \
--resource-group (or -g) <resource group name>
--yes (or -y) <skip confirmation, optional>
```