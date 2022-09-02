
using System;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Azure.Functions.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Azure;
using Azure.Data.Tables;
using Azure.Storage.Blobs;

[assembly: FunctionsStartup(typeof(TableStorage.Startup))]

namespace TableStorage 
{
    public class Startup : FunctionsStartup
    {
        public override void Configure(IFunctionsHostBuilder builder) 
        {
            string AccountName = Environment.GetEnvironmentVariable("AccountName");
            string TableItem = Environment.GetEnvironmentVariable("TableItem");
            string TableUser = Environment.GetEnvironmentVariable("TableUser");
            string TableImage = Environment.GetEnvironmentVariable("TableImage");
            string TableFavorite = Environment.GetEnvironmentVariable("TableFavorites");
            string Uri = Environment.GetEnvironmentVariable("Uri");
            string imageConnectionString = Environment.GetEnvironmentVariable("BlobUri");
            string blobContainerName = Environment.GetEnvironmentVariable("BlobImage");
            string AccountKey = Environment.GetEnvironmentVariable("AccountKey");

            TableClient itemTableClient = new TableClient(new Uri(Uri), 
                TableItem, 
                new TableSharedKeyCredential(AccountName, AccountKey));

            builder.Services.AddSingleton<TableClient>( (s) => {
                    return itemTableClient;
                });

            builder.Services.AddSingleton<HelloAzuriteTableStorage>( (s) => {
                    return new HelloAzuriteTableStorage(itemTableClient);
                });

            TableClient userTableClient = new TableClient(new Uri(Uri), 
                TableUser, 
                new TableSharedKeyCredential(AccountName, AccountKey));

            builder.Services.AddSingleton<TableClient>( (s) => {
                    return userTableClient;
                });

            builder.Services.AddSingleton<UserRequests>( (s) => {
                    return new UserRequests(userTableClient);
                });

            TableClient favoriteTableClient = new TableClient(new Uri(Uri), 
                TableFavorite, 
                new TableSharedKeyCredential(AccountName, AccountKey));

            builder.Services.AddSingleton<TableClient>( (s) => {
                    return favoriteTableClient;
                });

            builder.Services.AddSingleton<Favorites>( (s) => {
                    return new Favorites(favoriteTableClient);
                });

        }

    }

}