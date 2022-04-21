# Blob Storage
- Object storage solution optimized for storing massive amounts of unstructured data (text or binary data, images, logs, backups, etc)
- Two performance levels for storage accounts
  - Standard - general purpose, recommended for most things
  - Premium - uses SSDs
- Three access tiers which you can switch between at any time
  - Hot - Optimized for frequent access. High storage cost, low access cost. Default tier.
  - Cool - Optimized for storing lots of data. Low storage cost, high access cost.
  - Archive - Available only for individual block blobs. Optimized for data that can tolerate several hours of retrieval latency. Most cost-effective option for storage, but accessing is more expensive than other tiers.
- Three resource types
  - Storage account - Unique namespace in Azure for your data. Provides an endpoint for the objects in the storage account.
  - Container in the storage account - Similar to a directory in a file system but used for organizing blobs.
  - Blob
    - Block blobs - Text and binary data
    - Append blobs - Made up of blocks and optimized for append operations. Ideal for things like logging data
    - Page blobs - Stores random access files. Used to store virtual hard drive (VHD) files and serve as disks for Azure VMs
- Security options
  - Automatically encrypted
    - Uses AES256 transparently
    - Encryption keys can be managed by Microsoft. You can also manage your own keys or provide a key.
  - Can be controlled with AAD and RBAC
  - Can be secured in transit to apps
  - Can encrypt OS and data disks
  - Supports delegated access
- Redundancy options
  - Multiple copies of data are always stored
  - Primary region (3 replicated copies) support two options for replication
    - Locally redundant storage (LRS) - Copies data 3 times within the same physical location
    - Zone-redundant storage (ZRS) - Uses availability zones
  - Secondary region supports two options
    - Geo-redundant storage (GRS) - 3 copies made in primary region using LRS and copied to a single location in a secondary region (also LRS)
    - Geo-zone-redundant storage (GZRS) - Copies data across 3 availability zones in the primary region using ZRS and then copied to a single physical location in a secondary region using LRS

## Creating storage accounts
While this can be created in the Azure portal by searching for storage accounts, it may be more helpful to create a storage account through Azure Cloud Shell.
1. Open up a Cloud Shell
2. Create a new resource group
``` shell
az group create --name az204-blob-patrick-rg --location eastus
```
3. Create a blob block storage account
``` shell
az storage account create --resource-group az204-blob-patrick-rg --name \
az204blobpatrick --location eastus \
--kind BlockBlobStorage --sku Premium_LRS
```
4. Clean up the resource group when you're done
``` shell
az group delete --name az204-blob-patrick-rg --no-wait
```

## Storage Lifecycle
- General Purpose v2 and Blob storage accounts allow for rule based policies to transition data between access tiers. These rules can be used to manage expenses by moving data to cooler storage tiers when it's not used as frequently.
- Lifecycle policies are set using collections of rules in JSON
- Data in the archive tier is considered to be offline and can't be read or modified. In order to do anything with the data it must be rehydrated into the hot or cool tier. This can be done by copying to date into another tier (recommended for most scenarios) or by changing the blob's access tier.
  - A priority can be set for the rehydration operation by using the x-ms-rehydrate-priority header. Standard priority may take up to 15 hours and high priority will be prioritized over standard and may complete in under an hour for objects under 10 GB in size.
  - Note that changing a blob's tier doesn't affect last modified times and can result in data being archived after rehydration.

## Working with blob storage (come back and add documentation about .NET and REST)
https://docs.microsoft.com/en-us/learn/modules/work-azure-blob-storage/3-develop-blob-storage-dotnet has examples of using a .NET SDK for this.

## Useful Commands
Show a table of your storage accounts
``` shell
az storage account list --output table
```

Show the connection string for a storage account
``` shell
az storage account show-connection-string --name <name>
```