using FluentMigrator;

namespace ProductCatalog.Api.Infrastructure.Migrations;

[Migration(202606130002)]
public sealed class SeedDataMigration : Migration
{
    public override void Up()
    {
        Insert.IntoTable("categories")
            .Row(new { name = "Электроника" })
            .Row(new { name = "Одежда" })
            .Row(new { name = "Дом и кухня" });

        Execute.Sql("""
            INSERT INTO products (name, description, price, category_id)
            SELECT 'Смартфон Nova X', 'Смартфон с OLED-экраном и 256 ГБ памяти', 49990, id
            FROM categories WHERE name = 'Электроника';

            INSERT INTO products (name, description, price, category_id)
            SELECT 'Беспроводные наушники AirBeat', 'Наушники с активным шумоподавлением', 12990, id
            FROM categories WHERE name = 'Электроника';

            INSERT INTO products (name, description, price, category_id)
            SELECT 'Хлопковая футболка', 'Базовая футболка унисекс', 1990, id
            FROM categories WHERE name = 'Одежда';

            INSERT INTO products (name, description, price, category_id)
            SELECT 'Электрический чайник', 'Чайник объёмом 1,7 литра', 3490, id
            FROM categories WHERE name = 'Дом и кухня';
            """);
    }

    public override void Down()
    {
        Delete.FromTable("products").AllRows();
        Delete.FromTable("categories").AllRows();
    }
}
