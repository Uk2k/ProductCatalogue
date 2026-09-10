using System.Net;
using System.Net.Http.Json;
using ProductCatalogue.IntegrationTests.TestFramework;

namespace ProductCatalogue.IntegrationTests;

[Collection(IntegrationTestCollection.Name)]
public sealed class ProductHttpTests(SqlServerContainerFixture fixture)
{
    [Fact]
    public async Task Post_then_get_by_id_returns_created_product()
    {
        await using var database = await fixture.CreateDatabaseAsync();
        using var factory = new ProductCatalogueWebApplicationFactory(database);
        using var client = factory.CreateClient();
        var post = await client.PostAsJsonAsync("/products", new { name = "Keyboard", price = 49.99m, stock = 10 });
        var created = await post.Content.ReadFromJsonAsync<ProductDto>();
        var get = await client.GetFromJsonAsync<ProductDto>($"/products/{created!.Id}");
        Assert.Equal(HttpStatusCode.Created, post.StatusCode);
        Assert.Equal(created, get);
        Assert.Equal($"/products/{created.Id}", post.Headers.Location?.OriginalString);
    }

    [Fact]
    public async Task Get_list_returns_empty_collection_initially()
    {
        await using var database = await fixture.CreateDatabaseAsync();
        using var factory = new ProductCatalogueWebApplicationFactory(database);
        var products = await factory.CreateClient().GetFromJsonAsync<ProductDto[]>("/products");
        Assert.Empty(products!);
    }

    [Fact]
    public async Task Get_unknown_id_returns_not_found()
    {
        await using var database = await fixture.CreateDatabaseAsync();
        using var factory = new ProductCatalogueWebApplicationFactory(database);
        var response = await factory.CreateClient().GetAsync($"/products/{Guid.NewGuid()}");
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        Assert.Equal("application/problem+json", response.Content.Headers.ContentType?.MediaType);
    }

    [Fact]
    public async Task Post_invalid_product_returns_bad_request()
    {
        await using var database = await fixture.CreateDatabaseAsync();
        using var factory = new ProductCatalogueWebApplicationFactory(database);
        var response = await factory.CreateClient().PostAsJsonAsync("/products", new { name = "", price = 0, stock = -1 });
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        Assert.Equal(0, await database.Context.Products.CountAsync());
    }

    [Fact]
    public async Task Put_unknown_id_returns_problem_details_not_found()
    {
        await using var database = await fixture.CreateDatabaseAsync();
        using var factory = new ProductCatalogueWebApplicationFactory(database);
        var response = await factory.CreateClient().PutAsJsonAsync($"/products/{Guid.NewGuid()}", new { name = "Keyboard", price = 49.99m, stock = 10 });
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        Assert.Equal("application/problem+json", response.Content.Headers.ContentType?.MediaType);
    }

    [Fact]
    public async Task Delete_unknown_id_returns_problem_details_not_found()
    {
        await using var database = await fixture.CreateDatabaseAsync();
        using var factory = new ProductCatalogueWebApplicationFactory(database);
        var response = await factory.CreateClient().DeleteAsync($"/products/{Guid.NewGuid()}");
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        Assert.Equal("application/problem+json", response.Content.Headers.ContentType?.MediaType);
    }

    private sealed record ProductDto(Guid Id, string Name, decimal Price, int Stock);
}
