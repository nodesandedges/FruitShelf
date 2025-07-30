using Microsoft.EntityFrameworkCore;

namespace FruitShelf.Data.Features.Products;

public class ProductDataContext : DbContext
{
    public ProductDataContext(
        DbContextOptions<ProductDataContext> options) : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.UseSerialColumns(); // For Postgres
    }

    public DbSet<Product> Products { get; set; }
}