# Container Registry [MS Documentation on ACR](https://learn.microsoft.com/en-us/azure/container-registry/container-registry-intro)
Azure Container Registry (ACR) is a managed, private Docker registry. It can support CI/CD pipelines and build automation.

ACR allows for deployments to various targets
- Orchestration systems (k8s and others)
- Azure services (Azure Kubernetes Service and others)

ACR has three tiers
- Basic - Cost optimized with throughput suited for lower usage
- Standard - Same capabilities as basic but with more storage and throughput
- Premium - Highest amount of storage and throughput and features like geo-replication, content trust, and private links

ACR supports the following images and artifacts
- Windows and Linux Docker images
- Helm charts
- Images built using the Open Container Initiative (OCI) Image Format Specification

## Storage Capabilities
- Images are encrypted at rest
- Data is stored in the region the registry is created
- Availability zones are used for Premium registries
- Repos, images, layers, and tags can be created up to the registry storage limit

## ACR Tasks
ACR Tasks is a feature suite within ACR.
- Used for image building and automated patching for Docker containers
- Used for automated builds from source code updates, updates to a container's base image, or timers

The following tasks are supported
- Quick task - Builds and pushes a single container image to a registry on demand (docker build and docker push in the cloud)
  - Can be used to provide an integrated development environment by offloading container image builds to Azure
- Automatically triggered tasks triggered by the following
  - Source code updates
  - Base image updates
  - Schedule
- Multi-step task - Specify build and push operations for one or more containers in a YAML file

Each ACR Task has an associated source code context (the location of the source required to build the container image like a Git repo or a local filesystem).

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


# Azure Container Instances (ACI) [MS Documentation on ACI](https://learn.microsoft.com/en-us/azure/container-instances/container-instances-overview)
ACI offers a simple and fast way to run a container in Azure without managing VMs or higher-level services. It's a good solution with the following benefits
- Fast startup - Containers can start quickly since you don't need to deal with provisioning a VM
- Container access - Containers are given an IP address and a FQDN so they can be exposed directly to the internet
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