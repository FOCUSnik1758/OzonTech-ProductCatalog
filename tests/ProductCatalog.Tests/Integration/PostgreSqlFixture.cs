using FluentMigrator.Runner;
using Microsoft.Extensions.DependencyInjection;
using Npgsql;
using ProductCatalog.Api.Infrastructure.Migrations;
using Xunit;

namespace ProductCatalog.Tests.Integration;

public sealed class PostgreSqlFixture : IAsyncLifetime
{
    public const string ConnectionString =
        "Host=localhost;Port=15433;Database=product_catalog_test;Username=postgres;Password=postgres";

    public NpgsqlDataSource DataSource { get; private set; } = null!;

    public async Task InitializeAsync()
    {
        DataSource = NpgsqlDataSource.Create(ConnectionString);

        await using (var connection = await DataSource.OpenConnectionAsync())
        await using (var command = connection.CreateCommand())
        {
            command.CommandText = "DROP SCHEMA IF EXISTS public CASCADE; CREATE SCHEMA public;";
            await command.ExecuteNonQueryAsync();
        }

        using var services = new ServiceCollection()
            .AddFluentMigratorCore()
            .ConfigureRunner(runner => runner
                .AddPostgres()
                .WithGlobalConnectionString(ConnectionString)
                .ScanIn(typeof(InitialSchemaMigration).Assembly).For.Migrations())
            .AddLogging(logging => logging.AddFluentMigratorConsole())
            .BuildServiceProvider();

        using var scope = services.CreateScope();
        scope.ServiceProvider.GetRequiredService<IMigrationRunner>().MigrateUp();
    }

    public async Task DisposeAsync()
    {
        await DataSource.DisposeAsync();
    }
}
