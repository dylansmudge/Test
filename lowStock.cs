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


    public class LowStock
    {
        string AccountName = Environment.GetEnvironmentVariable("AccountName");
        string TableFavorite = Environment.GetEnvironmentVariable("TableFavorites");

        string TableItem = Environment.GetEnvironmentVariable("TableItem");
        string Uri = Environment.GetEnvironmentVariable("Uri");
        string AccountKey = Environment.GetEnvironmentVariable("AccountKey");
        private readonly TableClient _favoriteTableClient;
        private readonly TableClient _itemTableClient;

        public LowStock() 
        {
            this._favoriteTableClient = 
            new TableClient(new Uri(Uri), 
                TableFavorite, 
                new TableSharedKeyCredential(AccountName, AccountKey));

            this._itemTableClient = 
            new TableClient(new Uri(Uri), 
                TableItem, 
                new TableSharedKeyCredential(AccountName, AccountKey));
        }

            [FunctionName("GetLowStock")]
        public async Task<IActionResult> GetLowStock(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = null)] HttpRequest req,
            ILogger log)
        {
            try
            {
                string itemId = req.Query["ItemId"];

                Console.WriteLine($"ItemId {itemId}");


                Pageable<TableEntity> favoritesQuery = _favoriteTableClient.Query<TableEntity>(filter: $"ItemId eq {itemId}");
                int favoritesCount = favoritesQuery.Count();
                Console.WriteLine($"Favorited {favoritesCount} entities.");

                WarehouseItems itemsQuery = await _itemTableClient.GetEntityAsync<WarehouseItems>(
                partitionKey : $"{itemId}",
                rowKey : $"{itemId}");

                int itemsCount = itemsQuery.Quantity;
                Console.WriteLine($"Quantity is {itemsCount}");
                

                if (itemsCount < favoritesCount)
                {
                    return new OkObjectResult($"Low stock");
                }
                else
                    return new OkObjectResult($"There are {itemsCount - favoritesCount} more items yet to be wishlisted");
            }
            catch (Exception e)
            {
                log.LogError(e, "Problem loading");
                return new BadRequestObjectResult("There was an issue");
            }
        }
    }


}