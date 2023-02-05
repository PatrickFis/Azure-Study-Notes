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
    - Reads performed within a region that accepts writes offers consistency identical to strong consistency (ie, read your own write within the write region)
    - As the staleness window approaches the service will throttle new writes to allow replication to catch up and honor the consistency guarantee.
  - Session
    - Recommended for many real world scenarios
    - Provides write latencies, availability, and read throughput similar to eventual consistency while also offering consistency guarantees that suit the needs of applications that operate in the context of a user.
    - Read your own write (within write region like bounded staleness)
  - Consistent prefix
    - Updates made as single document writes see eventual consistency.
    - Updates made as a batch within a transaction are returned consistent to the transaction in which they are committed.
    - Write operations within a transaction of multiple documents are always visible together.
  - Eventual
    - No ordering guarantee for reads. Without further writes the replicas eventually converge.
    - Ideal when an application does not require ordering guarantees (like Retweets, likes, or non-threaded comments).



## Supported APIs
Various APIs allow your applications to treat Cosmos DB like it was another database technology without management overhead.
- API for NoSQL (aka Core SQL)
  - Features are available in this API first
  - Data is stored in document format
  - Provides query support through SQL
  - Recommended to be used if you want the following
    - New features come here first
    - Support for SQL
    - You're migrating from another database like Oracle, DynamoDB, HBase, DB2, etc.
- MongoDB
  - Stores data in a document structure via BSON
  - Compatible with MongoDB wire protocol without using any native MongoDB related code
  - Existing MongoDB apps can be used by changing the connection string they use and migrating your data
  - Recommended if you want to use the MongoDB ecosystem
- Cassandra API
  - Stores data in a column-oriented schema
  - Useful for storing large volumes of data
  - Apache Cassandra client drivers can use this API to interact with Cosmos
  - Only supports OLTP scenarios
  - Recommended for the following:
    - You're already familiar with Cassandra
    - You want to use Cassandra Query Language (CQL) and tools like CQL Shell
    - You want to use features from Cosmos DB like change feeds
- PostgreSQL API
  - Managed service running PostgreSQL at any scale using Citus open source for distributed tables
  - Stores data either on a single node or distributed in a multi-node configuration
  - Recommended if you want to use a managed open source relation database
- Table API
  - Stores data in a key/value format
  - If you're already using Azure Table storage you may have limitations in latency, scaling, throughput, global distribution, index management, and low query performance which are mitigated by this API.
  - Only supports OLTP scenarios
- Gremlin API
  - Stores data as edges and vertices
  - Graph queries
  - Used with data too complex for relational databases
  - Recommended for the following:
    - You have dynamic data
    - Your data has complex relations
    - Your data is too complex to be modeled with relational databases
    - You want to use the existing Gremlin ecosystem and skills
  - Only supports OLTP scenarios


## Request Units
- The cost of database operations is expressed by request units (RUs)
- Represents system resources (CPU, IOPS, memory)
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


# Udemy Notes
- Cosmos DB is a fully managed NoSQL database.
- Fast response times.
- Scales based on demand.
- Various APIs available to interact with Cosmos.

## Partitions
- Items in a container are divided into subsets called logical partitions
- Partitions are decided based on their partition key
- Each item has an item ID to uniquely identify an item in the partition (note that partition key + item ID uniquely identifies an item in the container)

## Setting up Cosmos
- Creating a new Cosmos DB resource is fairly simple, just remember to select the free tier so that you aren't charged for it.
- After the resource is created you'll need to make a new database.
  - Give the database a name
  - Create a container and give it a name
  - Choose a partition key for the container
  - Add an item to the container and note that some auto generated values are attached to the item

## Working with Cosmos
- SQL queries can be used with Cosmos, though their syntax is slightly different than what I use at work (things like using " instead of ' or being forced to alias a table to reference a column in the table). The columns also seem to be case sensitive when referenced.
- SQL can reference arrays inside of items using the following syntax (see Customers.json for setting up the items for this query): `SELECT o.quantity FROM o in Customers.orders`
- Assignment 3 from Udemy was to create query to display the total quantity from a customer. I solved this using this query (see the JOINs section on https://devblogs.microsoft.com/cosmosdb/understanding-how-to-query-arrays-in-azure-cosmos-db/ for information on how joins work in Cosmos since it's different from other DBs):
```sql
select c.customerId, sum(orders.quantity) as totalQuantity from Customers c
join orders in c.orders
group by c.customerId
```

## .NET and Cosmos
- Install the Microsoft.Azure.Cosmos dependency in your project for this
- IDs associated with items must be specified in an id field in the object or Cosmos will send you a bad request response (this is different from the UI which would generate it for you when you add an item).

## Stored Procs
- Stored procs in Cosmos are written in JS and called programmatically through a Container's Scripts field. The stored procs made as part of this course are these two (the first is a demo while the second creates orders in the orders container):
```js
function Display() 
{
    var context = getContext();
    var response = context.getResponse();
    
    response.setBody("This is a stored procedure");    
}
```

```js
function createItems(items)
{
    var context = getContext();
    var response = context.getResponse();
    
    if(!items) {
        response.setBody("Error: Items are undefined");
        return;
    }
    
    var numOfItems = items.length;
    checkLength(numOfItems);
    
    for(let i=0; i<numOfItems; i++) {
        createItem(items[i]);
    }
    
    function checkLength(itemLength) {
        if(itemLength == 0) {
            response.setBody("Error: Items are undefined");
            return;
        }
    }
    
    function createItem(item) {
        var collection = context.getCollection();
        var collectionLink = collection.getSelfLink();
        collection.createDocument(collectionLink, item);
    }
}
```

## Triggers
- Triggers are also written in JS like stored procedures
- Triggers aren't automatically executed and must be specified for each database operation where you want them to be executed
- The trigger written for this course is this one (it just sets the quantity on an order item):
```js
function validateItem() {
    var context = getContext();
    var request = context.getRequest();
    var item = request.getBody();
    
    if(!("quantity" in item)) {
        item["quantity"] = 0
    }
    
    request.setBody(item);
}
```

## Composite Indexes
- When you want to order a query by multiple columns you need a composite index. In a container it's defined in Scale & Settings -> Indexing Policy and looks like the compositeIndexes section:
```json
{
    "indexingMode": "consistent",
    "automatic": true,
    "includedPaths": [
        {
            "path": "/*"
        }
    ],
    "excludedPaths": [
        {
            "path": "/\"_etag\"/?"
        }
    ],
    "compositeIndexes": [
        [
            {
                "path" : "/category",
                "order" : "ascending"
            },
            {
                "path": "/quantity",
                "order": "ascending"
            }
        ]
    ]
}
```

## Misc.
- JSON for setting up some stuff in Cosmos is located in [Misc Files/Cosmos.json](Misc%20Files/Cosmos.json) and [Misc Files/Customers.json](Misc%20Files/Customers.json).
- Code for this is stored in [Code/Visual Studio Projects/UdemyCosmosApp](Code/Visual%20Studio%20Projects/UdemyCosmosApp/).

- See https://learn.microsoft.com/en-us/shows/exam-readiness-zone/preparing-for-az-204-develop-for-azure-storage-segment-2-of-5 for information on what may appear on the exam.
  - Be familiar with where Cosmos DB fits in to the larger Azure ecosystem
  - Be familiar with multi-region replication
    - See https://learn.microsoft.com/en-us/azure/cosmos-db/distribute-data-globally
    - Cosmos transparently replicates data to all the regions associated with your Azure Cosmos DB account.
  - Be familiar with consistency levels
    - Understand consistency patterns
    - Understand what you get from each consistency level and when you should use a particular consistency level
  - Understand the supported APIs
    - Mongo, Cassandra, and the Table API are really more for upgrading
    - Primarily focus on the Core(SQL) API when looking at functionality
  - Understand how Cosmos scales (sharding AKA partitioning)
    - Understand logical vs physical partitions
    - The exam may focus on how you establish the partition key
      - You should understand when to use an item as a partition key (so if you have a property with a wide range of values use that)
  - Understand the resource hierarchy
    - The Cosmos account
    - Databases within the account
    - Containers within databases
      - Containers are the focus of partitioning and throughput
    - Items inside the containers

## Cosmos SDK Notes [API Docs](https://learn.microsoft.com/en-us/dotnet/api/microsoft.azure.cosmos?view=azure-dotnet)
- CosmosClient: Provides a client-side logical representation of the Azure Cosmos DB account.
  - Constructors
    - CosmosClient(string accountEndpoint, string authKeyOrResourceToken, CosmosClientOptions clientOptions = default)
      - The accountEndpoint is the Cosmos service endpoint
      - The authKeyOrResourceToken is an account key or resource token
      - The clientOptions is an optional resource
      - Other variations of this endpoint use an AzureKeyCredential instead of the authKeyOrResourceToken parameter
  - Methods
    - CreateDatabaseAsync(string id, int? throughput = default, RequestOptions requestOptions = default, CancellationToken cancellationToken = default)
      - id = Database ID
      - throughput is an optional parameter to specify the number of RU/s for the DB
        - An alternate version of this method allows for this to be specified through a ThroughputProperties class which allows for additional configuration
      - requestOptions is an optional parameter for setting headers, ETags, and other properties
      - cancellationToken is used to propagate notifications that the operation should be cancelled
      - Returns a DatabaseResponse which contains information about the created resource
    - CreateDatabaseIfNotExistsAsync (same as CreateDatabaseAsync)
      - This method has the same parameters as CreateDatabaseAsync
      - This method verifies that a DB with the specified ID does not exist before creating the DB
      - 200 response = DB already exists, 201 = created
    - GetDatabase(string id)
      - Retrieves a Database object using the DB's ID
    - GetContainer(string databaseId, string containerId)
      - Retrieves a Container object using the DB's ID and the container's ID
- Database: Allows for reading or deleting an existing database
  - Constructors
    - Generally this is going to be returned from a CosmosClient
  - Methods
    - ReadAsync(RequestOptions requestOptions = default, CancellationToken cancellationToken = default)
      - Retrieves a DatabaseResponse containing DatabaseProperties with information about the DBs in Cosmos
    - CreateContainerAsync(string id, string partitionKeyPath, int? throughput = default, RequestOptions requestOptions = default, CancellationToken cancellationToken = default)
      - Creates a container with the specified ID and partition key
    - CreateContainerIfNotExistsAsync (same as CreateContainerAsync)
    - GetContainer(string id)
      - Retrieves a Container object using the container's ID
- Container: Provides a way to read, replace, or delete a specific container or item in a container
  - Constructors
    - Generally this is going to be returned from a CosmosClient or Database
  - Methods
    - CreateItemAsync`<T>`(T item, PartitionKey? partitionKey = default, ItemRequestOptions requestOptions = default, CancellationToken cancellationToken = default)
      - Returns an ItemResponse`<T>` after creating an item in a container
      - The partionKey is nullable. If null the key will be extracted from the item parameter.
    - ReadItemAsync`<T>`(string id, PartitionKey partitionKey, ItemRequestOptions requestOptions = default, CancellationToken cancellationToken = default)
       - Returns an ItemResponse`<T>` containing an item from the container with the specified ID and partition key
    - GetItemQueryIterator`<T>`(QueryDefinition queryDefinition, string continuationToken = default, QueryRequestOptions requestOptions = default)
      - Retrieves a FeedIterator`<T>` with items found from a SQL statement using prepared values. QueryDefinitions are defined like this: `QueryDefinition query = new QueryDefinition("select * from sales s where s.AccountNumber = @AccountInput ").WithParameter("@AccountInput", "Account1");`

## Change Feeds [MS Documentation](https://learn.microsoft.com/en-us/azure/cosmos-db/change-feed)
- Changes feeds are a persistent record of changes to a container in the order they occur.
- Change feeds work by listening to a container for changes and outputting a sorted list of documents that were changed in the order they were modified.
- Changes can be processed async
- Supports one or more consumers for parallel processing
- Per docs it's supported by all APIs except Postgres and Table
- Change feeds will contain inserts and updates but not deletes

### Change Feed Features
- Enabled by default
- Your provisioned throughput can be used to read from the change feed
- Inserts and updates are captured
- Deletes can be captured by setting a soft-delete flag within items
- Only the most recent change for a given item is included in the change log
- Changes appear in the feed exactly once, so checkpointing logic must be implemented by the client (or by using a change feed processor)
- Can be processed by Azure Functions
- Changes are written to another container