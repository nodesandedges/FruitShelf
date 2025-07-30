using System.ComponentModel.DataAnnotations;

namespace FruitShelf.Data.Features.Products;

public class Product
{
    [Required] public int Id { get; set; }
    [Required] public string Name { get; set; } = "";

    [Required] public double Price { get; set; }
    [Required] public int Stock { get; set; }

    public DateTime Created { get; set; }
    public DateTime LastUpdated { get; set; }
}