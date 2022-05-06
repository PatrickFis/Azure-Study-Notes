using StackExchange.Redis;
using System.Threading.Tasks;

namespace MyApp // Note: actual namespace depends on the project name.
{
    internal class Program
    {
        // connection string to your Redis Cache    
        static string connectionString = "primary connection string";
        static async Task Main(string[] args)
        {
            using (var cache = ConnectionMultiplexer.Connect(connectionString))
            {
                IDatabase db = cache.GetDatabase();

                // Ping the server
                var result = await db.ExecuteAsync("ping");
                Console.WriteLine($"Ping = {result.Type} : {result}");

                // Put a value into the cache
                bool setValue = await db.StringSetAsync("test:key2", "1020");
                Console.WriteLine($"Set: {setValue}");

                // Put a few more values in the cache
                for (int i = 0; i < 10; i++)
                {
                    await db.StringSetAsync($"{i}", i);
                    // Checking out what KeyExpire does
                    db.KeyExpire($"{i}", TimeSpan.FromMilliseconds(1000));
                }

                // Retrieve a value from the cache
                string value = await db.StringGetAsync("test:key2");
                Console.WriteLine($"Got value: {value}");

                // Retrieve a few more values
                for (int i = 0; i < 10; i++)
                {
                    value = await db.StringGetAsync($"{i}");
                    Console.WriteLine($"Value: {value}");
                }
            }
        }
    }
}