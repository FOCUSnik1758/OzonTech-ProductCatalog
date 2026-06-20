using ProductCatalog.Api.Common;
using ProductCatalog.Api.Contracts.Products;
using ProductCatalog.Api.Domain;

namespace ProductCatalog.Api.Services;

public interface IProductService
{
    Task<PagedResult<Product>> SearchAsync(ProductQuery query, CancellationToken cancellationToken);
    Task<Product> GetByIdAsync(int id, CancellationToken cancellationToken);
    Task<Product> CreateAsync(CreateProductRequest request, CancellationToken cancellationToken);
    Task<Product> UpdateAsync(int id, UpdateProductRequest request, CancellationToken cancellationToken);
    Task DeleteAsync(int id, CancellationToken cancellationToken);
}
