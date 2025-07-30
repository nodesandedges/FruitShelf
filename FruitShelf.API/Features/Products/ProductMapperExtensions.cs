using FruitShelf.Data.Features.Products;

namespace FruitShelf.Api.Features.Products;

/// <summary>
/// Simple mapper class, a fuller object could use AutoMapper or similar.
/// </summary>
public static class ProductMapperExtensions
{
    public static ProductDto ToDto(this Product product)
    {
        if (product is null) return null!;

        return new ProductDto
        {
            Id = product.Id,
            Name = product.Name,
            Price = product.Price,
            Stock = product.Stock,
            Created = product.Created,
            LastUpdated = product.LastUpdated
        };
    }

    public static Product ToProduct(this ProductCreateDto dto)
    {
        if (dto is null) return null!;
        
        return new Product
        {
            Name = dto.Name,
            Price = dto.Price,
            Stock = dto.Stock
        };
    }
    public static Product ToProduct(this ProductUpdateDto dto)
    {
        if (dto is null) return null!;
        
        return new Product
        {
            Id = dto.Id,
            Name = dto.Name,
            Price = dto.Price,
            Stock = dto.Stock
        };
    }
}