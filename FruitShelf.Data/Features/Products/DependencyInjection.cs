using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace FruitShelf.Data.Features.Products;

public static class DependencyInjection
{
    public static IServiceCollection AddFruitShelfData(this IServiceCollection services, string connectionString)
    {
        // Register your services here
        services.AddScoped<IProductsRepository, ProductsRepository>();
        services.AddDbContext<ProductDataContext>(o =>
            o.UseNpgsql(connectionString)
        );
        return services;
    }
}