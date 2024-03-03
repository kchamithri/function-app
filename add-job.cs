using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Extensions.Logging;
using System.Net;
using System.IO;
using System.Text.Json;

namespace addJob.Function
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

    public class add_job
    {
        private readonly ILogger<add_job> _logger;

        public add_job(ILogger<add_job> logger)
        {
            _logger = logger;
        }

        public class MultiResponse
        {
            [CosmosDBOutput("job-posting-app-db", "Job",
                Connection = "CosmosDbConnectionString", CreateIfNotExists = true)]
            public Job Job { get; set; }
            public HttpResponseData HttpResponse { get; set; }
        }

        [Function("add_job")]
        public static async Task<MultiResponse> Run([HttpTrigger(AuthorizationLevel.Anonymous, "post")] HttpRequestData req,
            FunctionContext executionContext)
        {
            var logger = executionContext.GetLogger("get-jobs");
            logger.LogInformation("C# HTTP trigger function processed a request.");

            // Get the request body
            string requestBody = await new StreamReader(req.Body).ReadToEndAsync();

            // Deserialize the JSON object
            Job job = JsonSerializer.Deserialize<Job>(requestBody);

            // Log the received job details
            logger.LogInformation("Received Job Details: {JobDetails}", JsonSerializer.Serialize(job));

            var response = req.CreateResponse(System.Net.HttpStatusCode.OK);

            // Return a response to both HTTP trigger and Azure Cosmos DB output binding.
            return new MultiResponse()
            {
                Job = job,
                HttpResponse = response
            };
        }
    }
}