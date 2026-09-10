namespace ProductCatalogue.Api.Features.Products;

public sealed record ProductResponse(Guid Id, string Name, decimal Price, int Stock);
