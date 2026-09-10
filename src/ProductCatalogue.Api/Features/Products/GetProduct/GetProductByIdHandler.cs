using MediatR;
using ProductCatalogue.Api.Data;

namespace ProductCatalogue.Api.Features.Products.GetProduct;

using ProductCatalogue.Api.Features.Products;

public sealed class GetProductByIdHandler(IProductRepository repository) : IRequestHandler<GetProductByIdQuery, ProductResponse?>
{
    public async Task<ProductResponse?> Handle(GetProductByIdQuery request, CancellationToken cancellationToken)
    {
        var product = await repository.GetByIdAsync(request.Id, cancellationToken);
        return product is null ? null : new ProductResponse(product.Id, product.Name, product.Price, product.Stock);
    }
}
