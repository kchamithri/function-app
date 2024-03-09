using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Extensions.Logging;
using System.Net;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;


namespace jobs.Function
{
    public class Job
    {
        public string id { get; set; } = Guid.NewGuid().ToString();
        public string companyName { get; set; }
        public string companyUrl { get; set; }
        public string link { get; set; }
        public string location { get; set; }
        public DateTime postedOn { get; set; }
        public List<string> skills { get; set; }
        public string title { get; set; }
        public string type { get; set; }
        public string description { get; set; }
    }

    public class get_jobs
    {
        private readonly ILogger<get_jobs> _logger;

        public get_jobs(ILogger<get_jobs> logger)
        {
            _logger = logger;
        }

        [Function("get-jobs")]
        public async Task<HttpResponseData> RunAsync([HttpTrigger(AuthorizationLevel.Anonymous, "get", "post")] HttpRequestData req,
                                                      [CosmosDBInput("job-posting-app-db", "Job",
                                                      Connection = "CosmosDbConnectionString",
                                                      SqlQuery = "SELECT * FROM c")] IEnumerable<Job> jobs,
                                                      FunctionContext executionContext)
        {
            var logger = executionContext.GetLogger("get-jobs");
            logger.LogInformation("C# HTTP trigger function processed a request.");

            var response = req.CreateResponse(HttpStatusCode.OK);

            // Check if job data is available in Redis Cache
            var cachedJobsJson = await RedisCacheHelper.GetDataFromCacheAsync("jobs");
            if (!string.IsNullOrEmpty(cachedJobsJson))
            {
                // If data is found in cache, return it
                _logger.LogInformation("Data retrieved from Redis Cache");
                await response.WriteStringAsync(cachedJobsJson);
                return response;
            }

            // If data is not found in cache, fetch it from Cosmos DB
            var outputJobs = jobs.ToList();
            _logger.LogInformation("Data fetched from Cosmos DB");

            // Serialize the output jobs to JSON
            var json = JsonSerializer.Serialize(outputJobs);

            // Store data in Redis Cache
            await RedisCacheHelper.StoreDataInCacheAsync("jobs", json);

            // Write the JSON content to the response body
            await response.WriteStringAsync(json);

            return response;
        }
    }
}
