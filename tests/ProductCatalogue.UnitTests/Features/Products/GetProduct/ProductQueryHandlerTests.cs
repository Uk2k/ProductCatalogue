using Microsoft.EntityFrameworkCore;
using ProductCatalogue.Api.Data;
using ProductCatalogue.Api.Features.Products;
using ProductCatalogue.Api.Features.Products.GetProduct;
using ProductCatalogue.Api.Features.Products.GetProducts;

namespace ProductCatalogue.UnitTests.Features.Products;

public sealed class ProductQueryHandlerTests
{
    [Fact]
    public async Task GetProducts_orders_by_name_then_id()
    {
        await using var db = CreateContext();
        db.Products.AddRange(new Product { Id = Guid.Parse("00000000-0000-0000-0000-000000000002"), Name = "Zed", Price = 2, Stock = 1 }, new Product { Id = Guid.Parse("00000000-0000-0000-0000-000000000001"), Name = "Alpha", Price = 1, Stock = 1 });
        await db.SaveChangesAsync();
        var result = await new GetProductsHandler(db).Handle(new(), default);
        Assert.Equal(["Alpha", "Zed"], result.Select(x => x.Name));
    }

    [Fact]
    public async Task GetProductById_returns_null_when_missing()
    {
        await using var db = CreateContext();
        var result = await new GetProductByIdHandler(db).Handle(new(Guid.NewGuid()), default);
        Assert.Null(result);
    }

    private static AppDbContext CreateContext() => new(new DbContextOptionsBuilder<AppDbContext>().UseInMemoryDatabase(Guid.NewGuid().ToString()).Options);
}
