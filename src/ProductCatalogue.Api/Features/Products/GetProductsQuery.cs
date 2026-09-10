using MediatR;

namespace ProductCatalogue.Api.Features.Products;

public sealed record GetProductsQuery : IRequest<IReadOnlyList<ProductResponse>>;
