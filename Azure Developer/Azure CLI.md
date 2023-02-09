- [Azure CLI](#azure-cli)
- [Resource Groups](#resource-groups)
  - [Creating resource groups](#creating-resource-groups)
  - [Deleting resource groups](#deleting-resource-groups)
  - [Check if resource group exists](#check-if-resource-group-exists)
  - [List resource groups](#list-resource-groups)
  - [Resource group locks](#resource-group-locks)
- [Virtual Machines](#virtual-machines)
  - [Create a VM](#create-a-vm)
  - [Run a command on a VM](#run-a-command-on-a-vm)
  - [Open a port on a VM](#open-a-port-on-a-vm)
  - [Delete a VM](#delete-a-vm)
- [Storage Accounts, Storage Account Containers and Blobs](#storage-accounts-storage-account-containers-and-blobs)
  - [Create a storage account](#create-a-storage-account)
  - [Delete a storage account](#delete-a-storage-account)
  - [Generate a shared access signature](#generate-a-shared-access-signature)
  - [Create a container](#create-a-container)
  - [Delete a container](#delete-a-container)
  - [Upload a blob to a container](#upload-a-blob-to-a-container)
  - [Change the access tier of a blob](#change-the-access-tier-of-a-blob)
  - [Delete a blob from a container](#delete-a-blob-from-a-container)
- [Key Vault](#key-vault)
  - [Create a key vault](#create-a-key-vault)
  - [Delete a key vault](#delete-a-key-vault)
  - [Create a new key](#create-a-new-key)
  - [Encrypt a string using a key](#encrypt-a-string-using-a-key)
  - [Decrypt a string using a key](#decrypt-a-string-using-a-key)
  - [Delete a key](#delete-a-key)
  - [Create (or update) a secret](#create-or-update-a-secret)
  - [Show the value of a secret](#show-the-value-of-a-secret)
  - [Delete a secret](#delete-a-secret)
- [Web applications](#web-applications)
  - [Create a new App Service Plan](#create-a-new-app-service-plan)
  - [Create a new web application](#create-a-new-web-application)
  - [Create a new web app from local files (like from cloud shell)](#create-a-new-web-app-from-local-files-like-from-cloud-shell)
  - [Delete a web app](#delete-a-web-app)
  - [Create a deployment slot](#create-a-deployment-slot)
  - [Swap deployment slots](#swap-deployment-slots)
  - [Configure a web app](#configure-a-web-app)
- [SQL Server](#sql-server)
  - [Create a SQL server](#create-a-sql-server)
  - [Delete a SQL server](#delete-a-sql-server)
  - [Create a database in a SQL server instance](#create-a-database-in-a-sql-server-instance)
  - [Delete a database](#delete-a-database)
- [Cosmos DB](#cosmos-db)
  - [Create a new Cosmos DB](#create-a-new-cosmos-db)
  - [Delete a Cosmos DB](#delete-a-cosmos-db)
  - [Create a SQL database under Cosmos (note that the other databases all have their own commands)](#create-a-sql-database-under-cosmos-note-that-the-other-databases-all-have-their-own-commands)
  - [Delete a SQL database under Cosmos](#delete-a-sql-database-under-cosmos)
- [API Management](#api-management)
  - [Create an API Management Service Instance](#create-an-api-management-service-instance)
  - [Create an APIM API](#create-an-apim-api)
  - [Import an API](#import-an-api)
  - [Delete an APIM API](#delete-an-apim-api)
  - [Delete an APIM Instance](#delete-an-apim-instance)
- [ARM](#arm)
  - [Deploy an ARM template to a resource group](#deploy-an-arm-template-to-a-resource-group)
- [CDN](#cdn)
  - [Create a new CDN Profile](#create-a-new-cdn-profile)
- [Azure Service Bus](#azure-service-bus)
  - [Create a Service Bus Namespace and a Queue](#create-a-service-bus-namespace-and-a-queue)
- [Azure Container Registry](#azure-container-registry)
  - [Creating an ACR and running an image with an ACR Task](#creating-an-acr-and-running-an-image-with-an-acr-task)
- [Azure Container Instances](#azure-container-instances)
  - [Deploying a container instance with Azure CLI](#deploying-a-container-instance-with-azure-cli)
  - [Executing commands and connecting to a container](#executing-commands-and-connecting-to-a-container)
  - [Viewing logs inside a container](#viewing-logs-inside-a-container)

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

# Example command. The returned value can be used in other requests by using the --sas-token optional parameter. The returned value should be given as the value of this parameter and must include the quotes in the returned value.
end=`date -u -d "30 minutes" '+%Y-%m-%dT%H:%MZ'`
az storage account generate-sas --permissions acdfilprtuwxy --account-name testazcliaccount --resource-types sco --services bfqt --expiry $end

# Example usage.
az storage container create -n testcontainer2 --account-name az204reviewstorage223 --sas-token "<token here>"
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
## Create a new App Service Plan
``` bash
az appservice plan create --name (or -n) <app service plan name> \
--resource-group (or -g) <resource group name> \
--location (or -l) <location> \
--sku <sku, like F1 for Free> \
--number-of-workers <number of workers, default = 1>
--is-linux (if this is a Linux App Service Plan)
```

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

## Create a deployment slot
``` bash
az webapp deployment slot create --name (or -n) <web app name> \
--resource-group (or -g) <resource group name> \
--slot (or -s) <slot name>
```

## Swap deployment slots
``` bash
az webapp deployment slot swap --slot (or -s) <slot name> \
--action <preview (apply target slot's setting on the source slot first), reset (reset the swap), swap (complete swap) (default: swap)> \
--name (or -n) <web app name> \
--resource-group (or -g) <resource group name> \
--target-slot <slot to swap (default: production)>
```

## Configure a web app
``` bash
az webapp config set --resource-group (or -g) <resource group name> \
--name (or -n) <web app name> \
# Various settings are available to choose from, here are a few
--always-on <false, true (used to leave your web app loaded all the time)> \
--auto-heal-enabled <false, true (used to turn on or off auto heal)> \
--generic-configurations <site configuration list in a key=value pair or @<json_file>>
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

# API Management
## Create an API Management Service Instance
``` bash
az apim create --name (or -n) <instance name> \
--publisher-email <email> \
--publisher-name <name> \
--resource-group (or -g) <resource group name> \
--location (or -l) <location> \
--sku-name <Basic, Consumption, Developer, Isolated, Premium, Standard>

# Example Command
az apim create --name MyApim -g MyResourceGroup -l eastus --sku-name Consumption --enable-client-certificate \
    --publisher-email email@mydomain.com --publisher-name Microsoft
```

## Create an APIM API
``` bash
az apim api create --api-id <API name> \
--display-name <name to display for the API> \
--path <path to the API> \
--resource-group (or -g) <resource group name> \
--service-name (or -n) <name of the APIM instance>

# Example command
az apim api create --service-name MyApim -g MyResourceGroup --api-id MyApi --path '/myapi' --display-name 'My API'
```

## Import an API
``` bash
az apim api import --path <path to the API> \
--resource-group (or -g) <resource group name> \
--service-name (or -n) <name of the APIM instance> \
--specification-format <GraphQL, OpenApi, OpenApiJson, Swagger, Wadl, Wsdl>

# Example command
az apim api import -g MyResourceGroup --service-name MyApim --path MyApi --specification-url https://MySpecificationURL --specification-format OpenApiJson
```

## Delete an APIM API
``` bash
az apim api delete --api-id <API name> \
--resource-group (or -g) <resource group name> \
--service-name (or -n) <name of the APIM instance>
```

## Delete an APIM Instance
``` bash
az apim delete --name (or -n) <name of the APIM instance> \
--resource-group (or -g) <resource group name>

# Example command
az apim delete -n MyApim -g MyResourceGroup
```

# ARM
## Deploy an ARM template to a resource group
``` bash
az deployment group create --resource-group (or -g) <resource group name> \
--name (or -n) <deployment name> \
--template-uri <URI here> \
--template-file <path to file here> \
--parameters <parameter file or JSON string of parameters here> \
--mode <Complete, Incremental (default)>
```

# CDN
## Create a new CDN Profile
``` bash
az cdn profile create --name (or -n) <CDN name> \
--resource-group (or -g) <resource group name> \
--location (or -l) <location> \
--sku <Standard_Verizon, Standard_Microsoft, Standard_Akamai, Premium_Verizon, default = Standard_Akamai> \
--tags <tags>
```

# Azure Service Bus
## Create a Service Bus Namespace and a Queue
``` bash
# Create a resource group to contain the namespace
az group create -n <resource group name> -l <location>

# Create the namespace
az servicebus namespace create --resource-group (or -g) <resource group name> \
--name (or -n) <namespace name> \
--location (or -l) <location>

# Create the queue
az servicebus queue create --resource-group (or -g) <resource group name> \
--namespace-name <namespace name> \
--name <queue name>

# Get the primary connection string for the namespace to connect to the queue to send and receive messages
az servicebus namespace authorization-rule keys list --resource-group (or -g) <resource group name> \
--namespace-name <namespace name> \
--name RootManageSharedAccessKey \
--query primaryConnectionString \
--output tsv
```

# Azure Container Registry
## Creating an ACR and running an image with an ACR Task
1. Create a resource group and a basic ACR
``` bash
az group create --name az204-acr-rg-patrick --location eastus

az acr create --resource-group az204-acr-rg-patrick --name az204patrickregistry --sku Basic
```
2. Set up a Dockerfile and then build it with the following command
``` bash
az acr build --image sample/hello-world:v1 --registry az204patrickregistry --file Dockerfile .
```
3. Verify that the repository was set up with the following commands
``` bash
az acr repository list --name az204patrickregistry --output table

az acr repository show-tags --name az204patrickregistry --repository sample/hello-world --output table
```
4. Run the image with the following command
``` bash
az acr run --registry az204patrickregistry --cmd '$Registry/sample/hello-world:v1' /dev/null
```
5. Clean up the resources afterwards
``` bash
az group delete --name az204-acr-rg-patrick --no-wait
```

# Azure Container Instances
## Deploying a container instance with Azure CLI
1. Create a new group for ACI
``` bash
az group create --name az204-aci-patrick-rg --location eastus
```
2. Create a DNS name to expose the container to the internet and create the container
``` bash
DNS_NAME_LABEL=aci-example-$RANDOM

# Note: The image can come from ACR. The command will prompt you to enter the username and password for ACI (which is enabled through the Access keys blade's admin user setting)
az container create --resource-group az204-aci-patrick-rg \
--name mycontainer \
--image mcr.microsoft.com/azuredocs/aci-helloworld \
--ports 80 \
--dns-name-label $DNS_NAME_LABEL --location eastus
```
3. Verify the container is running and then navigate to the FQDN
``` bash
az container show --resource-group az204-aci-patrick-rg \
    --name mycontainer \
    --query "{FQDN:ipAddress.fqdn,ProvisioningState:provisioningState}" \
    --out table
```
4. Remove the resource group with the container afterwards
``` bash
az group delete --name az204-aci-patrick-rg --no-wait
```

## Executing commands and connecting to a container
``` bash
# Commands can be run inside a container using a similar syntax as docker exec
az container exec --resource-group (or -g) <resource group name> \
--name <container group name> \
--exec-command "<command>"

# If you have multiple containers in your container group then you'll need to specify the name of the container with --container-name like in this example
az container exec --resource-group myResourceGroup --name mynginx --container-name nginx-app --exec-command "/bin/bash"

# This can be used to do things like launch a Bash shell like the following example
az container exec --resource-group myResourceGroup --name mynginx --exec-command "/bin/bash"
```

## Viewing logs inside a container
``` bash
az container logs --resource-group (or -g) <resource group name> \
--name <container group name> \
--container-name <container name>
```