using Microsoft.AspNetCore.Mvc;

namespace FruitShelf.Api.Features.Products;

[ApiController]
[Route("api/products")]
public class ProductsController : ControllerBase
{
    private readonly ILogger<ProductsController> _logger;
    private readonly IProductsService _productsService;

    public ProductsController(ILogger<ProductsController> logger, IProductsService productsService)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _productsService = productsService ?? throw new ArgumentNullException(nameof(productsService));
    }

    [HttpGet("")]
    [ProducesResponseType(typeof(ProductDto), StatusCodes.Status200OK)]
    public Task<IEnumerable<ProductDto>> Get(CancellationToken cancellationToken)
    {
        return _productsService.GetAsync(cancellationToken);
    }

    [HttpGet("{id:int}")]
    [ProducesResponseType(typeof(ProductDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ProductDto>> GetById(int id, CancellationToken cancellationToken)
    {
        var product = await _productsService.GetByIdAsync(id, cancellationToken);

        return product switch
        {
            null => NotFound(), // Returns 404 with optional content
            _ => Ok(product) // Returns 200 with the product
        };
    }
    
    [HttpDelete("{id:int}")]
    [ProducesResponseType(typeof(ProductDto), StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ProductDto>> DeleteById(int id, CancellationToken cancellationToken)
    {
        var deleted = await _productsService.DeleteByIdAsync(id, cancellationToken);

        return deleted switch
        {
            true => NoContent(),
            false => NotFound(),
        };
    }

    [HttpPost]
    [ProducesResponseType(typeof(ProductDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<ProductDto>> Create(ProductCreateDto dto, CancellationToken cancellationToken)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        var createdProduct = await _productsService.CreateAsync(dto, cancellationToken);

        return CreatedAtAction(
            nameof(GetById),
            new { id = createdProduct!.Id },
            createdProduct);
    }
    
    [HttpPut("{id:int}")]
    public async Task<IActionResult> UpdateById(int id, [FromBody] ProductUpdateDto product, CancellationToken cancellationToken)
    {
        var updated = await _productsService.UpdateAsync(id, product, cancellationToken);
        return updated ? NoContent() : NotFound();
    }

}