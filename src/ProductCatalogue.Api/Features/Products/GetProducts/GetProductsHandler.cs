using MediatR;
using ProductCatalogue.Api.Data;

namespace ProductCatalogue.Api.Features.Products.GetProducts;

using ProductCatalogue.Api.Features.Products;

public sealed class GetProductsHandler(IProductRepository repository) : IRequestHandler<GetProductsQuery, IReadOnlyList<ProductResponse>>
{
    public async Task<IReadOnlyList<ProductResponse>> Handle(GetProductsQuery request, CancellationToken cancellationToken) =>
        (await repository.ListAsync(cancellationToken))
            .Select(x => new ProductResponse(x.Id, x.Name, x.Price, x.Stock)).ToList();
}
