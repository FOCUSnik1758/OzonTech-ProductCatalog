using ProductCatalog.Api.Common;
using ProductCatalog.Api.Contracts.Products;
using ProductCatalog.Api.Domain;

namespace ProductCatalog.Api.Services;

public interface IProductService
{
    Task<PagedResult<Product>> SearchAsync(ProductQuery query, CancellationToken cancellationToken);
    Task<Product> GetByIdAsync(long id, CancellationToken cancellationToken);
    Task<Product> CreateAsync(CreateProductRequest request, CancellationToken cancellationToken);
    Task<Product> UpdateAsync(long id, UpdateProductRequest request, CancellationToken cancellationToken);
    Task DeleteAsync(long id, CancellationToken cancellationToken);
}
