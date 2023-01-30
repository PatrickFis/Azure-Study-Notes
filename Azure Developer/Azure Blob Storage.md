# Blob Storage [MS Documentation on Blob Storage](https://learn.microsoft.com/en-us/azure/storage/blobs/storage-blobs-overview)
- Object storage solution optimized for storing massive amounts of unstructured data (text or binary data, images, logs, backups, etc)
- Can be accessed via a REST API, Azure PowerShell, Azure CLI, or the Azure Storage client library
- Two performance levels for storage accounts
  - Standard - general purpose, recommended for most things
  - Premium - uses SSDs
    - Note: You can choose between three account types for premium accounts: block blobs, page blobs, or file shares.
      - Block blobs: Used for block blobs and append blobs. Recommended for scenarios with high transaction rates or scenarios that use smaller objects or require low latency.
      - Page blobs: Stores only page blobs. [MS Documentation on page blobs](https://learn.microsoft.com/en-us/azure/storage/blobs/storage-blob-pageblob-overview?tabs=dotnet) 
        - Page blobs are collections of 512-byte pages which allow for reads/writes of arbitrary ranges of bytes.
        - Ideal for storing index-based and sparse data structures (OSes, data disks for VMs and DBs - note that Azure SQL DB uses page blobs as the underlying persistent store for data).
      - File shares: Stores only file shares. Recommended for enterprise or high-performance scale applications. Supports Server Message Block (SMB) and NFS file shares.
- Three access tiers which you can switch between at any time
  - Hot - Optimized for frequent access. High storage cost, low access cost. Default tier.
  - Cool - Optimized for storing lots of data that is infrequently accessed. Low storage cost, high access cost.
  - Archive - Available only for individual block blobs. Optimized for data that can tolerate several hours of retrieval latency. Most cost-effective option for storage, but accessing is more expensive than other tiers.
  - You can change between the access tiers at any time.
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
  - Supports delegated access through shared access signatures
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

## Working with blob storage
- Microsoft provides a client library for interacting with blobs. The classes are in Azure.Storage.Blobs.
- Important classes in the library:
  - BlobClient - Used to manipulate blobs
  - BlobClientOptions - Provides the client configuration options for connecting to Azure Blob Storage
  - BlobContainerClient - Used to manipulate containers and their blobs
  - BlobServiceClient - Used to manipulate service resources and blob containers. The storage account provides the top-level namespace for the blob service.
  - BlobUriBuilder - Provides a convenient way to modify the contents of a Uri instance to point to different Azure Storage resources like an account, container, or blob
- Blobs and containers support REST as well
  - Custom metadata can be represented as HTTP headers. Metadata headers can be set on a request that creates a new container or blob resource or one to create a property on an existing resource.
  - Metadata headers are name/value pairs in the following format: x-ms-meta-name:string-value
  - Names are case insensitive
  - Names must be unique. Duplicates will cause the blob service to return a error (HTTP 400 - Bad Request)
  - Metadata values can be retrieved or set directly without returning or altering the content of the resource
  - Metadata values can only be read/written in full. Partial updates aren't supported. Setting metadata on a resource overwrites any existing metadata values for that resource.
  - GET/HEAD requests retrieve metadata headers
  - PUT requests set metadata headers. This will overwrite existing metadata, and a PUT with no headers will clear all existing metadata on the resource.

## Useful Commands
Show a table of your storage accounts
``` shell
az storage account list --output table
```

Show the connection string for a storage account
``` shell
az storage account show-connection-string --name <name>
```

# Udemy Study Notes
- A storage account is a resource which provides storage in the cloud.
- Storage accounts have different types
  - Standard General Purpose V2: Standard storage account for blobs, file shares, queues, and tables
  - Premium block blobs: Supported for block and sppend blobs. Used when you want fast access to blobs with high transaction rates.
  - Premium Page blobs: See MS documentation, also remember that these are used to store virtual hard disks for VMs.
  - Premium file shares: Faster access for file shares, also used for high transaction rates.

## Creating storage accounts
- Creating storage accounts is fairly simple in Azure:
  - Create a resource as usual and select storage accounts
  - Give the resource a unique name
  - Select the region for the storage
  - Select if you'd like a standard storage account or one of the various premium account types
  - Select a redundancy option
  - For testing the other default options are fine
- After the storage account is created the Azure portal will display various different options that you can explore with some of the more important ones listed below:
  - Under the data storage section you can find containers, file shares, queues, and tables. This will give you access to these respective resources inside your storage account.
  - Under the security + networking section you can find access keys and shared access signatures to enable applications to connect and use your storage account.

## Azure Blob service
- Optimized for large amounts of unstructured data (images, videos, etc.)
- Organizes data inside containers (which can be thought of as a folder)
  - Data will be uploaded as binary objects into the container
  - You'll receive a unique URL for each object
- There are different types of blobs:
  - Block blobs: blocks of data that can be managed individually (text files, images, etc.)
  - Append blobs: block blobs that are optimized for append operations, useful for logging
  - Page blobs: used for virtual disks
- Containers can be created through the Azure portal
  - Objects can be uploaded through the portal or through a program accessing the container
  - Objects can be uploaded into folders inside the blob service
- There are various ways to access objects stored inside a container
  - If you're logged in to Azure you can simply download the resource through the portal
  - It can be accessed via the unique URL given by the container (the URL will contain details like the name of the storage account [just a subdomain off windows.net], the name of the service [blob.core.windows.net], the name of the container, and then the name of the object). By default this will give a 404 not found if accessed anonymously. To allow anonymous access you can change the access level of the container (to private [no anonymouse access], blob [read access to blobs in the container], or container [read access for containers and blobs]).


## Shared Access Signatures
- Objects inside a container can have a SAS generated for them. The following features are available:
  -  You can specify the start and end dates for the SAS. 
  -  You can restrict who can use the SAS by allowed IP addresses.
- Storage accounts themselves can have SAS created for them. The following features are available:
  - You can pick which services (blob, file, queue, or table) the client is allowed to use.
  - You can pick their allowed resource types (service, container, or object).
  - You can change their allowed permissions (read, write, delete, list, add, create, update, process, or immutable storage).
  - Various other permissions settings.
  - Start and end dates.
  - IP address restrictions.
  - Allowed protocols (HTTPS or HTTPS and HTTP).
- There are various types of SAS (see [MS Link](https://learn.microsoft.com/en-us/rest/api/storageservices/delegate-access-with-shared-access-signature) for more information).
  - Account SAS
    - You can delegate access to operations that apply to a service
    - You can delegate access to read, write, and delete operations on blob containers, tables, queues, and file shares that are not permitted with a service SAS
  - Service SAS
    - You can delegate access to just one storage service (blob, queue, table, or files)
    - Can reference a stored access policy to provide another level of control over a set of signatures. This includes the ability to modify or revoke access to the resource if necessary.
  - User delegation SAS
    - Secured with AAD credentials
    - Only supports blob storage
    - Can be used to grant access to containers and blobs
    - Can be revoked by doing the following:
      - Call the "Revoke User Delegation Keys" operation (which will invalidate any SASes that rely on the key). Then call the "Get User Delegation Key" operation again to create new SASes.
      - Change the RBAC role assignment for the security principal that's used to create the SAS. When a client uses the SAS to access a resource, Azure Storage verifies that the security principal whose credentials were used to secure the SAS has the required permissions to the resource.

## Stored Access Policy
- Containers can have access policies set on them.
  - Policies need names and a list of permissions along with a start and end date.
- Access policies can be used when generating SASes to quickly create one which follows the policy.
  - Changing access policies can invalidate SASes if they're updated.

## Managing storage access through AAD
The following steps will walk you through setting up a new AD account and granting them access to a storage account.
1. Navigate to AAD
2. Click on Users
3. Click on new user
4. Click on create new user
5. Give the account a username and password (storageaccounttemp Vabo9669zxx)
6. Click create
7. Login to the account in Storage Explorer
8. Note that you can't see any storage accounts on it
9. Navigate to the storage account you want to grant the account access to
10. Navigate to Access Control
11. Click Add
12. Click add role assignment
13. Select the Storage Account Contributor role
14. Click next
15. Click select members
16. Locate the account that you just created and select it
17. Assign the role
18. Go to Storage Explorer
19. Refresh it
20. Verify that you're now able to access the storage account

## Access Tiers
- There are 3 access tiers.
  - Hot: Data which is accessed frequently.
  - Cool: Data which is accessed infrequently and stored for at least 30 days.
  - Archive: Data which is rarely accessed and stored for at least 180 days.
- Access tiers can be set at the blob level. By default Azure sets things in the hot access tier, though this can be changed.
- Data stored in the archive tier must be rehydrated before it can be accessed. This can take significant amounts of time.

## Lifecycle Management Policies
- Lifecycle management policies are available to make moving blobs between tiers (or even deleting them) can be managed more easily. The following policies can be defined:
  - Transition blobs between tiers.
  - Delete blobs.
  - Apply policies to the entire storage account or to a subset of blobs.
- Lifecycle policies can have the following properties:
  - Filters can be applied for different blob types
  - Actions like tierToCool, tierToArchive, and delete can be performed
  - Rules are supported for blob and append blobs in general-purpose V2 accounts, premium block blob and blob storage accounts

## Blob Versioning
- Allows you to maintain previous versions of a blob
- Previous versions can be restored
- Each blob gets an initial ID which is updated alongside the blob
- Blob versioning can be enabled or disabled at any time
- Blob versioning is available in the the portal when viewing a storage account. It's available by clicking on data protection under the data management section. After enabling it you will see a versions tab displayed when viewing a blob in the portal. Other options are then made available to set a blob to a previous version.
- If blob versioning is disabled you will still be able to access versions of blobs that were created while versioning was enabled.

## Blob Snapshots
- Blob snapshots are available when viewing blobs in the portal
- Snapshots create a copy of a blob at a point in time
- Snapshots can be promoted to the blob to restore it to the version in the snapshot
- The main difference between snapshots and versioning is scale. Versioning is applied to all blobs in your storage account (and incurs higher costs). Snapshots can be taken at an individual blob level.

## Soft Delete
- Safeguards against deletions by allowing a retention period of 1-365 days to be set
- Data will be available even after deleted or overwritten
- During the retention period you can also restore a blob's snapshot
- The retention period can be changed at any time
- Soft delete is available in the data protection blade on a storage account
- By default deleted blobs are kept for 7 days
- In the portal a toggle is available inside containers that shows deleted blobs

## Lab Notes
- The lab makes use of the Azure.Storage.Blobs dependency
- The storage account used for the labs is az204patrickstorage
- Metadata can be added to blobs through the Azure portal and used by your program
- I skipped doing the labs for table storage. I use table storage at work.

# Azure Blob Storage REST API [MS Documentation](https://learn.microsoft.com/en-us/rest/api/storageservices/blob-service-rest-api)
- https://learn.microsoft.com/en-us/rest/api/storageservices/naming-and-referencing-containers--blobs--and-metadata#resource-uri-syntax has a reference for URI syntax for endpoints:

> Each resource has a corresponding base URI, which refers to the resource itself.
> For the storage account, the base URI includes the name of the account only:
>
> https://myaccount.blob.core.windows.net
>
> For a container, the base URI includes the name of the account and the name of the container:
>
> https://myaccount.blob.core.windows.net/mycontainer
>
> For a blob, the base URI includes the name of the account, the name of the container, and the name of the blob:
> 
> https://myaccount.blob.core.windows.net/mycontainer/myblob
>
> The URI for a snapshot of a blob is formed as follows:
>
> https://myaccount.blob.core.windows.net/mycontainer/myblob?snapshot=`<DateTime>`

## Storage Account
- The storage account can be manipulated using the REST API with requests sent to https://myaccount.blob.core.windows.net/ with additional URI parameters
  - Containers can be listed with a GET request and an additional URI parameter: ?comp=list
  - Blob service properties (things like properties for storage analytics, CORS, and soft delete settings) can be manipulated with additional URI parameters: ?restype=service&comp=properties
    - A PUT request will set the properties
      - A request body in XML is used to set the properties
      ``` xml
      <?xml version="1.0" encoding="utf-8"?>  
      <StorageServiceProperties>  
          <Logging>  
              <Version>version-number</Version>  
              <Delete>true|false</Delete>  
              <Read>true|false</Read>  
              <Write>true|false</Write>  
              <RetentionPolicy>  
                  <Enabled>true|false</Enabled>  
                  <Days>number-of-days</Days>  
              </RetentionPolicy>  
          </Logging>  
          <Metrics>  
              <Version>version-number</Version>  
              <Enabled>true|false</Enabled>  
              <IncludeAPIs>true|false</IncludeAPIs>  
              <RetentionPolicy>  
                  <Enabled>true|false</Enabled>  
                  <Days>number-of-days</Days>  
              </RetentionPolicy>  
          </Metrics>  
          <!-- The DefaultServiceVersion element can only be set for the Blob service and the request must be made using version 2011-08-18 or later -->  
          <DefaultServiceVersion>default-service-version-string</DefaultServiceVersion>  
      </StorageServiceProperties>
      ```
    - A GET request will retrieve the properties
  - Blob service statistics can be retrieved for accounts with geo-redundant replication with a GET request with additional URI parameters: ?restype=service&comp=stats
  - Account information can be retrieved by sending GET/HEAD requests with additional URI parameters: ?restype=account&comp=properties
    - This operation can include another parameter: &sv=`<SAS token value>`
    - This operation can be done for containers or individual blobs as well by specifying /mycontainer or /mycontainer/myblob with the same URI parameters
  - User delegation SASes can be retrieved by sending a POST request with additional URI parameters: ?restype=service&comp=userdelegationkey
    - A request body in XML is used to set the start time and expiry time of the SAS
    ``` xml
    <?xml version="1.0" encoding="utf-8"?>  
    <KeyInfo>  
        <Start>String, formatted ISO Date</Start>
        <Expiry>String, formatted ISO Date </Expiry>
    </KeyInfo>  
    ```
  - All requests use the common headers noted below

## Containers
- Containers are manipulated using the REST API with requests sent to https://myaccount.blob.core.windows.net/mycontainer?restype=container
  - Containers can be created by making a PUT request with the following headers:
    - The common headers noted below
    - x-ms-blob-public-access:`<container, blob>` - Optional. Specifies whether data in the container can be accessed publicly and the level of access. Container will allow full public read access and allow for clients to enumerate blobs in the container via anonymous request, but not enumerate containers in the storage account. Blob will allow public read access for blobs. Clients can't enumerate data within the container via anonymous request.
  - Containers can be deleted by making a DELETE request with the following headers:
    - The common headers noted below
  - Container properties can be retrieved with a GET/HEAD request with the following headers:
    - The common headers noted below
  - Container metadata can be manipulated with an additional URI parameter: &comp=metadata
    - A GET/HEAD request will retrieve the metadata when sent with the following headers:
      - The common headers noted below
    - A PUT request will set metadata when sent with the following headers:
      - The common headers noted below
      - x-ms-meta-name:value - Optional. A name-value pair to associate with the container as metadata. Each call to this operation replaces all existing metadata attached to the container. To remove all metadata from the container, call this operation with no metadata headers.
  - Blobs inside a container can be listed by making a GET request with an additional URI parameter: &comp=list. The following headers can be sent:
    - The common headers noted below
    - A number of additional URI parameters can be specified to filter results

## Blobs
### Storing, Retrieving, Copying and Deleting Blobs
- Blobs can be stored and retrieved by making requests to https://myaccount.blob.core.windows.net/mycontainer/myblob
  - A PUT request will store a blob in your container when sent with the following headers
    - The common headers noted below
    - Content-Length - Required. The length of the request. For page or append blobs the value must be set to 0.
    - x-ms-blob-type: `<BlobBlob | PageBlob | AppendBlob>` - Required to specify the type of blob to be created in the storage account.
  - A PUT request can also be used to copy a blob when sent with the following headers
    - The common headers noted below
    - x-ms-copy-source:name - Required. Specifies the name of the source blob or file. Can be a URL which specifies the blob (though it must be URL encoded).
  - A GET request will read or download the blob from Azure. The response will include metadata and properties. The following headers are required:
    - The common headers noted below
    - The common URI parameters noted below are supported
  - A DELETE request will delete the blob from your storage account. The following headers are required:
    - The common headers noted below
    - x-ms-delete-snapshots: {include, only} - Required if the blob has associated snapshots. Include will delete the base blob and all of its snapshots while only will delete only the blob's snapshots and not the blob itself.
    - The common URI parameters noted below are supported
### Blob Metadata
- Blob metadata can be manipulated by making requests to https://myaccount.blob.core.windows.net/mycontainer/myblob?comp=metadata
  - A PUT request will store metadata when sent with the following headers:
    - The common headers noted below
    - x-ms-meta-name:value - Optional. Sets a name-value pair for the blob. Each call to this operation replaces all existing metadata attached to the blob. To remove all metadata from the blob, call this operation with no metadata headers.
  - A GET request will retrieve metadata when sent with the following headers:
    - The common headers noted below
    - The common URI parameters noted below are supported

### Common headers in requests
- There are common headers that should be on most requests:
  - Authorization - Required. Specifies the authorization scheme, account name, and signature.
  - Date or x-ms-date - Required. Specifies the Coordinated Universal Time (UTC) for the request.
  - x-ms-version - Required for all authorized requests. Specifies the version of the operation to use for this request.

### General Notes
- In general, the REST API operates by having requests to manipulate blobs sent to https://myaccount.blob.core.windows.net/mycontainer/myblob with a URI parameter of comp=`<operation>` for operations that aren't storing, getting, deleting, or copying a blob.

## Common URI parameters
- snapshot - Optional. The snapshot parameter is an opaque DateTime value that, when it's present, specifies the blob snapshot to be retrieved.
- versionid -	Optional, version 2019-12-12 and later. The versionid parameter is an opaque DateTime value that, when present, specifies the version of the blob to be retrieved.
- timeout	- Optional. The timeout parameter is expressed in seconds.

# Misc
- See https://learn.microsoft.com/en-us/shows/exam-readiness-zone/preparing-for-az-204-develop-for-azure-storage-segment-2-of-5 for information on what may appear on the exam.
  - The exam will assume that you know replication options and blob storage setup
  - Understand what the client libraries look like (see code examples) and its major classes as well as main methods
  - Be aware of how to create metadata (SDK and REST API)
    - It sounds like common questions come up around the REST API. It would be good to review the various API operations.
  - Be aware of the 3 access tiers as well as their advantages
  - Be aware of lifecycle management and how to transition a blob from one tier to another
  - Be aware of lifecycle policies and how to establish them