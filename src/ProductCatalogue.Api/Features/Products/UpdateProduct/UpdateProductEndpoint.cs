using FluentValidation;
using MediatR;
using System.ComponentModel.DataAnnotations;
using ProductCatalogue.Api.Infrastructure.Validation;

namespace ProductCatalogue.Api.Features.Products.UpdateProduct;

public static class UpdateProductEndpoint
{
    public static IEndpointRouteBuilder MapUpdateProductEndpoint(this IEndpointRouteBuilder endpoints)
    {
        endpoints.MapPut("/products/{id:guid}", async (Guid id, UpdateProductRequest request, ISender sender, IValidator<UpdateProductCommand> validator, CancellationToken ct) =>
        {
            var command = new UpdateProductCommand(id, request.Name, request.Price, request.Stock);
            var attributeErrors = DataAnnotationsValidator.Validate(request);
            if (attributeErrors.Count > 0) return Results.ValidationProblem(attributeErrors);
            var validation = await validator.ValidateAsync(new FluentValidation.ValidationContext<UpdateProductCommand>(command), ct);
            if (!validation.IsValid) return Results.ValidationProblem(validation.ToDictionary());
            return await sender.Send(command, ct) is { } product
                ? Results.Ok(product)
                : Results.Problem(statusCode: StatusCodes.Status404NotFound, title: "Product not found");
        });
        return endpoints;
    }

    public sealed record UpdateProductRequest(
        [property: Required, MinLength(1), MaxLength(200)] string Name,
        [property: Range(typeof(decimal), "0.01", "79228162514264337593543950335")] decimal Price,
        [property: Range(0, int.MaxValue)] int Stock);
}
