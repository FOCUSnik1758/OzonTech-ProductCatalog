using ProductCatalog.Api.Contracts.Products;
using ProductCatalog.Api.Validation;

namespace ProductCatalog.Tests.Validation;

public sealed class CreateProductRequestValidatorTests
{
    private readonly CreateProductRequestValidator _validator = new();

    [Fact]
    public async Task ValidateAsync_ValidRequest_ReturnsValidResult()
    {
        var request = new CreateProductRequest
        {
            Name = "Монитор",
            Description = "27-дюймовый монитор",
            Price = 25000,
            CategoryId = 1
        };

        var result = await _validator.ValidateAsync(request);

        Assert.True(result.IsValid);
    }

    [Fact]
    public async Task ValidateAsync_EmptyName_ReturnsError()
    {
        var request = new CreateProductRequest
        {
            Name = "",
            Description = "Описание",
            Price = 1000,
            CategoryId = 1
        };

        var result = await _validator.ValidateAsync(request);

        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, error => error.PropertyName == nameof(request.Name));
    }

    [Fact]
    public async Task ValidateAsync_NonPositivePrice_ReturnsError()
    {
        var request = new CreateProductRequest
        {
            Name = "Товар",
            Description = "Описание",
            Price = 0,
            CategoryId = 1
        };

        var result = await _validator.ValidateAsync(request);

        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, error => error.PropertyName == nameof(request.Price));
    }
}
