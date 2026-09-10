using ProductCatalogue.Api.Infrastructure.Validation;
using System.ComponentModel.DataAnnotations;

namespace ProductCatalogue.UnitTests.Infrastructure.Validation;

public sealed class DataAnnotationsValidatorTests
{
    [Fact]
    public void Validate_returns_required_and_range_errors()
    {
        var errors = DataAnnotationsValidator.Validate(new Request(null!, 0, -1));

        Assert.Contains("Name", errors.Keys);
        Assert.Contains("Price", errors.Keys);
        Assert.Contains("Stock", errors.Keys);
    }

    [Fact]
    public void Validate_returns_no_errors_for_valid_model()
    {
        var errors = DataAnnotationsValidator.Validate(new Request("Keyboard", 49.99m, 10));

        Assert.Empty(errors);
    }

    private sealed record Request(
        [property: Required, MinLength(1)] string Name,
        [property: Range(typeof(decimal), "0.01", "1000")] decimal Price,
        [property: Range(0, 100)] int Stock);
}
