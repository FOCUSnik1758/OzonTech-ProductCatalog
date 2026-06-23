namespace ProductCatalog.Api.Contracts.Storage;

public sealed class ProductCreatedStorageRequest
{
    public int ProductId { get; init; }

    public string ProductName { get; init; } = string.Empty;

    public decimal Price { get; init; }

    public long CategoryId { get; init; }
}