namespace FruitShelf.Data.Features.Products;

public class Seeder
{
    public static void SeedData(ProductDataContext ctx)
    {
        ctx.Products.AddRange(
            new Product
            {
                Name = "Apple", Price = 1.20, Stock = 150, Created = DateTime.UtcNow,
                LastUpdated = DateTime.UtcNow
            },
            new Product
            {
                Name = "Banana", Price = 0.50, Stock = 200, Created = DateTime.UtcNow,
                LastUpdated = DateTime.UtcNow
            },
            new Product
            {
                Name = "Mango", Price = 2.50, Stock = 80, Created = DateTime.UtcNow,
                LastUpdated = DateTime.UtcNow
            },
            new Product
            {
                Name = "Orange", Price = 0.80, Stock = 120, Created = DateTime.UtcNow,
                LastUpdated = DateTime.UtcNow
            },
            new Product
            {
                Name = "Strawberry", Price = 3.00, Stock = 60, Created = DateTime.UtcNow,
                LastUpdated = DateTime.UtcNow
            },
            new Product
            {
                Name = "Blueberry", Price = 4.00, Stock = 50, Created = DateTime.UtcNow,
                LastUpdated = DateTime.UtcNow
            },
            new Product
            {
                Name = "Pineapple", Price = 2.00, Stock = 40, Created = DateTime.UtcNow,
                LastUpdated = DateTime.UtcNow
            },
            new Product
            {
                Name = "Watermelon", Price = 5.00, Stock = 30, Created = DateTime.UtcNow,
                LastUpdated = DateTime.UtcNow
            },
            new Product
            {
                Name = "Pear", Price = 1.50, Stock = 100, Created = DateTime.UtcNow,
                LastUpdated = DateTime.UtcNow
            },
            new Product
            {
                Name = "Cherry", Price = 6.00, Stock = 25, Created = DateTime.UtcNow,
                LastUpdated = DateTime.UtcNow
            }
        );
        ctx.SaveChanges();
    }
}