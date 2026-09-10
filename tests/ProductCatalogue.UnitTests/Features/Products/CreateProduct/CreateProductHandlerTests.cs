using Microsoft.EntityFrameworkCore;
using ProductCatalogue.Api.Data;
using ProductCatalogue.Api.Features.Products.CreateProduct;

namespace ProductCatalogue.UnitTests.Features.Products.CreateProduct;

public sealed class CreateProductHandlerTests
{
    [Fact]
    public async Task Creates_product_with_generated_id_and_values()
    {
        await using var db = CreateContext();
        var result = await new CreateProductHandler(db).Handle(new("Keyboard", 49.99m, 10), default);
        Assert.NotEqual(Guid.Empty, result.Id);
        Assert.Equal(new(result.Id, "Keyboard", 49.99m, 10), result);
        Assert.Equal(1, await db.Products.CountAsync());
    }

    private static AppDbContext CreateContext() => new(new DbContextOptionsBuilder<AppDbContext>().UseInMemoryDatabase(Guid.NewGuid().ToString()).Options);
}
