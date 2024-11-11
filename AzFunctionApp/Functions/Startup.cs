using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using ST10339829_CLDV6212_POE_FINAL.Services;
using Microsoft.Azure.Functions.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection;

namespace AzFunctionApp.Functions
{
    public class Startup : FunctionsStartup
    {
        public override void Configure(IFunctionsHostBuilder builder)
        {
            // Register AzureBlobService as a singleton service
            string connectionString = Environment.GetEnvironmentVariable("AzureWebJobsStorage");
            builder.Services.AddSingleton(new AzureBlobService(connectionString));
        }
    }
}
