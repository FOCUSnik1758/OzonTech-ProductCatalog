using ProductCatalog.Api.Contracts.Categories;
using ProductCatalog.Api.Domain;

namespace ProductCatalog.Api.Services;

public interface ICategoryService
{
    Task<IReadOnlyCollection<Category>> GetAllAsync(CancellationToken cancellationToken);
    Task<Category> CreateAsync(CreateCategoryRequest request, CancellationToken cancellationToken);
}
