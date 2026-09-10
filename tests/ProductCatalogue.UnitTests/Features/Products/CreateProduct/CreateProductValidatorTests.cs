using ProductCatalogue.Api.Features.Products;
using ProductCatalogue.Api.Features.Products.CreateProduct;

namespace ProductCatalogue.UnitTests.Features.Products;

public sealed class CreateProductValidatorTests
{
    [Fact] public async Task Rejects_invalid_values() => Assert.False((await Validate(new(" ", 0, -1))).IsValid);
    [Fact] public async Task Accepts_valid_values() => Assert.True((await Validate(new("Keyboard", 49.99m, 10))).IsValid);
    [Fact] public async Task Rejects_name_over_200_characters() => Assert.False((await Validate(new(new string('x', 201), 1, 0))).IsValid);
    [Fact] public async Task Rejects_non_positive_price() => Assert.Contains("Price", (await Validate(new("Valid", 0, 0))).Errors.Select(x => x.PropertyName));
    [Fact] public async Task Rejects_negative_stock() => Assert.Contains("Stock", (await Validate(new("Valid", 1, -1))).Errors.Select(x => x.PropertyName));
    [Fact] public async Task Rejects_blank_name() => Assert.Contains("Name", (await Validate(new(" ", 1, 0))).Errors.Select(x => x.PropertyName));

    private static Task<FluentValidation.Results.ValidationResult> Validate(CreateProductCommand command) =>
        new CreateProductValidator().ValidateAsync(new FluentValidation.ValidationContext<CreateProductCommand>(command));
}
