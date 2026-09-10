using Microsoft.EntityFrameworkCore;

namespace ProductCatalogue.Api.Data;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddProductCataloguePersistence(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("ProductCatalogue");

        if (string.IsNullOrWhiteSpace(connectionString))
        {
            throw new InvalidOperationException(
                "Connection string 'ProductCatalogue' must be configured.");
        }

        services.AddDbContext<AppDbContext>(
            options => options.UseSqlServer(connectionString));
        services.AddScoped<IProductRepository, ProductRepository>();

        return services;
    }
}
