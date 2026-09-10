using MediatR;
using ProductCatalogue.Api.Data;
using ProductCatalogue.Api.Features.Products;

namespace ProductCatalogue.Api.Features.Products.UpdateProduct;

public sealed class UpdateProductHandler(IProductRepository repository) : IRequestHandler<UpdateProductCommand, ProductResponse?>
{
    public async Task<ProductResponse?> Handle(UpdateProductCommand request, CancellationToken cancellationToken)
    {
        var product = await repository.GetByIdAsync(request.Id, cancellationToken);
        if (product is null) return null;
        product.Name = request.Name;
        product.Price = request.Price;
        product.Stock = request.Stock;
        await repository.SaveChangesAsync(cancellationToken);
        return new ProductResponse(product.Id, product.Name, product.Price, product.Stock);
    }
}
