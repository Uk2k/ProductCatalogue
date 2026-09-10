using Microsoft.EntityFrameworkCore;
using ProductCatalogue.Api.Data;
using ProductCatalogue.Api.Features.Products;
using ProductCatalogue.Api.Features.Products.DeleteProduct;

namespace ProductCatalogue.UnitTests.Features.Products.DeleteProduct;

public sealed class DeleteProductHandlerTests
{
    [Fact]
    public async Task Deletes_existing_product()
    {
        await using var db = CreateContext();
        var id = Guid.NewGuid();
        db.Products.Add(new Product { Id = id, Name = "Delete", Price = 1, Stock = 1 });
        await db.SaveChangesAsync();
        Assert.True(await new DeleteProductHandler(db).Handle(new(id), default));
        Assert.False(await db.Products.AnyAsync());
    }

    [Fact]
    public async Task Returns_false_for_missing_product()
    {
        await using var db = CreateContext();
        Assert.False(await new DeleteProductHandler(db).Handle(new(Guid.NewGuid()), default));
    }

    private static AppDbContext CreateContext() => new(new DbContextOptionsBuilder<AppDbContext>().UseInMemoryDatabase(Guid.NewGuid().ToString()).Options);
}
