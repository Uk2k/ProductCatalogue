using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using ProductCatalogue.Api.Infrastructure.Observability;

namespace ProductCatalogue.UnitTests.Infrastructure.Observability;

public sealed class ObservabilityRegistrationTests
{
    [Fact]
    public void AddProductCatalogueObservability_WhenSettingIsAbsent_DisablesOpenTelemetry()
    {
        var configuration = new ConfigurationBuilder().Build();
        var services = new ServiceCollection();

        services.AddProductCatalogueObservability(configuration);

        var provider = services.BuildServiceProvider();
        Assert.False(provider.GetRequiredService<ObservabilityOptions>().OpenTelemetryEnabled);
    }

    [Fact]
    public void AddProductCatalogueObservability_WhenSettingIsTrue_EnablesOpenTelemetry()
    {
        var configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string?>
            {
                ["Observability:OpenTelemetryEnabled"] = "true"
            })
            .Build();
        var services = new ServiceCollection();

        services.AddProductCatalogueObservability(configuration);

        var provider = services.BuildServiceProvider();
        Assert.True(provider.GetRequiredService<ObservabilityOptions>().OpenTelemetryEnabled);
    }
}
