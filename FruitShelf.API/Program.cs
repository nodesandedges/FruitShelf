using FruitShelf.Api.Features.Products;
using FruitShelf.Api.Middleware;
using FruitShelf.Data.Features.Products;
using Serilog;
using Serilog.Formatting.Compact;

namespace FruitShelf.Api;

public class Program
{
    private const string ConfigurationProductsdb = "ProductsDb";

    public static void Main(string[] args)
    {
        // Create Serilog configuration, this could be obtained from configuration
        Log.Logger = new LoggerConfiguration()
            .Enrich.FromLogContext()
            .WriteTo.Console(new RenderedCompactJsonFormatter())
            .CreateLogger();

        try
        {
            Log.Information("Starting FruitShelf API");

            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.

            builder.Services.AddSerilog();

            builder.Services.AddControllers();
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();


            builder.Services.AddTransient<IProductsService, ProductsService>();

            var connectionString = builder.Configuration.GetConnectionString(ConfigurationProductsdb);
            if (String.IsNullOrWhiteSpace(connectionString))
            {
                throw new Exception($"No connection string configured for {ConfigurationProductsdb}");
            }

            builder.Services.AddScoped<CorrelationIdMiddleware>();
            builder.Services.AddFruitShelfData(connectionString);
            builder.Host.UseSerilog();

            var app = builder.Build();

            app.UseMiddleware<SanitisedErrorHandlingMiddleware>();
            app.UseMiddleware<RequestLoggingMiddleware>();
            app.UseMiddleware<CorrelationIdMiddleware>();

            app.UseSerilogRequestLogging(); // In this demo with also have the example of RequestLoggingMiddleware

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                SeedDatabaseOnce(app);

                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseHttpsRedirection();

            app.UseAuthorization();

            app.MapControllers();

            app.Run();
        }
        catch (Exception ex)
        {
            Log.Fatal(ex, "Application terminated unexpectedly");
        }
        finally
        {
            Log.CloseAndFlush(); // Try to write any remaining log messages.
        }
    }

    private static void SeedDatabaseOnce(WebApplication app)
    {
        // Seed some Data into the Database for this sample
        using var scope = app.Services.CreateScope();
        var ctx = scope.ServiceProvider.GetRequiredService<ProductDataContext>();
        if (!ctx.Products.Any())
        {
            Seeder.SeedData(ctx);
        }
    }
}