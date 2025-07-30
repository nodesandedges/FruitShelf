using FruitShelf.Data.Features.Products;

namespace FruitShelf.Api.Features.Products;

/// <summary>
/// ProductsService used to marshal data between the controller and Repository
/// allowing the controller to deal with only HTTP concerns and these exposed higher level CRUD operations. 
/// </summary>
public class ProductsService : IProductsService
{
    private readonly IProductsRepository _productsRepository;

    public ProductsService(IProductsRepository productsRepository)
    {
        _productsRepository = productsRepository ?? throw new ArgumentNullException(nameof(productsRepository));
    }

    public async Task<IEnumerable<ProductDto>> GetAsync(CancellationToken cancellationToken)
    {
        var products = await _productsRepository.GetAsync(cancellationToken).ConfigureAwait(false);
        return products.Select(p => p.ToDto());
    }

    public async Task<ProductDto?> GetByIdAsync(int id, CancellationToken cancellationToken) =>
        (await _productsRepository.FindByIdAsync(id, cancellationToken))?.ToDto();

    public async Task<bool> DeleteByIdAsync(int id, CancellationToken cancellationToken)
    {
        return await _productsRepository.DeleteByIdAsync(id, cancellationToken);
    }

    public async Task<ProductDto?> CreateAsync(ProductCreateDto dto, CancellationToken cancellationToken)
    {
        var product = dto.ToProduct();
        var result = await _productsRepository.AddAsync(product, cancellationToken);
        
        if (result is null) return null;
        return result.ToDto();
    }
    
    public async Task<bool> UpdateAsync(int id, ProductUpdateDto dto, CancellationToken cancellationToken)
    {
        var product = dto.ToProduct();
        var result = await _productsRepository.UpdateAsync(id, product, cancellationToken);
        return result;
    }
}