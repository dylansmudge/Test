using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System.IO;
using Azure.Data.Tables;
using Azure;
using System.Web;

namespace TableStorage
{


    public class UserRequests
    {

        string AccountName = Environment.GetEnvironmentVariable("AccountName");
        string TableUser = Environment.GetEnvironmentVariable("TableUser");
        string Uri = Environment.GetEnvironmentVariable("Uri");
        string AccountKey = Environment.GetEnvironmentVariable("AccountKey");
        private readonly TableClient _userTableClient;
        public UserRequests() 
        {
            this._userTableClient = 
            new TableClient(new Uri(Uri), 
                TableUser, 
                new TableSharedKeyCredential(AccountName, AccountKey));
        }
    
        [FunctionName("GetUsers")]
        public async Task<IActionResult> RunGetUsers(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = null)] HttpRequest req,
            ILogger log)
        {
            try
            {

                Pageable<TableEntity> queryResultsFilter = _userTableClient.Query<TableEntity>();

                Console.WriteLine($"The query returned {queryResultsFilter.Count()} entities.");
                
                return new OkObjectResult(queryResultsFilter);

            }
            catch (Exception e)
            {
                log.LogError(e, "Problem loading");
                return new BadRequestObjectResult("There was an issue");
            }
        }

        [FunctionName("GetUser1")]
        public async Task<IActionResult> RunGetUser1(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = null)] HttpRequest req,
            ILogger log)
        {
            try
            {
                string query = req.Query["PartitionKey"];
                Console.WriteLine($"query params string is {query}");

                Pageable<TableEntity> queryResultsFilter = _userTableClient.Query<TableEntity>(filter: $"PartitionKey eq '{query}'");

                Console.WriteLine($"The query returned {queryResultsFilter.Count()} entities.");
                
                return new OkObjectResult(queryResultsFilter);

            }
            catch (Exception e)
            {
                log.LogError(e, "Problem loading");
                return new BadRequestObjectResult("There was an issue");
            }
        }

        [FunctionName("PostUser")]
        public async Task<IActionResult> RunPostUser(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", "post", Route = null)] HttpRequest req,
            ILogger log)
        {
            try
            {

                string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
                log.LogInformation("Request body is {requestBody}", requestBody);
                Users users = JsonConvert.DeserializeObject<Users>(requestBody);

                await _userTableClient.AddEntityAsync(users);

                return new OkObjectResult(requestBody);

            }
            catch (Exception e)
            {
                log.LogError(e, "Error with creating item. Check that it does not exist already.");
            }

            return new BadRequestObjectResult("There was an issue");
        }
    }
}