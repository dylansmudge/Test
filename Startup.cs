
using System;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Azure.Functions.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Azure;
using Azure.Data.Tables;

[assembly: FunctionsStartup(typeof(TableStorage.Startup))]

namespace TableStorage 
{
    public class Startup : FunctionsStartup
    {
        public override void Configure(IFunctionsHostBuilder builder) 
        {

            // Construct a new "TableServiceClient using a TableSharedKeyCredential.

                builder.Services.AddScoped<HelloAzuriteTableStorage>( (s) => {

                    string AccountName = Environment.GetEnvironmentVariable("AccountName");
                    string TableName = Environment.GetEnvironmentVariable("TableName");
                    string Uri = Environment.GetEnvironmentVariable("Uri");
                    string AccountKey = Environment.GetEnvironmentVariable("AccountKey");
    
                    TableClient tableClient = new TableClient(
                        new Uri(Uri),
                        TableName,
                        new TableSharedKeyCredential(AccountName, AccountKey));

                        return new HelloAzuriteTableStorage(tableClient);
                    });
        }

    }

}