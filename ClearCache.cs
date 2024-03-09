using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Net;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;

namespace ClearCache.Function
{
    public class ClearCache
    {
        private readonly ILogger<ClearCache> _logger;

        public ClearCache(ILogger<ClearCache> logger)
        {
            _logger = logger;
        }

        [Function("clear-cache")]
        public async Task<HttpResponseData> RunAsync([HttpTrigger(AuthorizationLevel.Anonymous, "post")] HttpRequestData req,
                                                     FunctionContext executionContext)
        {
            var logger = executionContext.GetLogger("clear-cache");
            logger.LogInformation("C# HTTP trigger function processed a request.");

            var response = req.CreateResponse(HttpStatusCode.OK);

            // Clear data from Redis Cache
            var success = await RedisCacheHelper.ClearDataFromCacheAsync("jobs");
            if (success)
            {
                logger.LogInformation("Data cleared from Redis Cache");
                await response.WriteStringAsync("Data cleared from Redis Cache");
            }
            else
            {
                logger.LogError("Failed to clear data from Redis Cache");
                await response.WriteStringAsync("Failed to clear data from Redis Cache");
            }

            return response;
        }
    }
}
