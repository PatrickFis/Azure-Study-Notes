# Azure Cache for Redis
- In-memory data store based on Redis
- Keeps frequently accessed data in the server's memory
- Offered as open-source Redis or as a commercial product from Redis Labs as a managed service

Scenarios

| Pattern                  | Description                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                      |
| ------------------------ | -------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------- |
| Data cache               | Databases are often too large to load directly into a cache. It's common to use the cache-aside pattern to load data into the cache only as needed. When the system makes changes to the data, the system can also update the cache, which is then distributed to other clients.                                                                                                                                                                                                                                                 |
| Content cache            | Many web pages are generated from templates that use static content such as headers, footers, banners. These static items shouldn't change often. Using an in-memory cache provides quick access to static content compared to backend datastores.                                                                                                                                                                                                                                                                               |
| Session store            | This pattern is commonly used with shopping carts and other user history data that a web application might associate with user cookies. Storing too much in a cookie can have a negative effect on performance as the cookie size grows and is passed and validated with every request. A typical solution uses the cookie as a key to query the data in a database. Using an in-memory cache, like Azure Cache for Redis, to associate information with a user is much faster than interacting with a full relational database. |
| Job and message queuing  | Applications often add tasks to a queue when the operations associated with the request take time to execute. Longer running operations are queued to be processed in sequence, often by another server. This method of deferring work is called task queuing.                                                                                                                                                                                                                                                                   |
| Distributed transactions | Applications sometimes require a series of commands against a backend data-store to execute as a single atomic operation. All commands must succeed, or all must be rolled back to the initial state. Azure Cache for Redis supports executing a batch of commands as a single transaction.                                                                                                                                                                                                                                      |

Service Tiers

| Tier             | Description                                                                                                                                                                                                                       |
| ---------------- | --------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------- |
| Basic            | An OSS Redis cache running on a single VM. This tier has no service-level agreement (SLA) and is ideal for development/test and non-critical workloads.                                                                           |
| Standard         | An OSS Redis cache running on two VMs in a replicated configuration.                                                                                                                                                              |
| Premium          | High-performance OSS Redis caches. This tier offers higher throughput, lower latency, better availability, and more features. Premium caches are deployed on more powerful VMs compared to the VMs for Basic or Standard caches.  |
| Enterprise       | High-performance caches powered by Redis Labs' Redis Enterprise software. This tier supports Redis modules including RediSearch, RedisBloom, and RedisTimeSeries. Also, it offers even higher availability than the Premium tier. |
| Enterprise Flash | Cost-effective large caches powered by Redis Labs' Redis Enterprise software. This tier extends Redis data storage to non-volatile memory, which is cheaper than DRAM, on a VM. It reduces the overall per-GB memory cost.        |


## Configuring Redis
- Redis and your app should be configured in the same region
- Premium tiers support extra features
  - Allow for persistence to be configured for disaster recovery
  - VNets
  - Clustering - Data is split between multiple nodes. This requires you to specify the number of shards (up to 10) with the cost incurred being the cost of the original node times the number of shards.
- Redis has a command line tool that can be used to interact with Azure Cache for Redis
- A time to live (TTL) can be applied to keys in Redis so that it expires
- Clients can access Redis using the host name, port, and an access key. Two access keys are provided so that you can regenerate the primary key and have no downtime by switching your apps to use the secondary key.

## Connecting to Redis using C#
Create a resource group and resources for Redis
``` bash
az group create --name az204-redis-patrick-rg --location eastus

redisName=az204redis$RANDOM
az redis create --location eastus \
  --resource-group az204-redis-patrick-rg \
  --name $redisName \
  --sku Basic --vm-size c0
```
The code for interacting with Redis can be found [here](Code/Azure%20Redis/). When finished the resource group can be deleted.
``` bash
az group delete --name az204-redis-patrick-rg --no-wait
```

More code for interacting with Redis can be found [here](Code/Azure%20App%20Service/).

# Azure Content Delivery Network (CDN)
- CDNs are distributed networks of servers used to efficiently deliver web content to users
- CDNs are located close to end users to minimize latency
- CDNs offer better performance when apps need to make multiple round-trips to load content, large scaling for instantaneous high loads, and less traffic is sent to the origin server since more requests can be served from edge servers

Azure CDN handles requests through the following process
1. A user requests a file (AKA an asset) through a special URL which is routed to a POP (point of presence) closest to the user
2. The POP server requests the file from the origin server if it isn't in their cache
3. The origin server returns the file to the POP server
4. An edge server in the POP caches the file and returns it to the requestor. The file is cached based on the TTL specified in HTTP headers. The default TTL is 7 days.
5. Additional users can request the same file using the same URL
6. If the TTL hasn't expired the POP edge server returns the file directly from the cache

## CDN Products
- Azure CDN Standard from Microsoft
- Azure CDN Standard from Akamai
- Azure CDN Standard from Verizon
- Azure CDN Premium from Verizon
- https://learn.microsoft.com/en-us/azure/cdn/cdn-features has the features for each product

## Controlling Cache Behavior
https://docs.microsoft.com/en-us/learn/modules/develop-for-storage-cdns/3-control-cache-behavior

Caching can be controlled using the following options
- Ignore query strings - The first request and any query strings are sent to the origin server while subsequent requests ignore the query strings until the TTL expires
- Bypass caching for query strings - Query requests are sent directly to the origin server
- Cache every unique URL - Unique URLs are passed to the origin server and then cached

Content can be purged from the cache using the Azure CLI.

Content can be allowed or blocked in specific countries.

## Interacting with the CDN using C# (TODO)


# Udemy Notes (Split from Section 11 with Azure Monitoring Apps)
## Azure Cache for Redis
- Redis is an in-memory data store
- Helps provide low latency and high throughput for your data

### Creating a Redis Cache
- Search for Azure Cache for Redis in the marketplace
- Create the resource using the default options, except change the SKU to C0

### Interacting with Redis using the console from the portal
- Redis takes fairly simple commands to interact with it:
  - The set command will store a key-value pair in memory: `set <key name> <key value>`
  - The get command will retrieve a key value from memory: `get <key name>`
  - The exists command will tell you if a key has an associated value: `exists <key name>`
  - The del command will delete a key-value pair: `del <key name>`
  - The lpush command will push values onto a linked list: `lpush <key name> <element>`
  - The lrange command will list elements in the specified list in the given index range: `lrange <key name> <start index> <stop index>`

### Redis Data Types [Redis Documentation](https://redis.io/docs/data-types/tutorial/) and [More Redis Documentation](https://redis.io/docs/data-types/)
- Strings - Sequeuences of bytes
- Lists - Lists of strings sorted by insertion order
  - Implemented as a linked list, so elements can be added to the head or tail of the list in constant time. Accessing the head or tail of the list should also be fast.
- Sets - Unordered collections of unique strings that can add, remove, and test for existence in O(1) time
- Hashes - Collections of field-value pairs (so hashmaps, dictionaries, etc.)
- Sorted sets - Collections of unique strings that maintain order by each string's associated score
- Streams - Data structure that acts like an append-only log
- Geospatial indexes - Useful for finding locations within a given geographic radius or bounding box
- Bitmaps - Lets you perform bitwise operations on strings
- Bitfields - Efficiently encodes multiple counters in a string value. Supports atomic get, set, and increment operations and different overflow policies.
- HyperLogLog - Provides probabilistic estimates of the cardinality of large sets

### Working with Redis from .NET
1. Create a new console app in VS
2. Add the StackExchange.Redis NuGet package
3. Retrieve the connection string for Redis from the "Access keys" blade in the resource in the portal
4. See the code at [Code/Visual Studio Projects/UdemyRedisConsoleApp](Code/Visual%20Studio%20Projects/UdemyRedisConsoleApp/)

### Cache Key Eviction and Invalidation
- Default policy is to remove the least recently used (LRU) key from the cache.
- Keys can be deleted with the del command or the KeyDelete method in .NET
- Keys can be expired after a given timespan with the KeyExpire method

## Azure Content Delivery Network
- Helps delivery content to users across the globe by placing content at servers placed at different locations throughout the world
- General process for the CDN:
  - A user in a location makes a request against the CDN endpoint
  - The CDN checks whether the point of presence location closest to the user has the requested file
  - If not, a request is made to get the file from the source
  - A server in the POP will then cache the file
  - The server will send the file to the user
- Azure CDN Caching
  - Caching can be controlled from the "Caching rules" blade in the CDN
  - By default data is cached for 7 days
  - Settings can be overriden to retrieve data from the origin more frequently instead of every 7 days.

## Azure Front Door [MS Link to Documentation](https://learn.microsoft.com/en-us/azure/frontdoor/front-door-overview)
- Azure Front Door is Microsoft's modern cloud CDN
- Delivers content using MS's global edge network with their points of presence

### Why use it?
- It lets you build a modern internet-first architecture using a highly automated, secure, and reliable platform
- It accelerates your ability to deliver your app and content globally and adapt to new demand and markets
- It lets you secure your application with a zero trust framework

### Key Benefits
- You can leverage MS's edge locations to improve latency for apps
- You can accelerate application performance using anycast and split TCP connections
- SSL terminations and certificate management
- Native IPv6 and HTTP/2 protocol support
- Integrates with the typical CLI tools and other devops tools
- Allows you to integrate other Azure services like DNS, Web Apps, Storage, etc.
- Built-in reports
- Real time traffic monitoring and integration with Azure Monitor
- Each request sent to Front Door can be logged
- Provides caching, SSL offloading, and layer 3-4 DDoS protection
- Free managed SSL certs
- Low entry fee and simplified cost model
- Has a Web Application Firewall (WAF)
- Enables private backend connections with Private Links