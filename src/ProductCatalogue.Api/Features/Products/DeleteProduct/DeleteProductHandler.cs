using MediatR;
using ProductCatalogue.Api.Data;

namespace ProductCatalogue.Api.Features.Products.DeleteProduct;

public sealed class DeleteProductHandler(IProductRepository repository) : IRequestHandler<DeleteProductCommand, bool>
{
    public async Task<bool> Handle(DeleteProductCommand request, CancellationToken cancellationToken)
    {
        var product = await repository.GetByIdAsync(request.Id, cancellationToken);
        if (product is null) return false;
        repository.Remove(product);
        await repository.SaveChangesAsync(cancellationToken);
        return true;
    }
}
