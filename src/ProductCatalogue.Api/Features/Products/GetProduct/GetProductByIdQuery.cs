using MediatR;

namespace ProductCatalogue.Api.Features.Products;

public sealed record GetProductByIdQuery(Guid Id) : IRequest<ProductResponse?>;
