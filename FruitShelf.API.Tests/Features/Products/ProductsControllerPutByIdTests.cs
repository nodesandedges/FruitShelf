using FluentAssertions;
using FruitShelf.Api.Features.Products;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;

namespace FruitShelf.Api.Tests.Features.Products;

public class ProductsControllerPutByIdTests
{
    private static ProductsController CreateController(Mock<IProductsService> serviceMock)
    {
        var loggerMock = new Mock<ILogger<ProductsController>>();
        return new ProductsController(loggerMock.Object, serviceMock.Object);
    }

    public static IEnumerable<object[]> UpdateTestCases =>
        new List<object[]>
        {
            new object[] { 1, true, typeof(NoContentResult) },
            new object[] { 42, false, typeof(NotFoundResult) }
        };

    [Theory]
    [MemberData(nameof(UpdateTestCases))]
    public async Task Update_ReturnsExpectedResult(int id, bool serviceReturns, Type expectedResultType)
    {
        // Arrange
        var mock = new Mock<IProductsService>();
        var dto = new ProductUpdateDto
            {
                Id = id, Name = "Updated",Price = 99.99, Stock = 10
                
            };
        mock.Setup(s => s.UpdateAsync(id, dto, It.IsAny<CancellationToken>()))
            .ReturnsAsync(serviceReturns);

        var controller = CreateController(mock);

        // Act
        var result = await controller.UpdateById(id, dto, CancellationToken.None);

        // Assert
        result.Should().BeOfType(expectedResultType);
    }
}