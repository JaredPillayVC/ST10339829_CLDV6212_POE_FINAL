using Dapper;
using System.Data;
using System.Data.SqlClient;
using ST10339829_CLDV6212_POE_FINAL.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

public class OrderRepository
{
    private readonly string _connectionString;

    public OrderRepository(string connectionString)
    {
        _connectionString = connectionString;
    }

    public async Task<IEnumerable<Order>> GetOrdersAsync()
    {
        using (IDbConnection db = new SqlConnection(_connectionString))
        {
            // Retrieve all orders with relevant fields
            return await db.QueryAsync<Order>("SELECT OrderID, CustomerID, ProductID, OrderDate, Quantity, TotalAmount FROM [Order]");
        }
    }

    public async Task AddOrderAsync(Order order)
    {
        using (IDbConnection db = new SqlConnection(_connectionString))
        {
            // SQL query includes all columns for the new Order model
            string sql = @"
                INSERT INTO [Order] (CustomerID, ProductID, OrderDate, Quantity, TotalAmount) 
                VALUES (@CustomerID, @ProductID, @OrderDate, @Quantity, @TotalAmount)";

            await db.ExecuteAsync(sql, order);
        }
    }

    public async Task<Order> GetOrderByIdAsync(int orderId)
    {
        using (IDbConnection db = new SqlConnection(_connectionString))
        {
            string sql = "SELECT OrderID, CustomerID, ProductID, OrderDate, Quantity, TotalAmount FROM [Order] WHERE OrderID = @OrderID";
            return await db.QueryFirstOrDefaultAsync<Order>(sql, new { OrderID = orderId });
        }
    }

    public async Task UpdateOrderAsync(Order order)
    {
        using (IDbConnection db = new SqlConnection(_connectionString))
        {
            // Update query to modify all columns except OrderID
            string sql = @"
                UPDATE [Order]
                SET CustomerID = @CustomerID,
                    ProductID = @ProductID,
                    OrderDate = @OrderDate,
                    Quantity = @Quantity,
                    TotalAmount = @TotalAmount
                WHERE OrderID = @OrderID";

            await db.ExecuteAsync(sql, order);
        }
    }

    public async Task DeleteOrderAsync(int orderId)
    {
        using (IDbConnection db = new SqlConnection(_connectionString))
        {
            string sql = "DELETE FROM [Order] WHERE OrderID = @OrderID";
            await db.ExecuteAsync(sql, new { OrderID = orderId });
        }
    }
}
