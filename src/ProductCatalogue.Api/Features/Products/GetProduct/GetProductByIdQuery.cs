using MediatR;

namespace ProductCatalogue.Api.Features.Products.GetProduct;

public sealed record GetProductByIdQuery(Guid Id) : IRequest<ProductResponse?>;
