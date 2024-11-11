using Microsoft.AspNetCore.Mvc;
using ST10339829_CLDV6212_POE_FINAL.Models;
using Microsoft.AspNetCore.Mvc;
using ST10339829_CLDV6212_POE_FINAL.Models;
using System.Threading.Tasks;

namespace ST10339829_CLDV6212_POE_FINAL.Controllers
{
    public class CustomerController : Controller
    {
        private readonly CustomerRepository _customerRepository;

        public CustomerController()
        {
            string connectionString = "Server=tcp:10339829.database.windows.net,1433;Initial Catalog=st10339829cldv6212;Persist Security Info=False;User ID=jp10339829;Password=Lexi0131;MultipleActiveResultSets=False;Encrypt=True;TrustServerCertificate=False;Connection Timeout=30;";
            _customerRepository = new CustomerRepository(connectionString);
        }

        public async Task<IActionResult> Index()
        {
            var customers = await _customerRepository.GetCustomersAsync();
            return View(customers);
        }

        [HttpGet]
        public IActionResult Create() => View();

        [HttpPost]
        public async Task<IActionResult> Create(Customer customer)
        {
            if (ModelState.IsValid)
            {
                await _customerRepository.AddCustomerAsync(customer);
                return RedirectToAction(nameof(Index));
            }
            return View(customer);
        }

        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var customer = await _customerRepository.GetCustomerByIdAsync(id);
            if (customer == null)
            {
                return NotFound();
            }
            return View(customer);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(Customer customer)
        {
            if (ModelState.IsValid)
            {
                await _customerRepository.EditCustomerAsync(customer);
                return RedirectToAction(nameof(Index));
            }
            return View(customer);
        }

        [HttpGet]
        public async Task<IActionResult> Delete(int id)
        {
            var customer = await _customerRepository.GetCustomerByIdAsync(id);
            if (customer == null)
            {
                return NotFound();
            }
            return View(customer);
        }

        [HttpPost, ActionName("Delete")]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            await _customerRepository.DeleteCustomerAsync(id);
            return RedirectToAction(nameof(Index));
        }
    }
}

