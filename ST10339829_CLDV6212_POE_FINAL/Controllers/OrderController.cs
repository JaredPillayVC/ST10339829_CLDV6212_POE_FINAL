using Microsoft.AspNetCore.Mvc;
using ST10339829_CLDV6212_POE_FINAL.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

public class OrderController : Controller
{
    private readonly OrderRepository _orderRepository;
    private readonly CustomerRepository _customerRepository;
    private readonly ProductRepository _productRepository;

    public OrderController()
    {
        string connectionString = "Server=tcp:10339829.database.windows.net,1433;Initial Catalog=st10339829cldv6212;Persist Security Info=False;User ID=jp10339829;Password=Lexi0131;MultipleActiveResultSets=False;Encrypt=True;TrustServerCertificate=False;Connection Timeout=30;";
        _orderRepository = new OrderRepository(connectionString);
        _customerRepository = new CustomerRepository(connectionString);
        _productRepository = new ProductRepository(connectionString);

    }

    public async Task<IActionResult> Index()
    {
        List<Order> orders = (await _orderRepository.GetOrdersAsync()).ToList();
        return View(orders);
    }

    [HttpPost]
    public async Task<IActionResult> Index(Order order)
    {
        bool customerExists = await _customerRepository.CustomerExistsAsync(order.CustomerID);
        if (!customerExists)
        {
            ModelState.AddModelError("CustomerID", "The specified Customer ID does not exist.");
            List<Order> orders = (await _orderRepository.GetOrdersAsync()).ToList();
            return View("Index", orders); 
        }
        bool productExists = await _productRepository.ProductExistsAsync(order.ProductID);
        if (!productExists)
        {
            ModelState.AddModelError("ProductID", "The specified Product ID does not exist.");
            List<Order> orders = (await _orderRepository.GetOrdersAsync()).ToList();
            return View("Index", orders);
        }
        if (ModelState.IsValid)
        {
            await _orderRepository.AddOrderAsync(order);
            return RedirectToAction(nameof(Index));
        }

        List<Order> currentOrders = (await _orderRepository.GetOrdersAsync()).ToList();
        return View("Index", currentOrders); 
    }
}

