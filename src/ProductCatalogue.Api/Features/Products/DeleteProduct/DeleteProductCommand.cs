using MediatR;

namespace ProductCatalogue.Api.Features.Products.DeleteProduct;

public sealed record DeleteProductCommand(Guid Id) : IRequest<bool>;
