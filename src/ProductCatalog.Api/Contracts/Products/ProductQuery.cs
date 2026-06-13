namespace ProductCatalog.Api.Contracts.Products;

public sealed class ProductQuery
{
    public string? Search { get; set; }
    public long? CategoryId { get; set; }
    public decimal? MinPrice { get; set; }
    public decimal? MaxPrice { get; set; }
    public string SortBy { get; set; } = "name";
    public string SortDirection { get; set; } = "asc";
    public int Page { get; set; } = 1;
    public int PageSize { get; set; } = 20;
}
