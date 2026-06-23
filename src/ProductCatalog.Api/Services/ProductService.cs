using FluentValidation;
using ProductCatalog.Api.Clients;
using ProductCatalog.Api.Common;
using ProductCatalog.Api.Contracts.Products;
using ProductCatalog.Api.Domain;
using ProductCatalog.Api.Repositories;

namespace ProductCatalog.Api.Services;

public sealed class ProductService(
    IProductRepository productRepository,
    ICategoryRepository categoryRepository,
    IStorageClient storageClient,
    IValidator<CreateProductRequest> createValidator,
    IValidator<UpdateProductRequest> updateValidator,
    IValidator<ProductQuery> queryValidator) : IProductService
{
    public async Task<PagedResult<Product>> SearchAsync(
        ProductQuery query,
        CancellationToken cancellationToken)
    {
        await queryValidator.ValidateAndThrowAsync(
            query,
            cancellationToken);

        return await productRepository.SearchAsync(
            query,
            cancellationToken);
    }

    public async Task<Product> GetByIdAsync(
        int id,
        CancellationToken cancellationToken)
    {
        return await productRepository.GetByIdAsync(
            id,
            cancellationToken)
            ?? throw new NotFoundException(
                $"Товар с идентификатором {id} не найден.");
    }

    public async Task<Product> CreateAsync(
        CreateProductRequest request,
        CancellationToken cancellationToken)
    {
        await createValidator.ValidateAndThrowAsync(
            request,
            cancellationToken);

        await EnsureCategoryExistsAsync(
            request.CategoryId,
            cancellationToken);

        var id = await productRepository.CreateAsync(
            request,
            cancellationToken);

        var product = await GetByIdAsync(
            id,
            cancellationToken);

        try
        {
            await storageClient.NotifyProductCreatedAsync(
                product,
                cancellationToken);
        }
        catch
        {
            // Если склад не создал запись остатка,
            // удаляем только что созданный товар.
            await productRepository.DeleteAsync(
                product.Id,
                CancellationToken.None);

            throw;
        }

        return product;
    }

    public async Task<Product> UpdateAsync(
        int id,
        UpdateProductRequest request,
        CancellationToken cancellationToken)
    {
        await updateValidator.ValidateAndThrowAsync(
            request,
            cancellationToken);

        await EnsureCategoryExistsAsync(
            request.CategoryId,
            cancellationToken);

        var updated = await productRepository.UpdateAsync(
            id,
            request,
            cancellationToken);

        if (!updated)
        {
            throw new NotFoundException(
                $"Товар с идентификатором {id} не найден.");
        }

        return await GetByIdAsync(
            id,
            cancellationToken);
    }

    public async Task DeleteAsync(
        int id,
        CancellationToken cancellationToken)
    {
        var deleted = await productRepository.DeleteAsync(
            id,
            cancellationToken);

        if (!deleted)
        {
            throw new NotFoundException(
                $"Товар с идентификатором {id} не найден.");
        }
    }

    private async Task EnsureCategoryExistsAsync(
        long categoryId,
        CancellationToken cancellationToken)
    {
        if (await categoryRepository.GetByIdAsync(
                categoryId,
                cancellationToken) is null)
        {
            throw new NotFoundException(
                $"Категория с идентификатором {categoryId} не найдена.");
        }
    }
}