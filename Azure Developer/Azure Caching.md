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

## Controlling Cache Behavior
https://docs.microsoft.com/en-us/learn/modules/develop-for-storage-cdns/3-control-cache-behavior

Caching can be controlled using the following options
- Ignore query strings - The first request and any query strings are sent to the origin server while subsequent requests ignore the query strings until the TTL expires
- Bypass caching for query strings - Query requests are sent directly to the origin server
- Cache every unique URL - Unique URLs are passed to the origin server and then cached

Content can be purged from the cache using the Azure CLI.

Content can be allowed or blocked in specific countries.

## Interacting with the CDN using C# (TODO)