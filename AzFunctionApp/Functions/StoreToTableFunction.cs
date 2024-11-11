using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Microsoft.Azure.Cosmos.Table;
using ST10339829_CLDV6212_POE_FINAL.Models;

namespace AzFunctionApp.Functions
{
    public static class StoreToTableFunction
    {
        [FunctionName("StoreToTableFunction")]
        public static async Task<IActionResult> StoreToTable(
            [HttpTrigger(AuthorizationLevel.Function, "post", "put", Route = "store/{entityType}")] HttpRequest req,
            string entityType,
            ILogger log)
        {
            log.LogInformation("Processing request to store or update an entity.");
            string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            CloudStorageAccount storageAccount = CloudStorageAccount.Parse(Environment.GetEnvironmentVariable("AzureWebJobsStorage"));
            CloudTableClient tableClient = storageAccount.CreateCloudTableClient(new TableClientConfiguration());

            try
            {
                if (entityType.ToLower() == "customer")
                {
                    var customer = JsonConvert.DeserializeObject<Customer>(requestBody);
                    customer.SetRowKey(); // Set RowKey for the customer
                    CloudTable customerTable = tableClient.GetTableReference("Customers");
                    await customerTable.CreateIfNotExistsAsync();

                    TableOperation insertOrReplaceOperation = TableOperation.InsertOrReplace(customer);
                    await customerTable.ExecuteAsync(insertOrReplaceOperation);

                    return new OkObjectResult("Customer data stored or updated in Azure Table.");
                }
                else if (entityType.ToLower() == "product")
                {
                    var product = JsonConvert.DeserializeObject<Product>(requestBody);
                    product.SetRowKey(); // Set RowKey for the product
                    CloudTable productTable = tableClient.GetTableReference("Products");
                    await productTable.CreateIfNotExistsAsync();

                    TableOperation insertOrReplaceOperation = TableOperation.InsertOrReplace(product);
                    await productTable.ExecuteAsync(insertOrReplaceOperation);

                    return new OkObjectResult("Product data stored or updated in Azure Table.");
                }
                else
                {
                    return new BadRequestObjectResult("Invalid entity type. Use 'customer' or 'product'.");
                }
            }
            catch (StorageException ex)
            {
                log.LogError($"Error storing data in Azure Table Storage: {ex.Message}");
                return new StatusCodeResult(500);
            }
        }

        [FunctionName("GetEntityById")]
        public static async Task<IActionResult> GetEntityById(
        [HttpTrigger(AuthorizationLevel.Function, "get", Route = "get/{entityType}/{partitionKey}/{rowKey}")] HttpRequest req,
        string entityType,
        string partitionKey,
        string rowKey,
        ILogger log)
        {
            log.LogInformation("Getting an entity by ID.");
            CloudStorageAccount storageAccount = CloudStorageAccount.Parse(Environment.GetEnvironmentVariable("AzureWebJobsStorage"));
            CloudTableClient tableClient = storageAccount.CreateCloudTableClient(new TableClientConfiguration());

            try
            {
                CloudTable table = tableClient.GetTableReference(entityType.ToLower() == "customer" ? "Customers" : "Products");
                TableOperation retrieveOperation = TableOperation.Retrieve<Customer>(partitionKey, rowKey);
                var result = await table.ExecuteAsync(retrieveOperation);

                if (result.Result == null)
                {
                    return new NotFoundResult();
                }

                // Explicitly cast the result to the appropriate entity
                if (entityType.ToLower() == "customer")
                {
                    var customer = result.Result as Customer;
                    return new OkObjectResult(customer);
                }
                else if (entityType.ToLower() == "product")
                {
                    var product = result.Result as Product;
                    return new OkObjectResult(product);
                }
                else
                {
                    return new BadRequestObjectResult("Invalid entity type.");
                }
            }
            catch (StorageException ex)
            {
                log.LogError($"Error retrieving entity: {ex.Message}");
                return new StatusCodeResult(500);
            }
        }


        [FunctionName("GetAllEntities")]
        public static async Task<IActionResult> GetAllEntities(
            [HttpTrigger(AuthorizationLevel.Function, "get", Route = "get-all/{entityType}")] HttpRequest req,
            string entityType,
            ILogger log)
        {
            log.LogInformation("Getting all entities.");
            CloudStorageAccount storageAccount = CloudStorageAccount.Parse(Environment.GetEnvironmentVariable("AzureWebJobsStorage"));
            CloudTableClient tableClient = storageAccount.CreateCloudTableClient(new TableClientConfiguration());

            try
            {
                CloudTable table = tableClient.GetTableReference(entityType.ToLower() == "customer" ? "Customers" : "Products");
                TableQuery<DynamicTableEntity> query = new TableQuery<DynamicTableEntity>();
                var entities = await table.ExecuteQuerySegmentedAsync(query, null);

                return new OkObjectResult(entities.Results);
            }
            catch (StorageException ex)
            {
                log.LogError($"Error retrieving entities: {ex.Message}");
                return new StatusCodeResult(500);
            }
        }

        [FunctionName("DeleteEntity")]
        public static async Task<IActionResult> DeleteEntity(
            [HttpTrigger(AuthorizationLevel.Function, "delete", Route = "delete/{entityType}/{partitionKey}/{rowKey}")] HttpRequest req,
            string entityType,
            string partitionKey,
            string rowKey,
            ILogger log)
        {
            log.LogInformation("Deleting an entity.");
            CloudStorageAccount storageAccount = CloudStorageAccount.Parse(Environment.GetEnvironmentVariable("AzureWebJobsStorage"));
            CloudTableClient tableClient = storageAccount.CreateCloudTableClient(new TableClientConfiguration());

            try
            {
                CloudTable table = tableClient.GetTableReference(entityType.ToLower() == "customer" ? "Customers" : "Products");
                TableOperation retrieveOperation = TableOperation.Retrieve(partitionKey, rowKey);
                var result = await table.ExecuteAsync(retrieveOperation);

                if (result.Result != null)
                {
                    TableOperation deleteOperation = TableOperation.Delete((ITableEntity)result.Result);
                    await table.ExecuteAsync(deleteOperation);
                    return new OkObjectResult($"{entityType} deleted successfully.");
                }
                else
                {
                    return new NotFoundResult();
                }
            }
            catch (StorageException ex)
            {
                log.LogError($"Error deleting entity: {ex.Message}");
                return new StatusCodeResult(500);
            }
        }
    }
}
