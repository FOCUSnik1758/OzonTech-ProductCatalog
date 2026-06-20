using Dapper;
using Npgsql;
using ProductCatalog.Api.Common;
using ProductCatalog.Api.Contracts.Products;
using ProductCatalog.Api.Domain;

namespace ProductCatalog.Api.Repositories;

public sealed class ProductRepository(NpgsqlDataSource dataSource) : IProductRepository
{
    public async Task<PagedResult<Product>> SearchAsync(
        ProductQuery query,
        CancellationToken cancellationToken)
    {
        var orderBy = GetOrderBy(query.SortBy);
        var direction = query.SortDirection.Equals("desc", StringComparison.OrdinalIgnoreCase)
            ? "DESC"
            : "ASC";

        var sql = $"""
            SELECT COUNT(*)
            FROM products p
            WHERE (@Search IS NULL
                   OR p.name ILIKE '%' || @Search || '%'
                   OR p.description ILIKE '%' || @Search || '%')
              AND (@CategoryId IS NULL OR p.category_id = @CategoryId)
              AND (@MinPrice IS NULL OR p.price >= @MinPrice)
              AND (@MaxPrice IS NULL OR p.price <= @MaxPrice);

            SELECT
                p.id AS Id,
                p.name AS Name,
                p.description AS Description,
                p.price AS Price,
                p.category_id AS CategoryId,
                c.name AS CategoryName,
                p.created_at AS CreatedAt,
                p.updated_at AS UpdatedAt
            FROM products p
            INNER JOIN categories c ON c.id = p.category_id
            WHERE (@Search IS NULL
                   OR p.name ILIKE '%' || @Search || '%'
                   OR p.description ILIKE '%' || @Search || '%')
              AND (@CategoryId IS NULL OR p.category_id = @CategoryId)
              AND (@MinPrice IS NULL OR p.price >= @MinPrice)
              AND (@MaxPrice IS NULL OR p.price <= @MaxPrice)
            ORDER BY {orderBy} {direction}, p.id ASC
            LIMIT @PageSize OFFSET @Offset;
            """;

        var parameters = new
        {
            Search = string.IsNullOrWhiteSpace(query.Search) ? null : query.Search.Trim(),
            query.CategoryId,
            query.MinPrice,
            query.MaxPrice,
            query.PageSize,
            Offset = (query.Page - 1) * query.PageSize
        };

        await using var connection = await dataSource.OpenConnectionAsync(cancellationToken);

        using var multiple = await connection.QueryMultipleAsync(
            new CommandDefinition(sql, parameters, cancellationToken: cancellationToken));

        var totalCount = await multiple.ReadSingleAsync<long>();
        var products = (await multiple.ReadAsync<Product>()).AsList();

        return new PagedResult<Product>
        {
            Items = products,
            TotalCount = totalCount,
            Page = query.Page,
            PageSize = query.PageSize
        };
    }

    public async Task<Product?> GetByIdAsync(int id, CancellationToken cancellationToken)
    {
        const string sql = """
            SELECT
                p.id AS Id,
                p.name AS Name,
                p.description AS Description,
                p.price AS Price,
                p.category_id AS CategoryId,
                c.name AS CategoryName,
                p.created_at AS CreatedAt,
                p.updated_at AS UpdatedAt
            FROM products p
            INNER JOIN categories c ON c.id = p.category_id
            WHERE p.id = @Id;
            """;

        await using var connection = await dataSource.OpenConnectionAsync(cancellationToken);

        return await connection.QuerySingleOrDefaultAsync<Product>(
            new CommandDefinition(sql, new { Id = id }, cancellationToken: cancellationToken));
    }

    public async Task<int> CreateAsync(
        CreateProductRequest request,
        CancellationToken cancellationToken)
    {
        const string sql = """
            INSERT INTO products (name, description, price, category_id)
            VALUES (@Name, @Description, @Price, @CategoryId)
            RETURNING id;
            """;

        await using var connection = await dataSource.OpenConnectionAsync(cancellationToken);

        return await connection.ExecuteScalarAsync<int>(
            new CommandDefinition(sql, request, cancellationToken: cancellationToken));
    }

    public async Task<bool> UpdateAsync(
        int id,
        UpdateProductRequest request,
        CancellationToken cancellationToken)
    {
        const string sql = """
            UPDATE products
            SET name = @Name,
                description = @Description,
                price = @Price,
                category_id = @CategoryId,
                updated_at = CURRENT_TIMESTAMP
            WHERE id = @Id;
            """;

        var parameters = new
        {
            Id = id,
            request.Name,
            request.Description,
            request.Price,
            request.CategoryId
        };

        await using var connection = await dataSource.OpenConnectionAsync(cancellationToken);

        var affectedRows = await connection.ExecuteAsync(
            new CommandDefinition(sql, parameters, cancellationToken: cancellationToken));

        return affectedRows > 0;
    }

    public async Task<bool> DeleteAsync(int id, CancellationToken cancellationToken)
    {
        const string sql = "DELETE FROM products WHERE id = @Id;";

        await using var connection = await dataSource.OpenConnectionAsync(cancellationToken);

        var affectedRows = await connection.ExecuteAsync(
            new CommandDefinition(sql, new { Id = id }, cancellationToken: cancellationToken));

        return affectedRows > 0;
    }

    private static string GetOrderBy(string sortBy)
    {
        return sortBy.ToLowerInvariant() switch
        {
            "price" => "p.price",
            "createdat" => "p.created_at",
            _ => "p.name"
        };
    }
}
