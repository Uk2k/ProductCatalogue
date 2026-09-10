using Microsoft.EntityFrameworkCore;
using ProductCatalogue.Api.Data;
using ProductCatalogue.Api.Features.Products;

namespace ProductCatalogue.UnitTests.Data;

public sealed class ProductRepositoryTests
{
    [Fact]
    public async Task ListAsync_returns_products_ordered_by_name_then_id()
    {
        await using var db = CreateContext();
        db.Products.AddRange(
            new Product { Id = Guid.Parse("00000000-0000-0000-0000-000000000002"), Name = "Zed", Price = 2, Stock = 1 },
            new Product { Id = Guid.Parse("00000000-0000-0000-0000-000000000001"), Name = "Alpha", Price = 1, Stock = 1 });
        await db.SaveChangesAsync();
        var result = await new ProductRepository(db).ListAsync(default);

        Assert.Equal(["Alpha", "Zed"], result.Select(product => product.Name));
    }

    [Fact]
    public async Task GetByIdAsync_returns_matching_product()
    {
        await using var db = CreateContext();
        var id = Guid.NewGuid();
        db.Products.Add(new Product { Id = id, Name = "Keyboard", Price = 49.99m, Stock = 10 });
        await db.SaveChangesAsync();

        var result = await new ProductRepository(db).GetByIdAsync(id, default);

        Assert.Equal(id, result?.Id);
    }

    private static AppDbContext CreateContext() => new(new DbContextOptionsBuilder<AppDbContext>()
        .UseInMemoryDatabase(Guid.NewGuid().ToString()).Options);
}
