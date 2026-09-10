using MediatR;

namespace ProductCatalogue.Api.Features.Products;

public sealed record CreateProductCommand(string Name, decimal Price, int Stock) : IRequest<ProductResponse>;
