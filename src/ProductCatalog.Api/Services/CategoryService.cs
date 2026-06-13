using FluentValidation;
using ProductCatalog.Api.Contracts.Categories;
using ProductCatalog.Api.Domain;
using ProductCatalog.Api.Repositories;

namespace ProductCatalog.Api.Services;

public sealed class CategoryService(
    ICategoryRepository categoryRepository,
    IValidator<CreateCategoryRequest> validator) : ICategoryService
{
    public Task<IReadOnlyCollection<Category>> GetAllAsync(
        CancellationToken cancellationToken)
    {
        return categoryRepository.GetAllAsync(cancellationToken);
    }

    public async Task<Category> CreateAsync(
        CreateCategoryRequest request,
        CancellationToken cancellationToken)
    {
        await validator.ValidateAndThrowAsync(request, cancellationToken);

        var normalizedName = request.Name.Trim();
        var id = await categoryRepository.CreateAsync(normalizedName, cancellationToken);

        return new Category { Id = id, Name = normalizedName };
    }
}
