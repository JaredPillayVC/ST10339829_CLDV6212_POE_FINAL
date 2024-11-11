using Dapper;
using System.Data;
using System.Data.SqlClient;
using ST10339829_CLDV6212_POE_FINAL.Models;
using System.Threading.Tasks;
using System.Collections.Generic;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Database;

public class CustomerRepository
{
    private readonly string _connectionString;

    public CustomerRepository(string connectionString)
    {
        _connectionString = connectionString;
    }

    public async Task<IEnumerable<Customer>> GetCustomersAsync()
    {
        using (IDbConnection db = new SqlConnection(_connectionString))
        {
            return await db.QueryAsync<Customer>("SELECT * FROM Customer");
        }
    }

    public async Task<Customer> GetCustomerByIdAsync(int customerId)
    {
        using (IDbConnection db = new SqlConnection(_connectionString))
        {
            string sql = "SELECT * FROM Customer WHERE CustomerID = @CustomerID";
            return await db.QueryFirstOrDefaultAsync<Customer>(sql, new { CustomerID = customerId });
        }
    }

    public async Task AddCustomerAsync(Customer customer)
    {
        using (var connection = new SqlConnection(_connectionString))
        {
            string sql = "INSERT INTO Customer (Name, Email, Phone) VALUES (@Name, @Email, @Phone)";
            await connection.ExecuteAsync(sql, new
            {
                customer.Name,
                customer.Email,
                customer.Phone
            });
        }
    }

    public async Task EditCustomerAsync(Customer customer)
    {
        using (IDbConnection db = new SqlConnection(_connectionString))
        {
            string sql = @"
                UPDATE Customer
                SET Name = @Name,
                    Email = @Email,
                    Phone = @Phone
                WHERE CustomerID = @CustomerID";

            await db.ExecuteAsync(sql, customer);
        }
    }

    public async Task DeleteCustomerAsync(int customerId)
    {
        using (IDbConnection db = new SqlConnection(_connectionString))
        {
            string sql = "DELETE FROM Customer WHERE CustomerID = @CustomerID";
            await db.ExecuteAsync(sql, new { CustomerID = customerId });
        }
    }

    public async Task<bool> CustomerExistsAsync(int customerId)
    {
        using (IDbConnection db = new SqlConnection(_connectionString))
        {
            string sql = "SELECT COUNT(1) FROM Customer WHERE CustomerID = @CustomerID";
            return await db.ExecuteScalarAsync<bool>(sql, new { CustomerID = customerId });
        }
    }
}
