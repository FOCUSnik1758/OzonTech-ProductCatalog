using Dapper;
using Npgsql;
using ProductCatalog.Api.Domain;

namespace ProductCatalog.Api.Repositories;

public sealed class CategoryRepository(NpgsqlDataSource dataSource) : ICategoryRepository
{
    public async Task<IReadOnlyCollection<Category>> GetAllAsync(
        CancellationToken cancellationToken)
    {
        const string sql = """
            SELECT id AS Id, name AS Name
            FROM categories
            ORDER BY name;
            """;

        await using var connection = await dataSource.OpenConnectionAsync(cancellationToken);

        return (await connection.QueryAsync<Category>(
            new CommandDefinition(sql, cancellationToken: cancellationToken))).AsList();
    }

    public async Task<Category?> GetByIdAsync(long id, CancellationToken cancellationToken)
    {
        const string sql = """
            SELECT id AS Id, name AS Name
            FROM categories
            WHERE id = @Id;
            """;

        await using var connection = await dataSource.OpenConnectionAsync(cancellationToken);

        return await connection.QuerySingleOrDefaultAsync<Category>(
            new CommandDefinition(sql, new { Id = id }, cancellationToken: cancellationToken));
    }

    public async Task<long> CreateAsync(string name, CancellationToken cancellationToken)
    {
        const string sql = """
            INSERT INTO categories (name)
            VALUES (@Name)
            RETURNING id;
            """;

        await using var connection = await dataSource.OpenConnectionAsync(cancellationToken);

        return await connection.ExecuteScalarAsync<long>(
            new CommandDefinition(sql, new { Name = name }, cancellationToken: cancellationToken));
    }
}
