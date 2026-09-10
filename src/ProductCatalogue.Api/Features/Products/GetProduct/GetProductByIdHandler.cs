using MediatR;
using Microsoft.EntityFrameworkCore;
using ProductCatalogue.Api.Data;

namespace ProductCatalogue.Api.Features.Products.GetProduct;

using ProductCatalogue.Api.Features.Products;

public sealed class GetProductByIdHandler(AppDbContext db) : IRequestHandler<GetProductByIdQuery, ProductResponse?>
{
    public Task<ProductResponse?> Handle(GetProductByIdQuery request, CancellationToken cancellationToken) =>
        db.Products.AsNoTracking().Where(x => x.Id == request.Id)
            .Select(x => new ProductResponse(x.Id, x.Name, x.Price, x.Stock)).SingleOrDefaultAsync(cancellationToken);
}
