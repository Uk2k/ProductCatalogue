using MediatR;
using ProductCatalogue.Api.Features.Products;

namespace ProductCatalogue.Api.Features.Products.UpdateProduct;

public sealed record UpdateProductCommand(Guid Id, string Name, decimal Price, int Stock) : IRequest<ProductResponse?>;
