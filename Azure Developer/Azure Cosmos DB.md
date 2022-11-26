# Cosmos DB [MS Documentation on Cosmos DB](https://learn.microsoft.com/en-us/azure/cosmos-db/introduction)
Cosmos DB is a globally distributed database that is designed for low latency and elastic scalability. It has a 99.999% read and write availability SLA.

## Resource Hierarchy
- Your Azure Cosmos account is the unit of global distribution and high availability
  - Account contains a unique DNS name for management through the portal, CLI, or SDKs
  - By default 50 Azure Cosmos accounts can be created under a subscription (though it can be increased via a support request)
- An Azure Cosmos container is the unit of scalability
  - Provides provisioned throughput (RU/s)
  - Containers are horizontally partitioned and then replicated across multiple regions
  - Items added to the container are grouped into logical partitions and distributed across physical partitions based on the partition key
  - Throughput has two modes
    - Dedicated provisioned throughput mode - Throughput is reserved for that container and backed by SLAs
    - Shared provisioned throughput mode - Throughput is shared with other containers in the same database
  - Containers are schema agnostic containers of items
    - Items can represent a document in a collection, a row in a table, or a node or edge in a graph. This depends on the API you're using to interact with Cosmos.
    - Items with different schemas can be placed in the same container
    - All items in a container are automatically indexed without requiring explicit indexes or schema management
- An Azure Cosmos database is the unit of management for a set of Cosmos containers
  - Databases are analogous to namespaces

## Consistency Levels (reference MS Documentation for more info) [MS Documentation on Consistency Levels](https://learn.microsoft.com/en-us/azure/cosmos-db/consistency-levels)
- Cosmos DB provides five different consistency options and custom options as well
  - Strong
    - Offers a linearizability (meaning servicing requests concurrently) guarantee.
    - Reads are guaranteed to return the most recent committed version of an item.
    - Clients never see uncommitted or partial writes.
  - Bounded staleness
    - Recommended for apps requiring strong consistency
    - Reads are guaranteed to honor the consistent-prefix guarantee. Reads might lag behind writes by at most "K" versions (aka "updates") of an item or by "T" time interval, whichever is reached first.
    - Reads performed within a region that accepts writes offers consistency identical to strong consistency
    - As the staleness window approaches the service will throttle new writes to allow replication to catch up and honor the consistency guarantee.
  - Session
    - Recommended for many real world scenarios
    - Provides write latencies, availability, and read throughput similar to eventual consistency while also offering consistency guarantees that suit the needs of applications that operate in the context of a user.
  - Consistent prefix
    - Updates made as single document writes see eventual consistency.
    - Updates made as a batch within a transaction are returned consistent to the transaction in which they are committed.
    - Write operations within a transaction of multiple documents are always visible together.
  - Eventual
    - No ordering guarantee for reads. Without further writes the replicas eventually converge.
    - Ideal when an application does not require ordering guarantees (like Retweets, likes, or non-threaded comments).



## Supported APIs
Various APIs allow your applications to treat Cosmos DB like it was another database technology without management overhead.
- API for NoSQL
  - Features are available in this API first
  - Data is stored in document format
  - Provides query support through SQL
- MongoDB
  - Stores data in a document structure via BSON
  - Recommended if you want to use the MongoDB ecosystem
- Cassandra API
  - Stores data in a column-oriented schema
  - Apache Cassandra client drivers can use this API to interact with Cosmos
- PostgreSQL API
  - Managed service running PostgreSQL at any scale using Citus open source for distributed tables
  - Stores data either ona  single node or distributed in a multi-node configuration
- Table API
  - Stores data in a key/value format
  - Has limitations in latency, scaling, throughput, global distribution, index management, and low query performance
  - Only supports OLTP scenarios
- Gremlin API
  - Stores data as edges and vertices
  - Graph queries
  - Used with data too complex for relational databases
  - Only supports OLTP scenarios
- Table API
  - Stores values in key/value pairs
  - If you're already using Azure Table storage you may have limitations in latency, scaling, throughput, global distribution, index management, and low query performance which are mitigated by this API.
  - Only supports OLTP scenarios


## Request Units
- The cost of database operations is expressed by request units (RUs)
- Represents system resources (SPU, IOPS, memory)
- The cost to do a point read of a single 1 KB item is 1 RU
- RUs are charged in three different ways depending on the type of Cosmos account you've created
  - Provisioned throughput mode - You select how many RUs you want in increments of 100. Changes can be made programmatically or through the Azure portal. Throughput is provisioned at container and database granularity level.
  - Serverless mode - You get billed for however many RUs you consumed at the end of your billing period
  - Autoscale mode - Allows RU scaling based on usage

## Partitioning

### Logical Partitions
- Items in a container are divided into distinct subsets called logical partitions
- Logical partitions are formed based on the value of a partition key associated with each item in a container
- All items in a logical partition have the same partition key value
- Each item has an item ID unique within a logical partition
  - Partition key + item ID = unique identifier
- Items can be updated within a logical partition using database transactions

### Physical partitions
- Containers scale by distributing data and throughput across physical partitions
- One or more logical partitions are mapped to a single physical partition
- Physical partitions are an internal implementation of the system and are managed entirely by Cosmos
- Number depends on the following
  - Amount of throughput provisioned (10k RU/s limit for physical partitions)
  - Total data storage (each physical partition can hold 50 GB of data)

### Partition Keys
Partition keys should satisfy the following conditions
- Be a property which does not change. You can't update property values which are partition keys.
- Have a high cardinality.
- Spread RU consumption and data storage evenly across all logical partitions.
- Item IDs are generally a good choice for partition keys.
- Synthetic partition keys can be created programmatically by the client if your data doesn't have high cardinality.

## Microsoft SDKs (come back to this)
https://docs.microsoft.com/en-us/learn/modules/work-with-cosmos-db/2-cosmos-db-dotnet-overview

I had a lot of issues reading items from Cosmos so I need to come back to this later.

# Useful Commands
Create a new Cosmos DB
``` shell
az cosmosdb create --name accountName --resource-group resourceGroup
```

Retrieve the primary key for the account
``` shell
az cosmosdb keys list --name accountName --resource-group resourceGroup
```