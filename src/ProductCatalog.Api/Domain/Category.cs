namespace ProductCatalog.Api.Domain;

public sealed class Category
{
    public long Id { get; init; }
    public string Name { get; init; } = string.Empty;
}
