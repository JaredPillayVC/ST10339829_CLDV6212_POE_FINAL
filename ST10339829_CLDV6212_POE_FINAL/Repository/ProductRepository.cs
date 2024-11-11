using System.Data;
using System.Data.SqlClient;
using Dapper;
using ST10339829_CLDV6212_POE_FINAL.Models;
using System.Threading.Tasks;
using System.Collections.Generic;

public class ProductRepository
{
    private readonly string _connectionString;

    public ProductRepository(string connectionString)
    {
        _connectionString = connectionString;
    }

    public async Task<IEnumerable<Product>> GetProductsAsync()
    {
        using (IDbConnection db = new SqlConnection(_connectionString))
        {
            return await db.QueryAsync<Product>("SELECT * FROM Product");
        }
    }

    public async Task<Product> GetProductByIdAsync(int productId)
    {
        using (IDbConnection db = new SqlConnection(_connectionString))
        {
            string sql = "SELECT * FROM Product WHERE ProductID = @ProductID";
            return await db.QueryFirstOrDefaultAsync<Product>(sql, new { ProductID = productId });
        }
    }

    public async Task AddProductAsync(Product product)
    {
        using (var connection = new SqlConnection(_connectionString))
        {
            string sql = @"
                INSERT INTO Product (ProductID, Name, Price, Stock)
                VALUES (@ProductID, @Name, @Price, @Stock)";

            await connection.ExecuteAsync(sql, new
            {
                ProductID = product.ProductID,
                Name = product.Name,
                Price = product.Price,
                Stock = product.Stock
            });
        }
    }

    public async Task<bool> ProductExistsAsync(int productId)
    {
        using (IDbConnection db = new SqlConnection(_connectionString))
        {
            string sql = "SELECT COUNT(1) FROM Product WHERE ProductID = @ProductID";
            return await db.ExecuteScalarAsync<bool>(sql, new { ProductID = productId });
        }
    }

    public async Task EditProductAsync(Product product)
    {
        using (var connection = new SqlConnection(_connectionString))
        {
            string sql = @"
                UPDATE Product
                SET Name = @Name,
                    Price = @Price,
                    Stock = @Stock
                WHERE ProductID = @ProductID";

            await connection.ExecuteAsync(sql, new
            {
                ProductID = product.ProductID,
                Name = product.Name,
                Price = product.Price,
                Stock = product.Stock
            });
        }
    }

    public async Task DeleteProductAsync(int productId)
    {
        using (var connection = new SqlConnection(_connectionString))
        {
            string sql = "DELETE FROM Product WHERE ProductID = @ProductID";
            await connection.ExecuteAsync(sql, new { ProductID = productId });
        }
    }
}
