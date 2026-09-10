using MediatR;

namespace ProductCatalogue.Api.Features.Products.GetProduct;

public static class GetProductEndpoint
{
    public static IEndpointRouteBuilder MapGetProductEndpoint(this IEndpointRouteBuilder endpoints)
    {
        endpoints.MapGet("/products/{id:guid}", async (Guid id, ISender sender, CancellationToken ct) =>
            await sender.Send(new GetProductByIdQuery(id), ct) is { } product
                ? Results.Ok(product)
                : Results.Problem(statusCode: StatusCodes.Status404NotFound, title: "Product not found"));
        return endpoints;
    }
}
