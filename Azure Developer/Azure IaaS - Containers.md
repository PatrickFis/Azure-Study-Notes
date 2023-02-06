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
  - This uses similar syntax to `docker build`: the command to run it from an Azure CLI is `az acr build`
- Automatically triggered tasks triggered by the following
  - Source code updates
    - This can be used with the `az acr task create` command to configure builds to happen from updates to Git repos
    - This will require a personal access token (PAT) to set up the webhook used by ACR in GitHub or Azure DevOps repos.
  - Base image updates
    - Base images are parent images on which one or more application images are based
    - An ACR task can be created to track a dependency on a base image. When updates are pushed to your registry or the base image is updated in a public repo, the task can build any application images based on the base image.
  - Schedule
    - This is useful for running container workloads on a schedule
    - This can also be used for running maintenance operations or tests on images pushed regularly to your registry
- Multi-step task
  - Specify build and push operations for one or more containers in a YAML file
  - There are three task steps available in a multi-step task:
    - build: Build one or more container images using `docker build` syntax
    - push: Push built images to a container registry (can be ACR or a public one like Docker Hub)
    - cmd: Run a container like using `docker run` (supports passing parameters)
    - Example:
      ``` yml
      version: v1.1.0
      steps:
        - id: build-web
          build: -t $Registry/hello-world:$ID .
          when: ["-"]
        - id: build-tests
          build: -t $Registry/hello-world-tests ./funcTests
          when: ["-"]
        - id: push
          push: ["$Registry/helloworld:$ID"]
          when: ["build-web", "build-tests"]
        - id: hello-world-web
          cmd: $Registry/helloworld:$ID
        - id: funcTests
          cmd: $Registry/helloworld:$ID
          env: ["host=helloworld:80"]
        - cmd: $Registry/functions/helm package --app-version $ID -d ./helm ./helm/helloworld/
        - cmd: $Registry/functions/helm upgrade helloworld ./helm/helloworld/ --reuse-values --set helloworld.image=$Registry/helloworld:$ID
      ```
- Each ACR Task has an associated source code context (the location of the source required to build the container image like a Git repo or a local filesystem).

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

## ACI Documentation from Microsoft [link](https://docs.microsoft.com/en-us/azure/container-instances/) (These notes were from work, see if they can be merged into the section above)
### Container Groups [reference](https://docs.microsoft.com/en-us/azure/container-instances/container-instances-container-groups)
- Container groups are the top-level resource in ACI
- Container groups are collections of containers that get scheduled on the same host machine (similar to a pod in k8s)
- Multi-container groups are currently only supported for Linux containers
- Multi-container groups are commonly deployed using ARM templates (recommended when you need to deploy additional Azure resources) or yaml files (recommended if you're only deploying container instances)
  - Container group's configuration can be exported to a yaml file using the CLI command `az container export`
- ACI allocates resources to a multi-container group by adding the resource requests of the instances in the group (so if you have a group creating two container instances that each requests 1 CPU the entire group is allocated 2 CPUs)
  - Resource usage may be different than the maximum resources requested if resource limits are configured. This lets an instance use resources up to the configured limit. Resource usage by other container instances in the group may decrease because of this.
- Container groups share an external-facing IP address, 1 or more ports on that IP address, and a DNS label with a fully qualified domain name (FQDN). External clients can reach the container if you expose a port on the IP address. The IP and FQDN are released when the container group is deleted.
  - Containers inside a group can reach each other via localhost on any port (even if they aren't exposed externally)
  - Container groups can be deployed within an Azure VNet to allow them to securely communicate with other resources in the VNet
- External volumes can be mounted within a container group: Azure file shares, secrets, empty directories, and cloned git repos

#### Co-scheduled groups
- Co-scheduled/multi-container groups share a host machine, local network, storage, and lifecycle.
- The following example can be used to deploy a multi-container group with a yaml file:
  ``` yaml
  apiVersion: 2019-12-01
  location: eastus
  name: myContainerGroup
  properties:
    containers:
    - name: aci-tutorial-app
      properties:
        image: mcr.microsoft.com/azuredocs/aci-helloworld:latest
        resources:
          requests:
            cpu: 1
            memoryInGb: 1.5
        ports:
        - port: 80
        - port: 8080
    - name: aci-tutorial-sidecar
      properties:
        image: mcr.microsoft.com/azuredocs/aci-tutorial-sidecar
        resources:
          requests:
            cpu: 1
            memoryInGb: 1.5
    osType: Linux
    ipAddress:
      type: Public
      ports:
      - protocol: tcp
        port: 80
      - protocol: tcp
        port: 8080
  tags: {exampleTag: tutorial}
  type: Microsoft.ContainerInstance/containerGroups
  ```
- The yaml file can be deployed using the following command:
  ``` bash
  az container create --resource-group myResourceGroup --file deploy-aci.yaml

  # The deployment state can be viewed using the following command:
  az container show --resource-group myResourceGroup --name myContainerGroup --output table
  ```
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

# This can be used to do things like launch a Bash shell like the following example
az container exec --resource-group myResourceGroup --name mynginx --exec-command "/bin/bash"
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


# Studying from Udemy
## Why use containers?
- Let's say you want to install two separate apps on one virtual machine. One app could have a dependency on a third party library that conflicts with a library that the other app uses. It's difficult to isolate applications on the same virtual machine.
- Containers allow you to package your app along with it's libraries, frameworks, dependencies, etc. and deploy them to your VM. This helps you avoid having one app impact other apps. This also makes your app portable since you can just move it to another VM as long as it has your container runtime.

## What is Docker?
- An open platform for developing, shipping, and running apps in an app in an isolated environment called a container.
- Requires the Docker runtime
  - Allows containers to interact with underlying resources on the host machine
  - Images are the template used to create a Docker container
  - Containers are the runnable instances of images

## Containerizing a .NET application
- The code used for this is located in [Code/Visual Studio Projects/UdemyWebApp/](Code/Visual%20Studio%20Projects/UdemyWebApp/). This project was deployed earlier to an Azure App Service plan, but it can run inside a Docker container instead.
- The above code was turned into a Docker image by using Visual Studio's built in "Add Docker Support" functionality.
  - The course on Udemy had instructions for using a Linux VM to create the image, but I just built it on my personal computer since I have WSL2 and Docker Desktop already setup.

## Azure Container Registry
- A basic Container Registry was created to push images to.
  - I had to make the registry in Visual Studio since Visual Studio couldn't find the one I made directly in the portal for some reason.
- The course on Udemy has you publish it using the Azure CLI from a Linux VM, but I don't need to do that with my personal computer's setup.

### Azure Container Registry Permissions
Credit: https://learn.microsoft.com/en-us/azure/container-registry/container-registry-roles?tabs=azure-cli
| Role/Permission | Access Resource Manager | Create/delete registry | Push image | Pull image | Delete image data | Change policies | Sign images |
| --------------- | ----------------------- | ---------------------- | ---------- | ---------- | ----------------- | --------------- | ----------- |
| Owner           | X                       | X                      | X          | X          | X                 | X               |             |
| Contributor     | X                       | X                      | X          | X          | X                 | X               |             |
| Reader          | X                       |                        |            | X          |                   |                 |             |
| AcrPush         |                         |                        | X          | X          |                   |                 |             |
| AcrPull         |                         |                        |            | X          |                   |                 |             |
| AcrDelete       |                         |                        |            |            | X                 |                 |             |
| AcrImageSigner  |                         |                        |            |            |                   |                 | X           |

## Azure Container Instances
- ACI provides a fast and easy way to deploy containers
- Don't need to manage container's infrastructure
- ACI can have a public IP and DNS name
- Files can be persisted to Azure File Shares

### Deploying from our registry
- The admin user must be enabled on the registry to deploy a container instance.
  - This can be enabled in Settings -> Access keys -> toggle the Admin user setting.
- Creating a new container instance from the portal:
  - Locate Container Instances in the marketplace
  - Choose your resource group, name, region, etc
  - Select Azure Container Registry as your image source
  - Select the image you would like to use from your registry
  - Other values can be left as their default values
- After creating a container the instance can be found in the Container Instances resource in your resource group
  - The Containers blade under settings has a list of running containers in your resource
    - This blade also contains information about events (like starting the container, pulling the image, etc)
    - This blade contains container properties (memory, CPU, etc)
    - This blade contains the logs for your container
    - This blade provides a utility to connect to your container (like using docker exec to get inside the container)

## Azure Container Groups (Consider coming back to this)
- I just followed along with Udemy on this. Consider coming back to this.
- This has to be deployed using yaml or an ARM template.

## Azure Kubernetes
- Fully managed k8s service in Azure.
- Created a cluster using a dev/test pricing with a single node. Integrated with my existing registry.
- Deployments can be done by providing yaml through the portal. I made use of the yaml for Nginx as an example from https://kubernetes.io/docs/tasks/run-application/run-stateless-application-deployment/.
  - A load balancer was applied to make this publicly accessible:
``` yml
apiVersion: v1
kind: Service
metadata:
  name: nginxservice
spec:
  selector:
    app: nginx
  ports:
    - protocol: TCP
      port: 80
      targetPort: 80
  type: LoadBalancer
```

# Misc
- See https://learn.microsoft.com/en-us/shows/exam-readiness-zone/preparing-for-az-204-develop-azure-compute-solutions-1-of-5 for information on what may appear on the exam.
  - Be familiar with ACR and how to roll it out using the Portal, az cli, and Powershell
  - Be familiar with ACR's tier options and where they can be used as well as their advantages
  - Be familiar with ACR's tasks and build capabilities
  - Be familiar with ACI's features
  - Be familiar with how to deploy ACI
  - Be familiar with how to connect into ACI as well as the ability to deploy more than a single container with co-scheduled groups