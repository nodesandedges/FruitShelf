using FluentAssertions;
using FruitShelf.Api.Features.Products;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;

namespace FruitShelf.Api.Tests.Features.Products;

public class ProductsControllerGetByIdTests
{
    [Fact]
    public async Task GetById_ProductExists_ReturnsOkResult()
    {
        // Arrange
        var expected = CreateProductDto(1, "Test Product");
        var controller = CreateControllerWithProduct(expected.Id, expected);
        var cancellationToken = CancellationToken.None;

        // Act
        var result = await controller.GetById(expected.Id, cancellationToken);

        // Assert
        result.Result.Should().BeOfType<OkObjectResult>()
            .Which.Value.Should().BeAssignableTo<ProductDto>()
            .Subject.Should().BeEquivalentTo(expected);
    }

    [Fact]
    public async Task GetById_ProductDoesNotExist_ReturnsNotFound()
    {
        // Arrange
        var controller = CreateControllerWithProduct(42, null);
        var cancellationToken = CancellationToken.None;

        // Act
        var result = await controller.GetById(42, cancellationToken);

        // Assert
        result.Result.Should().BeOfType<NotFoundResult>();
    }

    // 🔧 Helper methods

    private static ProductsController CreateControllerWithProduct(int id, ProductDto? product)
    {
        var serviceMock = new Mock<IProductsService>();
        var loggerMock = new Mock<ILogger<ProductsController>>();

        serviceMock.Setup(s => s.GetByIdAsync(id, It.IsAny<CancellationToken>())).ReturnsAsync(product);

        return new ProductsController(loggerMock.Object, serviceMock.Object);
    }

    private static ProductDto CreateProductDto(int id, string name)
    {
        return new ProductDto
        {
            Id = id,
            Name = name
        };
    }
}