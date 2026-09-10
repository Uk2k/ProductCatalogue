using ProductCatalogue.Api.Features.Products.UpdateProduct;

namespace ProductCatalogue.UnitTests.Features.Products.UpdateProduct;

public sealed class UpdateProductValidatorTests
{
    [Fact]
    public async Task Rejects_invalid_values() => Assert.False((await new UpdateProductValidator().ValidateAsync(new FluentValidation.ValidationContext<UpdateProductCommand>(new(Guid.NewGuid(), "", 0, -1)))).IsValid);

    [Fact]
    public async Task Accepts_valid_values() => Assert.True((await Validate(new(Guid.NewGuid(), "Valid", 1, 0))).IsValid);

    [Fact]
    public async Task Rejects_name_over_200_characters() => Assert.False((await Validate(new(Guid.NewGuid(), new string('x', 201), 1, 0))).IsValid);

    private static Task<FluentValidation.Results.ValidationResult> Validate(UpdateProductCommand values) =>
        new UpdateProductValidator().ValidateAsync(new FluentValidation.ValidationContext<UpdateProductCommand>(values));
}
