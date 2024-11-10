using Microsoft.AspNetCore.Mvc;
using ST10339829_CLDV6212_POE_FINAL.Models;
using ST10339829_CLDV6212_POE_FINAL.Services;

namespace ST10339829_CLDV6212_POE_FINAL.Controllers
{
    public class ProductController : Controller
    {
        private readonly TableService _tableService;
        public ProductController()
        {
            string connectionString = "DefaultEndpointsProtocol=https;AccountName=st10339829;AccountKey=b1RzjUhuhot2MIrD+6YOgiT2AMeWOX5b5ILd6ROUzt30pD8LVb7GnwPAGKeuP3nPyRX8lGmlwVr2+AStHgokZw==;EndpointSuffix=core.windows.net";

            _tableService = new TableService(connectionString);
        }
        public async Task<IActionResult> Index()
        {
            var products = await _tableService.GetProductsAsync();
            return View(products);
        }
        [HttpGet]
        public IActionResult Create() => View();
        [HttpPost]
        public async Task<IActionResult> Create(Product product)
        {
            if (ModelState.IsValid)
            {
                await _tableService.AddProductTableAsync(product);
                return RedirectToAction(nameof(Index));
            }
            return View(product);
        }
    }
}
