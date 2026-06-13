using ProductCatalog.Api.Contracts.Products;
using ProductCatalog.Api.Validation;

namespace ProductCatalog.Tests.Validation;

public sealed class ProductQueryValidatorTests
{
    private readonly ProductQueryValidator _validator = new();

    [Fact]
    public async Task ValidateAsync_PageSizeMoreThan100_ReturnsError()
    {
        var query = new ProductQuery { PageSize = 101 };

        var result = await _validator.ValidateAsync(query);

        Assert.False(result.IsValid);
    }

    [Fact]
    public async Task ValidateAsync_MinPriceGreaterThanMaxPrice_ReturnsError()
    {
        var query = new ProductQuery
        {
            MinPrice = 5000,
            MaxPrice = 1000
        };

        var result = await _validator.ValidateAsync(query);

        Assert.False(result.IsValid);
    }

    [Fact]
    public async Task ValidateAsync_AllowedSorting_ReturnsValidResult()
    {
        var query = new ProductQuery
        {
            SortBy = "price",
            SortDirection = "desc"
        };

        var result = await _validator.ValidateAsync(query);

        Assert.True(result.IsValid);
    }
}
