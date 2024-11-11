using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Azure.Storage.Files.Shares;

namespace AzFunctionApp.Functions
{
    public static class UploadContractFunction
    {
        // TODO: Connection String to be updated with new storage acc 
        private static string connectionString = "DefaultEndpointsProtocol=https;AccountName=st10339829;AccountKey=b1RzjUhuhot2MIrD+6YOgiT2AMeWOX5b5ILd6ROUzt30pD8LVb7GnwPAGKeuP3nPyRX8lGmlwVr2+AStHgokZw==;EndpointSuffix=core.windows.net";
        private static string shareName = "contracts";

        [FunctionName("UploadContract")]
        public static async Task<IActionResult> UploadContract(
            [HttpTrigger(AuthorizationLevel.Function, "post", Route = null)] HttpRequest req,
            ILogger log)
        {
            log.LogInformation("C# HTTP trigger function to upload contract.");

            var shareClient = new ShareClient(connectionString, shareName);
            await shareClient.CreateIfNotExistsAsync();
            var directoryClient = shareClient.GetRootDirectoryClient();

            var formdata = await req.ReadFormAsync();
            var file = formdata.Files["file"];

            if (file == null || file.Length == 0)
            {
                return new BadRequestObjectResult("Please upload a file.");
            }

            var fileClient = directoryClient.GetFileClient(file.FileName);
            await fileClient.CreateAsync(file.Length);

            using (var stream = file.OpenReadStream())
            {
                await fileClient.UploadAsync(stream);
            }

            return new OkObjectResult($"File {file.FileName} uploaded successfully.");
        }
    }
}
