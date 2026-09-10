using FluentValidation;
using MediatR;
using System.ComponentModel.DataAnnotations;
using ProductCatalogue.Api.Infrastructure.Validation;

namespace ProductCatalogue.Api.Features.Products.CreateProduct;

public static class CreateProductEndpoint
{
    public static IEndpointRouteBuilder MapCreateProductEndpoint(this IEndpointRouteBuilder endpoints)
    {
        endpoints.MapPost("/products", async (CreateProductRequest request, ISender sender, IValidator<CreateProductCommand> validator, CancellationToken ct) =>
        {
            var command = new CreateProductCommand(request.Name, request.Price, request.Stock);
            var attributeErrors = DataAnnotationsValidator.Validate(request);
            if (attributeErrors.Count > 0) return Results.ValidationProblem(attributeErrors);
            var result = await validator.ValidateAsync(new FluentValidation.ValidationContext<CreateProductCommand>(command), ct);
            if (!result.IsValid) return Results.ValidationProblem(result.ToDictionary());
            var product = await sender.Send(command, ct);
            return Results.Created($"/products/{product.Id}", product);
        });
        return endpoints;
    }

    public sealed record CreateProductRequest(
        [property: Required, MinLength(1), MaxLength(200)] string Name,
        [property: Range(typeof(decimal), "0.01", "79228162514264337593543950335")] decimal Price,
        [property: Range(0, int.MaxValue)] int Stock);
}
