using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;

namespace jobs.Function
{
    public class get_jobs
    {
        private readonly ILogger<get_jobs> _logger;

        public get_jobs(ILogger<get_jobs> logger)
        {
            _logger = logger;
        }

        [Function("get_jobs")]
        public IActionResult Run([HttpTrigger(AuthorizationLevel.Anonymous, "get", "post")] HttpRequest req)
        {
            _logger.LogInformation("C# HTTP trigger function processed a request.");
            return new OkObjectResult("Welcome to Azure Functions!");
        }
    }
}
