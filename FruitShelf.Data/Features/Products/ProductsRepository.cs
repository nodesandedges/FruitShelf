using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace FruitShelf.Data.Features.Products;

public class ProductsRepository(ILogger<IProductsRepository> logger, ProductDataContext db) : IProductsRepository
{
    public async Task<List<Product>> GetAsync(CancellationToken cancellationToken)
    {
        return await db.Products
            .ToListAsync(cancellationToken)
            .ConfigureAwait(false);
    }

    public async Task<Product?> FindByIdAsync(int id, CancellationToken cancellationToken)
    {
        if (id < 0) throw new ArgumentNullException(nameof(id));

        return await db.Products
            .FindAsync([id], cancellationToken)
            .ConfigureAwait(false);
    }

    public async Task<bool> DeleteByIdAsync(int id, CancellationToken cancellationToken)
    {
        using (logger.BeginScope(new Dictionary<string, object>
               {
                   ["ProductId"] = id
               }))
        {
            logger.LogInformation("Attempting to delete product.");

            var product = await db.Products
                .FindAsync([id], cancellationToken)
                .ConfigureAwait(false);

            if (product is null)
            {
                logger.LogWarning("Product not found.");
                return false;
            }

            db.Products.Remove(product);
            await db.SaveChangesAsync(cancellationToken)
                .ConfigureAwait(false);

            logger.LogInformation("Product deleted successfully.");
            return true;
        }
    }

    public async Task<bool> UpdateAsync(int id, Product updatedProduct, CancellationToken cancellationToken)
    {
        using (logger.BeginScope(new Dictionary<string, object>
               {
                   ["ProductId"] = id
               }))
        {
            logger.LogInformation("Attempting to update product.");

            if (id <= 0) throw new ArgumentOutOfRangeException(nameof(id));
            if (updatedProduct is null) throw new ArgumentNullException(nameof(updatedProduct));
            if (updatedProduct.Id != id) throw new ArgumentException("Product ID mismatch.");

            var existingProduct = await FindByIdAsync(id, cancellationToken)
                .ConfigureAwait(false);

            if (existingProduct is null)
            {
                logger.LogWarning("Product not found.");
                return false;
            }

            // Update fields (only those that should be updatable)
            var now = GetUtcNow();
            existingProduct.LastUpdated = now;
            
            existingProduct.Name = updatedProduct.Name;
            existingProduct.Price = updatedProduct.Price;
            existingProduct.Stock = updatedProduct.Stock;


            db.Products.Update(existingProduct);
            await db.SaveChangesAsync(cancellationToken);

            logger.LogInformation("Product updated successfully.");
            return true;
        }
    }

    public async Task<Product> AddAsync(Product entity, CancellationToken cancellationToken)
    {
        if (entity is null) throw new ArgumentNullException(nameof(entity));

        var now = GetUtcNow();
        entity.Created = now;
        entity.LastUpdated = now;

        db.Products.Add(entity);
        _ = await db.SaveChangesAsync(cancellationToken)
            .ConfigureAwait(false);
        return entity;
    }

    private static DateTime GetUtcNow()
    {
        return DateTime.UtcNow;
    }
}