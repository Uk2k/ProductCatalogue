using ProductCatalogue.IntegrationTests.TestFramework;

namespace ProductCatalogue.IntegrationTests;

[Collection(IntegrationTestCollection.Name)]
public sealed class PersistenceTests(SqlServerContainerFixture fixture)
{
    [Fact]
    public async Task InitialCreate_creates_products_table()
    {
        await using var database = await fixture.CreateDatabaseAsync();

        Assert.Empty(await database.Context.Database.GetPendingMigrationsAsync());
        _ = await database.Context.Products.Take(0).ToListAsync();
    }

    [Fact]
    public async Task Product_can_be_saved_and_read_by_guid()
    {
        await using var database = await fixture.CreateDatabaseAsync();
        var productId = Guid.NewGuid();

        database.Context.Products.Add(new Product
        {
            Id = productId,
            Name = "Integration product",
            Price = 12.34m,
            Stock = 5
        });
        await database.Context.SaveChangesAsync();

        await using var readContext = database.CreateContext();
        var product = await readContext.Products.SingleAsync(product => product.Id == productId);

        Assert.Equal("Integration product", product.Name);
        Assert.Equal(12.34m, product.Price);
        Assert.Equal(5, product.Stock);
    }

    [Fact]
    public async Task Database_rejects_non_positive_price()
    {
        await using var database = await fixture.CreateDatabaseAsync();
        database.Context.Products.Add(new Product { Id = Guid.NewGuid(), Name = "Invalid price", Price = 0, Stock = 1 });

        var exception = await Assert.ThrowsAsync<DbUpdateException>(() => database.Context.SaveChangesAsync());

        Assert.Contains("CK_Products_Price_Positive", GetExceptionMessages(exception));
    }

    [Fact]
    public async Task Database_rejects_negative_stock()
    {
        await using var database = await fixture.CreateDatabaseAsync();
        database.Context.Products.Add(new Product { Id = Guid.NewGuid(), Name = "Invalid stock", Price = 1, Stock = -1 });

        var exception = await Assert.ThrowsAsync<DbUpdateException>(() => database.Context.SaveChangesAsync());

        Assert.Contains("CK_Products_Stock_NonNegative", GetExceptionMessages(exception));
    }

    private static string GetExceptionMessages(Exception exception)
    {
        var messages = new List<string>();
        for (var current = exception; current is not null; current = current.InnerException)
        {
            messages.Add(current.Message);
        }

        return string.Join(Environment.NewLine, messages);
    }
}
