namespace FruitShelf.Api.Features.Products;

public interface IProductsService
{
    Task<IEnumerable<ProductDto>> GetAsync(CancellationToken cancellationToken);
    Task<ProductDto?> GetByIdAsync(int id, CancellationToken cancellationToken);
    Task<bool> DeleteByIdAsync(int id, CancellationToken cancellationToken);
    Task<ProductDto?> CreateAsync(ProductCreateDto dto, CancellationToken cancellationToken);
    Task<bool> UpdateAsync(int id, ProductUpdateDto dto, CancellationToken cancellationToken);
}