using Microsoft.Extensions.Configuration;
using StackExchange.Redis;

namespace SportsStatsTracker
{
    class Program
    {
        static async Task Main(string[] args)
        {
            var config = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json")
                .Build();

            string connectionString = config["CacheConnection"];

            using (var cache = ConnectionMultiplexer.Connect(connectionString))
            {
                IDatabase db = cache.GetDatabase();
                bool setValue = await db.StringSetAsync("test:key", "some value");
                Console.WriteLine($"SET: {setValue}");

                string? getValue = await db.StringGetAsync("test:key");
                Console.WriteLine($"GET: {getValue}");

                long newValue = await db.StringIncrementAsync("counter", 50);
                Console.WriteLine($"INCR new value = {newValue}");

                var result = await db.ExecuteAsync("ping");
                Console.WriteLine($"PING = {result.Type} : {result}");

                result = await db.ExecuteAsync("flushdb");
                Console.WriteLine($"FLUSHDB = {result.Type} : {result}");


                var stat = new GameStat("Soccer", new DateTime(1950, 7, 16), "FIFA World Cup", 
                new[] { "Uruguay", "Brazil" },
                new[] { ("Uruguay", 2), ("Brazil", 1) });

                string serializedValue = Newtonsoft.Json.JsonConvert.SerializeObject(stat);
                bool added = db.StringSet("event:1950-world-cup", serializedValue);
                Console.WriteLine(serializedValue); // displays "serializedValue"

                var result1 = db.StringGet("event:1950-world-cup");
                var stat1 = Newtonsoft.Json.JsonConvert.DeserializeObject<GameStat>(result1.ToString());
                Console.WriteLine(stat1.Sport); // displays "Soccer"
            }
        }
    }
}