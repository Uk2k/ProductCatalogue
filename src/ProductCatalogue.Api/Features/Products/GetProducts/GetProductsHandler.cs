using MediatR;
using Microsoft.EntityFrameworkCore;
using ProductCatalogue.Api.Data;

namespace ProductCatalogue.Api.Features.Products;

public sealed class GetProductsHandler(AppDbContext db) : IRequestHandler<GetProductsQuery, IReadOnlyList<ProductResponse>>
{
    public async Task<IReadOnlyList<ProductResponse>> Handle(GetProductsQuery request, CancellationToken cancellationToken) =>
        await db.Products.AsNoTracking().OrderBy(x => x.Name).ThenBy(x => x.Id)
            .Select(x => new ProductResponse(x.Id, x.Name, x.Price, x.Stock)).ToListAsync(cancellationToken);
}
