using MediatR;

namespace ProductCatalogue.Api.Features.Products.CreateProduct;

public sealed record CreateProductCommand(string Name, decimal Price, int Stock) : IRequest<ProductResponse>;
