using MediatR;

namespace ProductCatalogue.Api.Features.Products.GetProducts;

public sealed record GetProductsQuery : IRequest<IReadOnlyList<ProductResponse>>;
