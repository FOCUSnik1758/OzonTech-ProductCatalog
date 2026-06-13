using ProductCatalog.Api.Contracts.Products;
using ProductCatalog.Api.Repositories;

namespace ProductCatalog.Tests.Integration;

public sealed class ProductRepositoryIntegrationTests :
    IClassFixture<PostgreSqlFixture>
{
    private readonly ProductRepository _repository;

    public ProductRepositoryIntegrationTests(PostgreSqlFixture fixture)
    {
        _repository = new ProductRepository(fixture.DataSource);
    }

    [Fact]
    public async Task SearchAsync_SeededProducts_ReturnsProducts()
    {
        var result = await _repository.SearchAsync(
            new ProductQuery { Page = 1, PageSize = 10 },
            CancellationToken.None);

        Assert.True(result.TotalCount >= 4);
        Assert.NotEmpty(result.Items);
    }

    [Fact]
    public async Task SearchAsync_TextSearch_ReturnsMatchingProduct()
    {
        var result = await _repository.SearchAsync(
            new ProductQuery
            {
                Search = "наушники",
                Page = 1,
                PageSize = 10
            },
            CancellationToken.None);

        Assert.Contains(
            result.Items,
            product => product.Name.Contains(
                "наушники",
                StringComparison.OrdinalIgnoreCase));
    }

    [Fact]
    public async Task CreateAndGetByIdAsync_ReturnsCreatedProduct()
    {
        var id = await _repository.CreateAsync(
            new CreateProductRequest
            {
                Name = "Тестовый товар",
                Description = "Товар из integration-теста",
                Price = 999,
                CategoryId = 1
            },
            CancellationToken.None);

        var product = await _repository.GetByIdAsync(id, CancellationToken.None);

        Assert.NotNull(product);
        Assert.Equal("Тестовый товар", product.Name);
        Assert.Equal(999, product.Price);
    }
}
