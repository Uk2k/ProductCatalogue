using System.Net;
using System.Net.Http.Json;
using Microsoft.EntityFrameworkCore;
using ProductCatalogue.IntegrationTests.TestFramework;

namespace ProductCatalogue.IntegrationTests.Features.Products;

[Collection(IntegrationTestCollection.Name)]
public sealed class ProductCrudTests(SqlServerContainerFixture fixture)
{
    [Fact]
    public async Task Put_updates_existing_product()
    {
        await using var database = await fixture.CreateDatabaseAsync();
        using var client = new ProductCatalogueWebApplicationFactory(database).CreateClient();
        var created = await (await client.PostAsJsonAsync("/products", new { name = "Old", price = 1, stock = 1 })).Content.ReadFromJsonAsync<ProductDto>();
        var response = await client.PutAsJsonAsync($"/products/{created!.Id}", new { name = "New", price = 2, stock = 3 });
        var updated = await response.Content.ReadFromJsonAsync<ProductDto>();
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.Equal(new ProductDto(created.Id, "New", 2, 3), updated);
    }

    [Fact]
    public async Task Put_missing_product_returns_not_found()
    {
        await using var database = await fixture.CreateDatabaseAsync();
        using var client = new ProductCatalogueWebApplicationFactory(database).CreateClient();
        var response = await client.PutAsJsonAsync($"/products/{Guid.NewGuid()}", new { name = "New", price = 2, stock = 3 });
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task Delete_removes_existing_product()
    {
        await using var database = await fixture.CreateDatabaseAsync();
        using var client = new ProductCatalogueWebApplicationFactory(database).CreateClient();
        var created = await (await client.PostAsJsonAsync("/products", new { name = "Delete me", price = 1, stock = 1 })).Content.ReadFromJsonAsync<ProductDto>();
        var response = await client.DeleteAsync($"/products/{created!.Id}");
        Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);
        Assert.Equal(0, await database.Context.Products.CountAsync(x => x.Id == created.Id));
    }

    [Fact]
    public async Task Delete_missing_product_returns_not_found()
    {
        await using var database = await fixture.CreateDatabaseAsync();
        using var client = new ProductCatalogueWebApplicationFactory(database).CreateClient();
        Assert.Equal(HttpStatusCode.NotFound, (await client.DeleteAsync($"/products/{Guid.NewGuid()}")).StatusCode);
    }

    private sealed record ProductDto(Guid Id, string Name, decimal Price, int Stock);
}
