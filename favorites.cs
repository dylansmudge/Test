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
    public class Favorites
    {
        private readonly TableClient _favoriteTableClient;

        public Favorites(TableClient favoriteTableClient)
        {
            this._favoriteTableClient = favoriteTableClient;
        }

        [FunctionName("GetFavorites")]
        public async Task<IActionResult> RunGetFavorites(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = null)] HttpRequest req,
            ILogger log)
        {
            try
            {
                Pageable<TableEntity> queryResultsFilter = _favoriteTableClient.Query<TableEntity>();

                Console.WriteLine($"The query returned {queryResultsFilter.Count()} entities.");
                
                return new OkObjectResult(queryResultsFilter);

            }
            catch (Exception e)
            {
                log.LogError(e, "Problem loading");
                return new BadRequestObjectResult("There was an issue");
            }
        }

        [FunctionName("MakeFavorite")]
        public async Task<IActionResult> RunMakeFavorite(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", "post", Route = null)] HttpRequest req,
            ILogger log)
        {
            try
            {
                string UserId = req.Query["UserId"];
                string ItemId = req.Query["ItemId"];
                Console.WriteLine($"query params string is UserId {UserId} ItemId {ItemId}");
                Pageable<TableEntity> favoriteFilter = _favoriteTableClient.Query<TableEntity>(filter: $"UserId eq '{UserId}' and ItemId eq '{ItemId}'");
                string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
                log.LogInformation("Request body is {requestBody}", requestBody);
                FavoriteItems favorites = JsonConvert.DeserializeObject<FavoriteItems>(requestBody);
                log.LogInformation("isFavorite is {requestBody}");

                await _favoriteTableClient.UpsertEntityAsync(favorites);

                return new OkObjectResult(favoriteFilter);

            }
            catch (Exception e)
            {
                log.LogError(e, "Error with creating item. Check that it does not exist already.");
            }

            return new BadRequestObjectResult("There was an issue");
        }
        [FunctionName("RemoveFavorite")]
        public async Task<IActionResult> RunRemoveFavorite(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", "post", Route = null)] HttpRequest req,
            ILogger log)
        {
            try
            {
                string UserId = req.Query["UserId"];
                string ItemId = req.Query["ItemId"];
                Console.WriteLine($"query params string is UserId {UserId} ItemId {ItemId}");
                Pageable<TableEntity> favoriteFilter = _favoriteTableClient.Query<TableEntity>(filter: $"UserId eq '{UserId}' and ItemId eq '{ItemId}'");
                string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
                log.LogInformation("Request body is {requestBody}", requestBody);
                FavoriteItems favorites = JsonConvert.DeserializeObject<FavoriteItems>(requestBody);
                log.LogInformation("isFavorite is {requestBody}");

                await _favoriteTableClient.UpsertEntityAsync(favorites);

                return new OkObjectResult(favoriteFilter);

            }
            catch (Exception e)
            {
                log.LogError(e, "Error with creating item. Check that it does not exist already.");
            }

            return new BadRequestObjectResult("There was an issue");
        }

    }
}