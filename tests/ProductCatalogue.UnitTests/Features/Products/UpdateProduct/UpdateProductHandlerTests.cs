using Microsoft.EntityFrameworkCore;
using ProductCatalogue.Api.Data;
using ProductCatalogue.Api.Features.Products;
using ProductCatalogue.Api.Features.Products.UpdateProduct;

namespace ProductCatalogue.UnitTests.Features.Products.UpdateProduct;

public sealed class UpdateProductHandlerTests
{
    [Fact]
    public async Task Updates_existing_product()
    {
        await using var db = CreateContext();
        var id = Guid.NewGuid();
        db.Products.Add(new Product { Id = id, Name = "Old", Price = 1, Stock = 1 });
        await db.SaveChangesAsync();
        var result = await new UpdateProductHandler(new ProductRepository(db)).Handle(new(id, "New", 2, 3), default);
        Assert.Equal(new(id, "New", 2, 3), result);
    }

    [Fact]
    public async Task Returns_null_for_missing_product()
    {
        await using var db = CreateContext();
        Assert.Null(await new UpdateProductHandler(new ProductRepository(db)).Handle(new(Guid.NewGuid(), "New", 2, 3), default));
    }

    private static AppDbContext CreateContext() => new(new DbContextOptionsBuilder<AppDbContext>().UseInMemoryDatabase(Guid.NewGuid().ToString()).Options);
}
