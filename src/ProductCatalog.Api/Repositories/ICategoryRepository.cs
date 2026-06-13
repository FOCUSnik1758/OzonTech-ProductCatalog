using ProductCatalog.Api.Domain;

namespace ProductCatalog.Api.Repositories;

public interface ICategoryRepository
{
    Task<IReadOnlyCollection<Category>> GetAllAsync(CancellationToken cancellationToken);
    Task<Category?> GetByIdAsync(long id, CancellationToken cancellationToken);
    Task<long> CreateAsync(string name, CancellationToken cancellationToken);
}
