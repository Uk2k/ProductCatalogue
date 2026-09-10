using FluentValidation;
using MediatR;

namespace ProductCatalogue.Api.Features.Products.CreateProduct;

public static class CreateProductEndpoint
{
    public static IEndpointRouteBuilder MapCreateProductEndpoint(this IEndpointRouteBuilder endpoints)
    {
        endpoints.MapPost("/products", async (CreateProductRequest request, ISender sender, IValidator<CreateProductCommand> validator, CancellationToken ct) =>
        {
            var command = new CreateProductCommand(request.Name, request.Price, request.Stock);
            var result = await validator.ValidateAsync(new FluentValidation.ValidationContext<CreateProductCommand>(command), ct);
            if (!result.IsValid) return Results.ValidationProblem(result.ToDictionary());
            var product = await sender.Send(command, ct);
            return Results.Created($"/products/{product.Id}", product);
        });
        return endpoints;
    }

    public sealed record CreateProductRequest(string Name, decimal Price, int Stock);
}
