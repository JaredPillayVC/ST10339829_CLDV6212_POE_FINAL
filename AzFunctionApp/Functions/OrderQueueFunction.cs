using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Azure.Storage.Queues;
using System.Collections.Generic;

namespace AzFunctionApp.Functions
{
    public static class OrderQueueFunction
    {
        // TODO: Connection String to be updated with new storage acc 
        private static string connectionString = "DefaultEndpointsProtocol=https;AccountName=st10339829;AccountKey=b1RzjUhuhot2MIrD+6YOgiT2AMeWOX5b5ILd6ROUzt30pD8LVb7GnwPAGKeuP3nPyRX8lGmlwVr2+AStHgokZw==;EndpointSuffix=core.windows.net";
        private static string queueName = "process-order";

        [FunctionName("AddOrderToQueue")]
        public static async Task<IActionResult> AddOrderToQueue(
            [HttpTrigger(AuthorizationLevel.Function, "post", Route = "order")] HttpRequest req,
            ILogger log)
        {
            string order = await req.ReadAsStringAsync();
            if (string.IsNullOrEmpty(order))
            {
                return new BadRequestObjectResult("Please provide order details.");
            }

            var queueClient = new QueueClient(connectionString, queueName);
            await queueClient.CreateIfNotExistsAsync();
            await queueClient.SendMessageAsync(order);

            log.LogInformation("Order added to queue.");
            return new OkObjectResult($"Order queued successfully: {order}");
        }

        [FunctionName("RetrieveOrdersFromQueue")]
        public static async Task<IActionResult> RetrieveOrdersFromQueue(
        [HttpTrigger(AuthorizationLevel.Function, "get", Route = "order")] HttpRequest req,
        ILogger log)
        {
            var queueClient = new QueueClient(connectionString, queueName);
            if (!await queueClient.ExistsAsync())
            {
                return new NotFoundObjectResult("Queue not found.");
            }

            var messages = await queueClient.ReceiveMessagesAsync(maxMessages: 32);

            var orderDetails = new List<string>();

            foreach (var message in messages.Value)
            {
                log.LogInformation($"Processing message: {message.MessageText}");
                orderDetails.Add(message.MessageText);
                await queueClient.DeleteMessageAsync(message.MessageId, message.PopReceipt);
            }

            return new OkObjectResult(orderDetails);
        }
    }
}
