using System.Text.Json;
using StackExchange.Redis;
using System;
using System.Threading.Tasks;
using System.Collections.Generic;
using addJob.Function;

public class RedisCacheHelper
{
    private static ConnectionMultiplexer _connection;

    //     public class Job
    // {
    //     public string id { get; set; } = Guid.NewGuid().ToString();
    //     public string companyName { get; set; }
    //     public string companyUrl { get; set; }
    //     public string link { get; set; }
    //     public string location { get; set; }
    //     public DateTime postedOn { get; set; }
    //     public List<string> skills { get; set; }
    //     public string title { get; set; }
    //     public string type { get; set; }
    //     public string description { get; set; }
    // }

    public static ConnectionMultiplexer Connection
    {
        get
        {
            if (_connection == null || !_connection.IsConnected)
            {
                var connectionString = "job-posting-app.redis.cache.windows.net:6380,password=0tfLwwB1YAf0JcsGtTDnimdlD0HBAEspSAzCaKulFME=,ssl=True,abortConnect=False";
                _connection = ConnectionMultiplexer.Connect(connectionString);
            }
            return _connection;
        }
    }

    public static async Task<string> GetDataFromCacheAsync(string key)
    {
        try
        {
            var db = Connection.GetDatabase();
            return await db.StringGetAsync(key);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error getting data from Redis Cache: {ex.Message}");
            return null;
        }
    }

    public static async Task<bool> StoreDataInCacheAsync(string key, string value)
    {
        try
        {
            var db = Connection.GetDatabase();
            return await db.StringSetAsync(key, value);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error storing data in Redis Cache: {ex.Message}");
            return false;
        }
    }

    public static async Task SaveJobToCacheAsync(Job job)
    {
        try
        {
            // Serialize the job object to JSON
            var jobJson = JsonSerializer.Serialize(job);
            Console.WriteLine($"Save the job data to the cache");
            // Save the job data to the cache
            await StoreDataInCacheAsync($"job_{job.id}", jobJson);
        }
        catch (Exception ex)
        {
            // Log any errors encountered while saving data to the cache
            Console.WriteLine($"Error saving job data to cache: {ex.Message}");
        }
    }

    public static async Task<bool> ClearDataFromCacheAsync(string key)
    {
        try
        {
            var db = Connection.GetDatabase();
            return await db.KeyDeleteAsync(key);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error clearing data from Redis Cache: {ex.Message}");
            return false;
        }
    }
}
