using FluentValidation;
using MediatR;

namespace ProductCatalogue.Api.Features.Products;

public static class ProductEndpoints
{
    public static IEndpointRouteBuilder MapProductEndpoints(this IEndpointRouteBuilder endpoints)
    {
        var group = endpoints.MapGroup("/products");
        group.MapGet("/{id:guid}", async (Guid id, ISender sender, CancellationToken ct) =>
            await sender.Send(new GetProductByIdQuery(id), ct) is { } product ? Results.Ok(product) : Results.NotFound());
        group.MapGet("", async (ISender sender, CancellationToken ct) => Results.Ok(await sender.Send(new GetProductsQuery(), ct)));
        group.MapPost("", async (CreateProductRequest request, ISender sender, IValidator<CreateProductCommand> validator, HttpContext http, CancellationToken ct) =>
        {
            var command = new CreateProductCommand(request.Name, request.Price, request.Stock);
            var result = await validator.ValidateAsync(command, ct);
            if (!result.IsValid)
                return Results.ValidationProblem(result.ToDictionary());
            var product = await sender.Send(command, ct);
            return Results.Created($"/products/{product.Id}", product);
        });
        return endpoints;
    }

    public sealed record CreateProductRequest(string Name, decimal Price, int Stock);
}
