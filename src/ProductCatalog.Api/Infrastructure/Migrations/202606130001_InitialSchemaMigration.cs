using FluentMigrator;

namespace ProductCatalog.Api.Infrastructure.Migrations;

[Migration(202606130001)]
public sealed class InitialSchemaMigration : Migration
{
    public override void Up()
    {
        Execute.Sql("CREATE EXTENSION IF NOT EXISTS pg_trgm;");

        Create.Table("categories")
            .WithColumn("id").AsInt64().PrimaryKey().Identity()
            .WithColumn("name").AsString(100).NotNullable();

        Create.UniqueConstraint("uq_categories_name")
            .OnTable("categories")
            .Column("name");

        Create.Table("products")
            .WithColumn("id").AsInt32().PrimaryKey().Identity()
            .WithColumn("name").AsString(200).NotNullable()
            .WithColumn("description").AsString(4000).NotNullable()
            .WithColumn("price").AsDecimal(18, 2).NotNullable()
            .WithColumn("category_id").AsInt64().NotNullable()
            .WithColumn("created_at").AsDateTime().NotNullable()
                .WithDefault(SystemMethods.CurrentUTCDateTime)
            .WithColumn("updated_at").AsDateTime().NotNullable()
                .WithDefault(SystemMethods.CurrentUTCDateTime);

        Create.ForeignKey("fk_products_categories")
            .FromTable("products").ForeignColumn("category_id")
            .ToTable("categories").PrimaryColumn("id");

        Create.Index("ix_products_category_id")
            .OnTable("products")
            .OnColumn("category_id").Ascending();

        Create.Index("ix_products_price")
            .OnTable("products")
            .OnColumn("price").Ascending();

        Create.Index("ix_products_created_at")
            .OnTable("products")
            .OnColumn("created_at").Descending();

        Execute.Sql("""
            ALTER TABLE products
            ADD CONSTRAINT ck_products_price_positive CHECK (price > 0);
            """);

        Execute.Sql("""
            CREATE INDEX ix_products_name_trgm
            ON products USING gin (name gin_trgm_ops);
            """);

        Execute.Sql("""
            CREATE INDEX ix_products_description_trgm
            ON products USING gin (description gin_trgm_ops);
            """);
    }

    public override void Down()
    {
        Delete.Table("products");
        Delete.Table("categories");
    }
}
