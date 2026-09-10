using MediatR;

namespace ProductCatalogue.Api.Features.Products.DeleteProduct;

public static class DeleteProductEndpoint
{
    public static IEndpointRouteBuilder MapDeleteProductEndpoint(this IEndpointRouteBuilder endpoints)
    {
        endpoints.MapDelete("/products/{id:guid}", async (Guid id, ISender sender, CancellationToken ct) =>
            await sender.Send(new DeleteProductCommand(id), ct)
                ? Results.NoContent()
                : Results.Problem(statusCode: StatusCodes.Status404NotFound, title: "Product not found"));
        return endpoints;
    }
}
