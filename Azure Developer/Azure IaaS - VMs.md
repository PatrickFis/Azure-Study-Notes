# VMs

## Availability
VMs have various options for ensuring availability

### Availability zones [MS Documentation on regions and availability zones](https://learn.microsoft.com/en-us/azure/reliability/availability-zones-overview?context=%2Fazure%2Fvirtual-machines%2Fcontext%2Fcontext)
Physically separate zones within an Azure region. Combo of a fault domain and an update domain.
-  Fault domains - Logical group of underlying hardware similar to a rack in an on-prem datacenter. VMs created in Azure are distributed across fault domains to minimize the impact of hardware failures.
-  Update domains - Logical group of underlying hardware that can undergo maintenance or rebooted at the same time. Azure distributes your VMs over update domains so that at least one instance is up at a time.
-  HA made possible through the following
   -  Zonal services - Resource pinned to a specific zone
   -  Zone-redundant services - Azure replicates things across zones

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
- Supports the use of Azure load balancer for layer-4 traffic distribution and Azure Application Gateway for layer-7 traffic distribution and TLS termination.

### Load Balancer [MS Documentation on Azure Load Balancer](https://learn.microsoft.com/en-us/azure/load-balancer/load-balancer-overview)
Azure Load Balancer is a layer 4 (TCP, UDP) load balancer that distributes incoming traffic among healthy VMs.
- Layer 4 of the OSI model is the single point of contact for clients.
- Public load balancers provide outbound connections for VMs inside your virtual network. Used to balance internet traffic to your VMs.
- Internal or private load balancers are used to balance traffic inside a virtual network. Can be connected to from an on-prem network in a hybrid scenario.

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