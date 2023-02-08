# VMs

## Design considerations
- There are various aspects of VMs that are important to think about before creating one:
  - Availability: Azure provides a 99.9% SLA for a single VM with premium storage for all disks.
  - VM size: Sizing is going to be dependent on your workload. Size will affect things like processing power, memory, and storage capacity.
  - VM limits: There's a default quota of 20 VMs per region (which can be raised with a support ticket).
  - VM image: You can bring your own image or use one from the Azure Marketplace.
  - VM disks: The type of disk you use determines the performance level and the storage account type that contains the disks. 
    - Azure provides two types of disks:
      - Standard disks: Backed by HDDs. Cost-effective, ideal for dev/test workloads.
      - Premium disks: Backed by SSDs. Good for production workloads.
    - Azure provides two options for disk storage:
      - Managed disks: This is the recommended model from Azure. You specify the size of the disk you want (up to 4 TB) and Azure creates it and manages the disk and the storage. Note that this way lets you not worry about storage account limits and is easier to scale than unmanaged disks.
      - Unmanaged disks: You're responsible for the storage accounts holding virtual hard disks (VHDs) that correspond to VM disks. Note that a single storage account has a fixed-rate limit of 20,000 I/O operations per second, so your storage account is only capable of supporting 40 standard VHDs at full utilization. Scaling out requires more storage accounts which can get complicated.
  - VM extensions
    - Windows VMs have extensions that configure things after the VM is deployed. Common tasks like the following can be done through extensions:
       - Run custom scripts: The Custom Script Extension will run your script on a VM when it is provisioned.
       - Deploy and manage configurations: The PowerShell Desired State Configuration (DSC) Extension helps set up and manage configurations and environments.
       - Collect diagnostics data: The Azure Diagnostics Extension helps configure your VM to diagnostics data.
    - Linux VMs support cloud-init across most Linux distros that support it and works with all the major automation tools like Ansible, Chef, SaltStack, and Puppet.

## Availability
VMs have various options for ensuring availability

### Availability zones [MS Documentation on regions and availability zones](https://learn.microsoft.com/en-us/azure/reliability/availability-zones-overview?context=%2Fazure%2Fvirtual-machines%2Fcontext%2Fcontext)
Physically separate zones within an Azure region. Combo of a fault domain and an update domain.
-  Fault domains - Logical group of underlying hardware similar to a rack in an on-prem datacenter. VMs created in Azure are distributed across fault domains to minimize the impact of hardware failures.
-  Update domains - Logical group of underlying hardware that can undergo maintenance or be rebooted at the same time. Azure distributes your VMs over update domains so that at least one instance is up at a time.
-  Azure services which support Availability Zones fall into two categories:
   -  Zonal services - Resource pinned to a specific zone (VM, managed disks, standard IP addresses, etc)
   -  Zone-redundant services - Services which Azure replicates across zones (zone-redundant storage, SQL databases)

### Availability Sets [MS Documentation on availability sets](https://learn.microsoft.com/en-us/azure/virtual-machines/availability-set-overview)
Logical grouping of VMs used by Azure to provide redundancy and availability to your application. Composed of fault domains and update domains. 
- No cost to availability sets themselves. The costs come from each VM instance that you create.
- Availability sets can be configured with up to 3 fault domains and 20 update domains.
- Fault Domain: Groups of VMs that share a common power source and network switch. Similar to a rack in an on-prem datacenter.
  - Limits the impact of hardware failures.
  - Disk fault domains are used for managed disks that are attached to VMs (the disks are within the same fault domain). Note that VMs with managed disks can only be created in a managed availability set with the number of managed disk fault domains varying by region.
- Update Domain: Groups of VMs and underlying physical hardware that can be rebooted at the same time. 
  - The order that they are rebooted during planned maintenance may not be sequential.
  - Only one domain is rebooted at a time. A domain is given 30 minutes to recover before maintenance proceeds to the next one.
  - Example of usage with 5 update domains: 6th VM created goes in the first domain, 7th in the second, and so on.

### VM Scale Sets [MS Documentation on VM scale sets](https://learn.microsoft.com/en-us/azure/virtual-machine-scale-sets/overview?context=%2Fazure%2Fvirtual-machines%2Fcontext%2Fcontext)
- Group of load balanced VMs. VM instances increase or decrease in response to demand or on a defined schedule.
- Benefits
  - Easy to create and manage multiple VMs
    - Maintains a consistent config across VMs
  - Provides high availability by using availability zones and sets
  - Automatic scaling
  - Large-scale
    - Supports up to 1000 VM instances for standard marketplace and custom images. Note that using a managed image that the limit is reduced to 600 VM instances.
- Two modes: Uniform and Flexible [Documentation](https://learn.microsoft.com/en-us/azure/virtual-machine-scale-sets/virtual-machine-scale-sets-orchestration-modes)
  - Uniform
    - Optimized for large-scale workloads with identical instances.
    - Use a VM profile or template to scale up to the desired capacity.
    - Individual uniform VM instances are exposed via the Virtual Machine Scale Set VM API and are not compatible with the standard IaaS VM API commands.
  - Flexible
    - Allows for identical or multiple VM types.
    - Spreads VMs across fault domains or an Availability Zone.
- Supports the use of Azure load balancer for layer-4 traffic distribution and Azure Application Gateway for layer-7 traffic distribution and TLS termination.

### Load Balancer [MS Documentation on Azure Load Balancer](https://learn.microsoft.com/en-us/azure/load-balancer/load-balancer-overview)
Azure Load Balancer is a layer 4 (TCP, UDP) load balancer that distributes incoming traffic among healthy VMs.
- Layer 4 of the OSI model is the single point of contact for clients.
- Public load balancers provide outbound connections for VMs inside your virtual network. Used to balance internet traffic to your VMs.
- Internal or private load balancers are used to balance traffic inside a virtual network. Can be connected to from an on-prem network in a hybrid scenario.

## VM Sizing
| VM Type                  | Description                                                                                                                                                               | Example Applications                                                                                                                                                                                                                                 |
| ------------------------ | ------------------------------------------------------------------------------------------------------------------------------------------------------------------------- | ---------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------- |
| General Purpose          | Balanced CPU to memory ratio. Ideal for testing and development, small to medium DBs, and low to medium traffic web servers.                                              | Enterprise grade apps, dev/test environments, entry-level and mid-range DBs, web servers, analytics, virtual desktops, e-commerce systems, apps requiring encryption of data while in use (confidential computing).                                  |
| Compute Optimized        | High CPU to memory ratio. Good for medium traffic web servers, network appliances, batch processes, and application servers.                                              | Web servers, network appliances, batch processes, app servers, video encoding and rendering, gaming apps, and AI inferencing scenarios.                                                                                                              |
| Memory Optimized         | High memory to CPU ratio. Great for relation DBs, medium to large caches, and in-memory analytics.                                                                        | Large relational database servers, data warehousing and business intelligence applications, in-memory analytics workloads, and additional business-critical applications including systems that process financial transactions of various nature.    |
| Storage Optimized        | High disk throughput and IO for Big Data, SQL, NoSQL DBs, data warehousing and large transactional databases.                                                             | Apps requiring high disk throughput, big data apps, NoSQL databases, large transactional databases, enterprise search engines, and data warehousing solutions.                                                                                       |
| GPU                      | Specialized VMs for heavy graphic rendering and video editing, as well as model training and inferencing (ND) with deep learning. Available with single or multiple GPUs. | Specialized workloads requiring heavy graphic rendering, video editing, remote visualization, and machine learning, training, and inference scenarios for deep learning.                                                                             |
| High Performance Compute | Fastest and most powerful CPUs with optional high-throughput network interfaces (RDMA).                                                                                   | Apps requiring dense computation, such as implicit finite element analysis, molecular dynamics, and computational chemistry; as well as apps driven by memory bandwidth, such as fluid dynamics, finite element analysis, and reservoir simulations. |

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

# Misc
- See https://learn.microsoft.com/en-us/shows/exam-readiness-zone/preparing-for-az-204-develop-azure-compute-solutions-1-of-5 for information on what may appear on the exam.
  - Be familiar with sizing options and use cases for choosing a particular one and possibly extensions
  - Be familiar with availability zones vs availability sets
  - Scale sets and load balancers may show up, but they probably won't be the focus of a question
  - Be aware of fault domains and update domains and how they may impact the rolling out of a particular application that spans multiple VMs

## ARM Notes for VMs
- The various options available in ARM for VMs are located here: https://learn.microsoft.com/en-us/azure/templates/Microsoft.Compute/virtualMachines?pivots=deployment-language-arm-template