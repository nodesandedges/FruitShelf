using System.ComponentModel.DataAnnotations;

namespace FruitShelf.Api.Features.Products;

public class ProductCreateDto
{
    [Required]
    [StringLength(100, MinimumLength = 2)]
    public string Name { get; set; } = string.Empty;

    [Range(0, 1000)] public int Stock { get; set; }

    [Range(0.00, 10_000)] public double Price { get; set; }
}