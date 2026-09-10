using MediatR;

namespace ProductCatalogue.Api.Features.Products.GetProducts;

public static class GetProductsEndpoint
{
    public static IEndpointRouteBuilder MapGetProductsEndpoint(this IEndpointRouteBuilder endpoints)
    {
        endpoints.MapGet("/products", async (ISender sender, CancellationToken ct) => Results.Ok(await sender.Send(new GetProductsQuery(), ct)));
        return endpoints;
    }
}
