using ProductCatalogue.Api.Features.Products.UpdateProduct;

namespace ProductCatalogue.UnitTests.Features.Products.UpdateProduct;

public sealed class UpdateProductValidatorTests
{
    [Fact]
    public async Task Rejects_invalid_values() => Assert.False((await new UpdateProductValidator().ValidateAsync(new FluentValidation.ValidationContext<UpdateProductCommand>(new(Guid.NewGuid(), "", 0, -1)))).IsValid);
}
