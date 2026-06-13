using ProductCatalog.Api.Common;
using ProductCatalog.Api.Contracts.Products;
using ProductCatalog.Api.Domain;

namespace ProductCatalog.Api.Repositories;

public interface IProductRepository
{
    Task<PagedResult<Product>> SearchAsync(ProductQuery query, CancellationToken cancellationToken);
    Task<Product?> GetByIdAsync(long id, CancellationToken cancellationToken);
    Task<long> CreateAsync(CreateProductRequest request, CancellationToken cancellationToken);
    Task<bool> UpdateAsync(long id, UpdateProductRequest request, CancellationToken cancellationToken);
    Task<bool> DeleteAsync(long id, CancellationToken cancellationToken);
}
