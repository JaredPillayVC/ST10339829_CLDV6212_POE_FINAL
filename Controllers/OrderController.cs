using Microsoft.AspNetCore.Mvc;
using ST10339829_CLDV6212_POE_FINAL.Services;

namespace ST10339829_CLDV6212_POE_FINAL.Controllers
{
    public class OrderController : Controller
    {
        private readonly AzureQueueService _azureQueueService;

        public OrderController()
        {
            string connectionString = "DefaultEndpointsProtocol=https;AccountName=st10339829;AccountKey=b1RzjUhuhot2MIrD+6YOgiT2AMeWOX5b5ILd6ROUzt30pD8LVb7GnwPAGKeuP3nPyRX8lGmlwVr2+AStHgokZw==;EndpointSuffix=core.windows.net";
            _azureQueueService = new AzureQueueService(connectionString);
        }

        public async Task<IActionResult> Index()
        {
            try
            {
                var queuedMessages = await _azureQueueService.RetriveMessagesAsync();
                return View(queuedMessages);
            }
            catch (System.Exception ex)
            {
                ViewBag.Error = $"Error retrieving queue messages: {ex.Message}";
                return View("Error");
            }
        }

        [HttpPost]
        public async Task<IActionResult> ProcessOrder(string order)
        {
            if (string.IsNullOrEmpty(order))
            {
                TempData["ErrorMessage"] = "Order cannot be empty.";
                return RedirectToAction(nameof(Index));
            }

            try
            {
                await _azureQueueService.CreateMessageAsync(order);
                TempData["SuccessMessage"] = "Order has been successfully processed.";
            }
            catch (System.Exception ex)
            {
                TempData["ErrorMessage"] = $"Error processing order: {ex.Message}";
            }

            return RedirectToAction(nameof(Index));
        }
    }
}
