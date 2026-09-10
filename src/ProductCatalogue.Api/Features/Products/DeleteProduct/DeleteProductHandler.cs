using MediatR;
using ProductCatalogue.Api.Data;

namespace ProductCatalogue.Api.Features.Products.DeleteProduct;

public sealed class DeleteProductHandler(AppDbContext db) : IRequestHandler<DeleteProductCommand, bool>
{
    public async Task<bool> Handle(DeleteProductCommand request, CancellationToken cancellationToken)
    {
        var product = await db.Products.FindAsync([request.Id], cancellationToken);
        if (product is null) return false;
        db.Products.Remove(product);
        await db.SaveChangesAsync(cancellationToken);
        return true;
    }
}
