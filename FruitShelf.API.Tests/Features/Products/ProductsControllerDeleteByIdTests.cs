using FluentAssertions;
using FruitShelf.Api.Features.Products;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;

namespace FruitShelf.Api.Tests.Features.Products;

public class ProductsControllerDeleteByIdTests
{
    [Fact]
    public async Task DeleteeById_ProductExists_ReturnsOkResult()
    {
        // Arrange
        var controller = CreateControllerWithProduct(1, true);
        var cancellationToken = CancellationToken.None;

        // Act
        var result = await controller.DeleteById(1, cancellationToken);

        // Assert
        result.Result.Should().BeOfType<NoContentResult>();
    }

    [Fact]
    public async Task DeleteById_ProductDoesNotExist_ReturnsNotFound()
    {
        // Arrange
        var controller = CreateControllerWithProduct(42, false);
        var cancellationToken = CancellationToken.None;

        // Act
        var result = await controller.DeleteById(42, cancellationToken);

        // Assert
        result.Result.Should().BeOfType<NotFoundResult>();
    }

    // 🔧 Helper methods

    private static ProductsController CreateControllerWithProduct(int id, bool result)
    {
        var serviceMock = new Mock<IProductsService>();
        var loggerMock = new Mock<ILogger<ProductsController>>();

        serviceMock.Setup(s => s.DeleteByIdAsync(id, It.IsAny<CancellationToken>())).ReturnsAsync(result);

        return new ProductsController(loggerMock.Object, serviceMock.Object);
    }
}