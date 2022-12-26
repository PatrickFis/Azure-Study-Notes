using Newtonsoft.Json;
using StackExchange.Redis;
using UdemyRedisConsoleApp;

string connectionString = "connection string here";

ConnectionMultiplexer redis = ConnectionMultiplexer.Connect(connectionString);

SetCacheData("u1", 10, 100);
SetCacheData("u1", 20, 200);
SetCacheData("u1", 30, 300);
SetCacheDataWithExpiration("u2", 10, 100, TimeSpan.FromSeconds(20));

GetCacheData();
GetCachedCart("u1");

void SetCacheData(string userId, int productId, int quantity)
{
    IDatabase database = redis.GetDatabase();
    CartItem cartItem = new() { ProductId = productId, Quantity = quantity };
    string key = String.Concat(userId, ":cartItems");

    database.ListRightPush(key, JsonConvert.SerializeObject(cartItem));
    Console.WriteLine("Cache data set");
}

void SetCacheDataWithExpiration(string userId, int productId, int quantity, TimeSpan expiry)
{
    IDatabase database = redis.GetDatabase();
    CartItem cartItem = new() { ProductId = productId, Quantity = quantity };
    string key = String.Concat(userId, ":cartItems");

    database.ListRightPush(key, JsonConvert.SerializeObject(cartItem));
    database.KeyExpire(key, expiry);
    Console.WriteLine("Cache data set");
}

void DemoSetData()
{
    IDatabase database = redis.GetDatabase();
    database.StringSet("top_course", "AZ-204");
    Console.WriteLine("Cache data set");
}

void GetCacheData()
{
    IDatabase database = redis.GetDatabase();
    if(database.KeyExists("top_course"))
    {
        Console.WriteLine($"top_course: {database.StringGet("top_course")}");
    }
}

void GetCachedCart(string userId)
{
    IDatabase database = redis.GetDatabase();
    string key = String.Concat(userId, ":cartItems");

    if (database.KeyExists(key))
    {
        RedisValue[] result = database.ListRange(key);
        foreach(RedisValue value in result)
        {
            Console.WriteLine(value.ToString());
        }
    }
}