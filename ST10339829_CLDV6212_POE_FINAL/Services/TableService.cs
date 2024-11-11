using Microsoft.Azure.Cosmos.Table;
using ST10339829_CLDV6212_POE_FINAL.Models;

namespace ST10339829_CLDV6212_POE_FINAL.Services
{
    public class TableService
    {
        private readonly CloudTableClient _cloudTableClient;
        private readonly CloudTable _customerCloudTable;
        private readonly CloudTable _productCloudTable;

        public TableService(string connectionString)
        {
            CloudStorageAccount cloudStorageAccount = CloudStorageAccount.Parse(connectionString);
            _cloudTableClient = cloudStorageAccount.CreateCloudTableClient(new TableClientConfiguration());
            _customerCloudTable = _cloudTableClient.GetTableReference("Customers");
            _productCloudTable = _cloudTableClient.GetTableReference("Products");

            _customerCloudTable.CreateIfNotExists();
            _productCloudTable.CreateIfNotExists();
        }

        public async Task AddCustomerTableAsync(Customer customer)
        {
            customer.CID = await GetNextCIDAsync();

            customer.SetRowKey();

            if (string.IsNullOrEmpty(customer.PartitionKey) || string.IsNullOrEmpty(customer.RowKey))
            {
                throw new InvalidOperationException("PartitionKey and RowKey must be set.");
            }

            var insertCustomer = TableOperation.Insert(customer);
            await _customerCloudTable.ExecuteAsync(insertCustomer);
        }


        public async Task<List<Customer>> GetCustomersAsync()
        {
            var query = new TableQuery<Customer>();
            var customers = await _customerCloudTable.ExecuteQuerySegmentedAsync(query, null);
            return customers.Results;
        }

        private async Task<int> GetNextCIDAsync()
        {
            var query = new TableQuery<Customer>()
                .Select(new string[] { "CID" });

            var customers = await _customerCloudTable.ExecuteQuerySegmentedAsync(query, null);
            var maxCID = customers.Results.Max(c => c.CID.HasValue ? c.CID.Value : 0);
            return maxCID + 1;
        }

        public async Task AddProductTableAsync(Product product)
        {
            product.ProductID = await GetNextPIDAsync();

            product.SetRowKey();

            if (string.IsNullOrEmpty(product.PartitionKey) || string.IsNullOrEmpty(product.RowKey))
            {
                throw new InvalidOperationException("PartitionKey and RowKey must be set.");
            }
            var insertProduct = TableOperation.Insert(product);
            await _productCloudTable.ExecuteAsync(insertProduct);
        }
        public async Task<List<Product>> GetProductsAsync()
        {
            var query = new TableQuery<Product>();
            var products = await _productCloudTable.ExecuteQuerySegmentedAsync(query, null);
            return products.Results;
        }
        private async Task<int> GetNextPIDAsync()
        {
            var query = new TableQuery<Product>()
                .Select(new string[] { "PID" });
            var product = await _productCloudTable.ExecuteQuerySegmentedAsync(query, null);
            var maxPID = product.Results.Max(c => c.ProductID.HasValue ? c.ProductID.Value : 0);
            return maxPID + 1;
        }
    }
}
