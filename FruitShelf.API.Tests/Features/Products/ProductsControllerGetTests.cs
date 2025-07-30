using FluentAssertions;
using FruitShelf.Api.Features.Products;
using Microsoft.Extensions.Logging;
using Moq;

namespace FruitShelf.Api.Tests.Features.Products;

public class ProductsControllerGetTests
{
    [Fact]
    public async Task Get_ReturnsProductList()
    {
        // Arrange
        var expectedProducts = new List<ProductDto>
        {
            CreateProductDto(1, "Apple"),
            CreateProductDto(2, "Banana")
        };
        var cancellationToken = CancellationToken.None;

        var controller = CreateControllerWithProducts(expectedProducts);

        // Act
        var result = await controller.Get(cancellationToken);

        // Assert
        result.Should().NotBeNull();
        result.Should().BeEquivalentTo(expectedProducts);
    }

    [Fact]
    public async Task Get_ReturnsEmptyList_WhenNoProductsExist()
    {
        // Arrange
        var cancellationToken = CancellationToken.None;
        var controller = CreateControllerWithProducts(new List<ProductDto>());

        // Act
        var result = await controller.Get(cancellationToken);

        // Assert
        result.Should().NotBeNull();
        result.Should().BeEmpty();
    }

    // 🔧 Helper methods

    private static ProductsController CreateControllerWithProducts(IEnumerable<ProductDto> products)
    {
        var serviceMock = new Mock<IProductsService>();
        var loggerMock = new Mock<ILogger<ProductsController>>();
        var cancellationToken = CancellationToken.None;

        serviceMock.Setup(s => s.GetAsync(cancellationToken)).ReturnsAsync(products);

        return new ProductsController(loggerMock.Object, serviceMock.Object);
    }

    private static ProductDto CreateProductDto(int id, string name) =>
        new() { Id = id, Name = name };
}