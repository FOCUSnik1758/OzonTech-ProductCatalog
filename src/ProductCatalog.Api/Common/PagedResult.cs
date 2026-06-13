namespace ProductCatalog.Api.Common;

public sealed class PagedResult<T>
{
    public required IReadOnlyCollection<T> Items { get; init; }
    public required long TotalCount { get; init; }
    public required int Page { get; init; }
    public required int PageSize { get; init; }

    public int TotalPages => TotalCount == 0
        ? 0
        : (int)Math.Ceiling(TotalCount / (double)PageSize);
}
