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


    public class HelloAzuriteTableStorage
    {

        string AccountName = Environment.GetEnvironmentVariable("AccountName");
        string TableItem = Environment.GetEnvironmentVariable("TableItem");
        string Uri = Environment.GetEnvironmentVariable("Uri");
        string AccountKey = Environment.GetEnvironmentVariable("AccountKey");

        private readonly TableClient _itemTableClient;
        public HelloAzuriteTableStorage() 
        {
            this._itemTableClient = 
            new TableClient(new Uri(Uri), 
                TableItem, 
                new TableSharedKeyCredential(AccountName, AccountKey));
        }

        [FunctionName("PostItem")]
        public async Task<IActionResult> RunPostItem(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", "post", Route = null)] HttpRequest req,
            ILogger log)
        {
            try
            {

                string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
                log.LogInformation("Request body is {requestBody}", requestBody);
                /*
                expect this (within postman):
                {
                    "PartitionKey": "3",
                    "RowKey": "3",
                    "Name": "HB Pencil",
                    "Description": "It's a pencil",
                    "Price": 2,
                    "Quantity": 31,
                    "isFavorite": false
                }
                */

                WarehouseItems warehouseItems = JsonConvert.DeserializeObject<WarehouseItems>(requestBody);

                await _itemTableClient.AddEntityAsync(warehouseItems);

                return new OkObjectResult(requestBody);

            }
            catch (Exception e)
            {
                log.LogError(e, "Error with creating item. Check that it does not exist already.");
            }

            return new BadRequestObjectResult("There was an issue");
        }
    
        [FunctionName("GetItems")]
        public async Task<IActionResult> RunGetItems(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = null)] HttpRequest req,
            ILogger log)
        {
            try
            {
                Pageable<TableEntity> queryResultsFilter = _itemTableClient.Query<TableEntity>();

                Console.WriteLine($"The query returned {queryResultsFilter.Count()} entities.");
                
                return new OkObjectResult(queryResultsFilter);

            }
            catch (Exception e)
            {
                log.LogError(e, "Problem loading");
                return new BadRequestObjectResult("There was an issue");
            }
        }

                [FunctionName("GetItem1")]
        public async Task<IActionResult> RunGetItem1(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = null)] HttpRequest req,
            ILogger log)
        {
            try
            {
                string partitionKey = req.Query["PartitionKey"];
                Pageable<TableEntity> queryResultsFilter = _itemTableClient.Query<TableEntity>(filter: $"PartitionKey eq {partitionKey}");
                return new OkObjectResult(queryResultsFilter);
            }
            catch (Exception e)
            {
                log.LogError(e, "Problem loading");
                return new BadRequestObjectResult("There was an issue");
            }
        }

        [FunctionName("PostItems")]
        public async Task<IActionResult> RunPostItems(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", "post", Route = null)] HttpRequest req,
            ILogger log)
        {
            try
            {

                string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
                log.LogInformation("Request body is {requestBody}", requestBody);
                WarehouseItems warehouseItems = JsonConvert.DeserializeObject<WarehouseItems>(requestBody);

                await _itemTableClient.AddEntityAsync(warehouseItems);

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