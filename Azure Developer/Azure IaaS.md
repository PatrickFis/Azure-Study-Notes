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