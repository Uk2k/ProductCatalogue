using MediatR;
using ProductCatalogue.Api.Data;

namespace ProductCatalogue.Api.Features.Products.CreateProduct;

using ProductCatalogue.Api.Features.Products;

public sealed class CreateProductHandler(AppDbContext db) : IRequestHandler<CreateProductCommand, ProductResponse>
{
    public async Task<ProductResponse> Handle(CreateProductCommand request, CancellationToken cancellationToken)
    {
        var product = new Product { Id = Guid.NewGuid(), Name = request.Name, Price = request.Price, Stock = request.Stock };
        db.Products.Add(product);
        await db.SaveChangesAsync(cancellationToken);
        return new ProductResponse(product.Id, product.Name, product.Price, product.Stock);
    }
}
