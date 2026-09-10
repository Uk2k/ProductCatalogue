using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;

namespace ProductCatalogue.IntegrationTests.TestFramework;

public sealed class ProductCatalogueWebApplicationFactory(
    SqlServerContainerFixture.DatabaseScope database) : WebApplicationFactory<Program>
{
    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.UseSetting("ConnectionStrings:ProductCatalogue", database.ConnectionString);
    }
}
