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

namespace TableStorage
{


    public class HelloAzuriteTableStorage
    {
        private readonly TableClient _tableClient;
        public HelloAzuriteTableStorage(TableClient tableClient) 
        {
            this._tableClient = tableClient;
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

                await _tableClient.AddEntityAsync(warehouseItems);

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

                Pageable<TableEntity> queryResultsFilter = _tableClient.Query<TableEntity>();

                Console.WriteLine($"The query returned {queryResultsFilter.Count()} entities.");
                
                return new OkObjectResult(queryResultsFilter);

            }
            catch (Exception e)
            {
                log.LogError(e, "Problem loading");
                return new BadRequestObjectResult("There was an issue");
            }
        }
    }
}