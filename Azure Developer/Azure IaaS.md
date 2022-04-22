# VMs

## Availability
VMs have various options for ensuring availability

### Availability zones
Physically separate zones within an Azure region. Combo of a fault domain and an update domain.
-  Fault domains - Logical group of underlying hardware similar to a rack in an on-prem datacenter. VMs created in Azure are distributed across fault domains to minimize the impact of hardware failures.
-  Update domains - Logical group of underlying hardware that can undergo maintenance or rebooted at the same time. Azure distributes your VMs over update domains so that at least one instance is up at a time.
-  HA made possible through the following
   -  Zonal services - Resource pinned to a specific zone
   -  Zone-redundant services - Azure replicates things across zones
- Availability Sets - Logical grouping of VMs used by Azure to provide redundancy and availability to your application. Composed of fault domains and update domains.

### VM Scale Sets
Group of load balanced VMs. VM instances increase or decrease in response to demand or on a defined schedule.

### Load Balancer
Azure Load Balancer is a layer 4 (TCP, UDP) load balancer that distributes incoming traffic among healthy VMs.

## VM Sizing
| VM Type                  | Description                                                                            |
| :----------------------- | :------------------------------------------------------------------------------------- |
| General Purpose          | Balanced CPU to memory ratio                                                           |
| Compute Optimized        | High CPU to memory ratio                                                               |
| Memory Optimized         | High memory to CPU ratio                                                               |
| Storage Optimized        | High disk throughput and IO (big data)                                                 |
| GPU                      | Specialized VMs for heavy graphic rendering (and other applications)                   |
| High Performance Compute | Fastest and most powerful CPUs with optional high-throughput network interfaces (RDMA) |

VMs can be resized as long as your current hardware config can fit in the new size. Stopping and deallocating VMs allows you to select any size.

## Creating VMs from the CLI
1. Create a group for the VM
``` bash
az group create --name az204-patrick-vm --location eastus
```
2. Create the VM (Australia East was the only location I could find that had space for the free tier of VM that I wanted to use)
``` bash
az vm create \
    --resource-group az204-patrick-vm \
    --name az204vm \
    --image UbuntuLTS \
    --generate-ssh-keys \
    --size Standard_B1s \
    --admin-username patrick \
    --location australiaeast
```
3. Install a web server on the VM and open port 80. Install Nginx using apt after opening the port and SSHing in.
``` bash
az vm open-port --port 80 \
--resource-group az204-patrick-vm \
--name az204vm
```
4. When done with the VM delete the resource group that it's in. Also delete the Network Watcher resource group that was created alongside it.
``` bash
az group delete --name az204-patrick-vm --no-wait
az group delete --name NetworkWatcherRG --no-wait
```


# ARM
Azure Resource Manager is the deployment and management system for Azure. It is a management layer allowing for creation, updates, and deletion of Azure resources in your subscription.
- ARM has a declarative syntax that allows for any Azure resource be deployed
- Results are repeatable using templates
- ARM orchestrates ordering operations for you

## Templates
- Template files can be written to extend JSON and use functions provided by ARM. Templates have the following sections
  - Parameters - Allows templates to be used in different environments
  - Variables - Can be constructed from parameters
  - User-defined functions
  - Resources
  - Outputs from the deployed resources
- Templates are converted by ARM to REST API operations
- Templates can be deployed using the following
  - Azure portal
  - Azure CLI
  - PowerShell
  - REST API
  - Button in GitHub repository
  - Azure Cloud Shell
- Templates can be divided into reusable templates so that other templates can just link them together
- Template specs enable you to store a template as a resource type so that they can be shared. You can then use RBAC to manage access to the template spec. Users with read access can then deploy it but not change it.
- Resources can be deployed conditionally using the condition element with a value that resolves to true or false. Note that child resources must specify the same condition as it does not cascade.
  - Conditional deployments can be used to create new resources or use existing ones

## Deployment Modes
There are two modes used when deploying resources.
- Incremental update
  - Default mode
  - Resources inside a resource group that aren't in the template are left unchanged
  - Redeployments should include all properties of resources and incremental mode will interpret their absence as a change
- Complete update
  - Resources inside a resource group that aren't in the template are deleted

The deployment mode is set using a command like this
``` bash
az deployment group create \
  --mode Complete \
  --name ExampleDeployment \
  --resource-group ExampleResourceGroup \
  --template-file storage.json
```

## Deploying using ARM
https://docs.microsoft.com/en-us/learn/modules/create-deploy-azure-resource-manager-templates/6-create-deploy-resource-manager-template

Code can be found in [Code/Azure ARM](Code/Azure%20ARM/)

## Resources
https://docs.microsoft.com/en-us/azure/azure-resource-manager/templates/template-functions


# Container Registry
Azure Container Registry (ACR) is a managed, private Docker registry. It can support CI/CD pipelines and build automation.

ACR allows for deployments to various targets
- Orchestration systems (k8s and others)
- Azure services (Azure Kubernetes Service and others)

ACR has three tiers
- Basic - Cost optimized with throughput suited for lower usage
- Standard - Same capabilities as basic but with more throughput
- Premium - Highest amount of throughput and features like geo-replication, content trust, and private links

ACR supports the following images and artifacts
- Windows and Linux Docker images
- Helm charts
- Images built using the Open Container Initiative (OCI) Image Format Specification

## Storage Capabilities
- Images are encrypted at rest
- Data is stored in the region the registry is created
- Availability zones are used for Premium registries
- Repos, images, layers, and tags can be created up to the registry storage limit

## Tasks
ACR Tasks is a feature suite within ACR.
- Used for image building and automated patching for Docker containers
- Used for automated builds from source code updates, updates to a container's base image, or timers

The following tasks are supported
- Quick task - Builds and pushes a single container image to a registry on demand (docker build and docker push in the cloud)
- Automatically triggered tasks triggered by the following
  - Source code updates
  - Base image updates
  - Schedule
- Multi-step task - Specify build and push operations for one or more containers in a YAML file

## Creating an ACR and building and running an image using tasks
https://docs.microsoft.com/en-us/learn/modules/publish-container-image-to-azure-container-registry/6-build-run-image-azure-container-registry

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


# Azure Container Instances (ACI)
ACI offers a simple and fast way to run a container in Azure without managing VMs or higher-level services. It's a good solution with the following benefits
- Fast startup
- Container access - Containers are given an IP address and a FQDN
- Apps are isolated as they would be in a VM
- The minimum amount of customer data is stored to run the container
- ACI allows CPU cores and memory to be specified
- File storage uses Azure Files
- Supports Linux and Windows
- Deployments are typically handled through ARM templates(recommended when you need additional Azure services) or YAML (recommended when you're only deploying container instances)
- ACI allocates resources to a container group based on what each container instance has requested
- Container groups share an IP address and a port namespace on that IP address also
- External volumes can be mounted from the following places:
  - Azure file shares
  - Secrets
  - Empty directory
  - Cloned git repo

## Deploying a container instance with Azure CLI
1. Create a new group for ACI
``` bash
az group create --name az204-aci-patrick-rg --location eastus
```
2. Create a DNS name to expose the container to the internet and create the container
``` bash
DNS_NAME_LABEL=aci-example-$RANDOM

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

## Restart Policies
Containers can be created with the following restart policies
- Always - Default setting
- Never
- OnFailure

The restart poilcy is specified with the `--restart-policy` parameter when using `az container create`.

## Environment Variables
Environment variables are specified using the `--environment-variables` parameter when using `az container create`.

ACI supports secure values to pass secrets as environment variables

## Azure File Shares in Containers
By default containers are stateless. To persist state data must be stored externally.
- Azure Files shares are only supported by Linux containers
  - Requires the container to run as root
  - Limited to CIFS support