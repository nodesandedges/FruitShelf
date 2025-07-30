using Microsoft.EntityFrameworkCore;
namespace FruitShelf.Data.Features.Products;
using Microsoft.EntityFrameworkCore.Design;

public class ProductDataContextFactory : IDesignTimeDbContextFactory<ProductDataContext>
{
    public ProductDataContext CreateDbContext(string[] args)
    {
        // Create Serilog configuration, this could be obtained from configuration

        Console.WriteLine($"Starting {nameof(ProductDataContextFactory)}...");

        const string dbConnectionString = "DB_CONNECTION_STRING";
        var connectionString = Environment.GetEnvironmentVariable(dbConnectionString);

        Console.WriteLine("Read connection string");
        var optionsBuilder = new DbContextOptionsBuilder<ProductDataContext>();
        if (String.IsNullOrWhiteSpace(connectionString))
        {
            throw new Exception($"No connection string configured for {dbConnectionString}");
        }
        optionsBuilder.UseNpgsql(connectionString);

        return new ProductDataContext(optionsBuilder.Options);
    }
}