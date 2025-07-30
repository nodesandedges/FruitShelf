using FluentAssertions;
using FruitShelf.Data.Features.Products;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Moq;

namespace FruitShelf.Data.Tests.Features.Products;

public class ProductsRepositoryTests
{
    private ProductDataContext CreateInMemoryContext(string dbName)
    {
        var options = new DbContextOptionsBuilder<ProductDataContext>()
            .UseInMemoryDatabase(databaseName: dbName)
            .Options;
        return new ProductDataContext(options);
    }

    private ProductsRepository CreateRepository(ProductDataContext context)
    {
        var loggerMock = new Mock<ILogger<IProductsRepository>>();
        return new ProductsRepository(loggerMock.Object, context);
    }

    [Fact]
    public async Task AddAsync_Should_Add_Product()
    {
        // Arrange
        var context = CreateInMemoryContext("AddProductDb");
        var repo = CreateRepository(context);
        var product = new Product { Name = "Test", Price = 10.5, Stock = 5 };

        // Act
        var result = await repo.AddAsync(product, CancellationToken.None);

        // Assert
        result.Id.Should().BeGreaterThan(0);
        result.Created.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(2));
        (await context.Products.CountAsync()).Should().Be(1);
    }

    [Theory]
    [InlineData(1, true)]
    [InlineData(99, false)]
    public async Task DeleteByIdAsync_Should_Delete_If_Exists(int id, bool expectedResult)
    {
        // Arrange
        var context = CreateInMemoryContext("DeleteProductDb_" + id);
        context.Products.Add(new Product { Id = 1, Name = "ToDelete", Price = 5, Stock = 1 });
        await context.SaveChangesAsync();

        var repo = CreateRepository(context);

        // Act
        var result = await repo.DeleteByIdAsync(id, CancellationToken.None);

        // Assert
        result.Should().Be(expectedResult);
        var exists = await context.Products.FindAsync(id);
        exists.Should().BeNull();
    }

    [Fact]
    public async Task GetAsync_Should_Return_All_Products()
    {
        // Arrange
        var context = CreateInMemoryContext("GetAllDb");
        context.Products.AddRange(
            new Product { Name = "A", Price = 1, Stock = 1 },
            new Product { Name = "B", Price = 2, Stock = 2 }
        );
        await context.SaveChangesAsync();

        var repo = CreateRepository(context);

        // Act
        var products = await repo.GetAsync(CancellationToken.None);

        // Assert
        products.Should().HaveCount(2);
    }

    [Theory]
    [InlineData(1, true)]
    [InlineData(42, false)]
    public async Task FindByIdAsync_Should_Return_Product_If_Found(int id, bool expectedFound)
    {
        // Arrange
        var context = CreateInMemoryContext("FindByIdDb_" + id);
        context.Products.Add(new Product { Id = 1, Name = "FindMe", Price = 9, Stock = 2 });
        await context.SaveChangesAsync();

        var repo = CreateRepository(context);

        // Act
        var product = await repo.FindByIdAsync(id, CancellationToken.None);

        // Assert
        (product != null).Should().Be(expectedFound);
    }

    [Fact]
    public async Task UpdateAsync_Should_Return_False_If_Not_Found()
    {
        // Arrange
        var context = CreateInMemoryContext("UpdateMissingDb");
        var repo = CreateRepository(context);
        var product = new Product { Id = 10, Name = "DoesNotExist", Price = 0, Stock = 0 };

        // Act
        var result = await repo.UpdateAsync(10, product, CancellationToken.None);

        // Assert
        result.Should().BeFalse();
    }

    [Fact]
    public async Task UpdateAsync_Should_Update_Existing_Product()
    {
        // Arrange
        var context = CreateInMemoryContext("UpdateExistingDb");
        var existing = new Product { Id = 1, Name = "Old", Price = 5, Stock = 5 };
        context.Products.Add(existing);
        await context.SaveChangesAsync();

        var repo = CreateRepository(context);
        var updated = new Product { Id = 1, Name = "New", Price = 10, Stock = 20 };

        // Act
        var result = await repo.UpdateAsync(1, updated, CancellationToken.None);

        // Assert
        result.Should().BeTrue();
        var saved = await context.Products.FindAsync(1);
        saved!.Name.Should().Be("New");
        saved.Price.Should().Be(10);
        saved.Stock.Should().Be(20);
    }
}
