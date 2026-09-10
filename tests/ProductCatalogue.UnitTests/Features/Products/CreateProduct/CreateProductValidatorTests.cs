using ProductCatalogue.Api.Features.Products;

namespace ProductCatalogue.UnitTests.Features.Products;

public sealed class CreateProductValidatorTests
{
    [Fact] public async Task Rejects_invalid_values() => Assert.False((await Validate(new(" ", 0, -1))).IsValid);
    [Fact] public async Task Accepts_valid_values() => Assert.True((await Validate(new("Keyboard", 49.99m, 10))).IsValid);
    [Fact] public async Task Rejects_name_over_200_characters() => Assert.False((await Validate(new(new string('x', 201), 1, 0))).IsValid);

    private static Task<FluentValidation.Results.ValidationResult> Validate(CreateProductCommand command) =>
        new CreateProductValidator().ValidateAsync(new FluentValidation.ValidationContext<CreateProductCommand>(command));
}
