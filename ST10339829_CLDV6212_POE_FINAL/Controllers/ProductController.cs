using Microsoft.AspNetCore.Mvc;
using ST10339829_CLDV6212_POE_FINAL.Models;
using System.Threading.Tasks;

namespace ST10339829_CLDV6212_POE_FINAL.Controllers
{
    public class ProductController : Controller
    {
        private readonly ProductRepository _productRepository;

        public ProductController()
        {
            string connectionString = "Server=tcp:10339829.database.windows.net,1433;Initial Catalog=st10339829cldv6212;Persist Security Info=False;User ID=jp10339829;Password=Lexi0131;MultipleActiveResultSets=False;Encrypt=True;TrustServerCertificate=False;Connection Timeout=30;";
            _productRepository = new ProductRepository(connectionString);
        }

        public async Task<IActionResult> Index()
        {
            var products = await _productRepository.GetProductsAsync();
            return View(products);
        }

        [HttpGet]
        public IActionResult Create() => View();

        [HttpPost]
        public async Task<IActionResult> Create(Product product)
        {
            if (ModelState.IsValid)
            {
                await _productRepository.AddProductAsync(product);
                return RedirectToAction(nameof(Index));
            }
            return View(product);
        }

        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var productExists = await _productRepository.ProductExistsAsync(id);
            if (!productExists)
            {
                return NotFound();
            }

            var product = await _productRepository.GetProductByIdAsync(id);
            return View(product);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(Product product)
        {
            if (ModelState.IsValid)
            {
                await _productRepository.EditProductAsync(product);
                return RedirectToAction(nameof(Index));
            }
            return View(product);
        }

        [HttpGet]
        public async Task<IActionResult> Delete(int id)
        {
            var productExists = await _productRepository.ProductExistsAsync(id);
            if (!productExists)
            {
                return NotFound();
            }

            var product = await _productRepository.GetProductByIdAsync(id); 
            return View(product);
        }

        [HttpPost, ActionName("Delete")]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            await _productRepository.DeleteProductAsync(id);
            return RedirectToAction(nameof(Index));
        }
    }
}
