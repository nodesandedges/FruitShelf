namespace FruitShelf.Data.Features.Products;

public interface IProductsRepository
{
    Task<Product> AddAsync(Product entity, CancellationToken cancellationToken);
    Task<List<Product>> GetAsync(CancellationToken cancellationToken);
    Task<Product?> FindByIdAsync(int id, CancellationToken cancellationToken);
    Task<bool> DeleteByIdAsync(int id, CancellationToken cancellationToken);
    Task<bool> UpdateAsync(int id, Product updatedProduct, CancellationToken cancellationToken);
}