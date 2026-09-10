using ProductCatalogue.Api.Features.Products;

namespace ProductCatalogue.Api.Data;

public interface IProductRepository
{
    Task<Product?> GetByIdAsync(Guid id, CancellationToken cancellationToken);
    Task<IReadOnlyList<Product>> ListAsync(CancellationToken cancellationToken);
    void Add(Product product);
    void Remove(Product product);
    Task SaveChangesAsync(CancellationToken cancellationToken);
}
