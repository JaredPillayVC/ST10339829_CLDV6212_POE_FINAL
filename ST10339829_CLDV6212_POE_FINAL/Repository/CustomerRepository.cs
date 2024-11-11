using Dapper;
using System.Data;
using System.Data.SqlClient;
using ST10339829_CLDV6212_POE_FINAL.Models;

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
            return await db.QueryAsync<Customer>("SELECT * FROM Customers");
        }
    }

    public async Task AddCustomerAsync(Customer customer)
    {
        using (IDbConnection db = new SqlConnection(_connectionString))
        {
            string sql = "INSERT INTO Customers (FirstName, LastName, Email, PhoneNumber) VALUES (@FirstName, @LastName, @Email, @PhoneNumber)";
            await db.ExecuteAsync(sql, customer);
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
