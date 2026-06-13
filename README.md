# ProductCatalog — сервис товаров маркетплейса

Учебный REST API на ASP.NET Core 8:

- PostgreSQL;
- Dapper и SQL-запросы без Entity Framework;
- FluentMigrator;
- FluentValidation;
- поиск, фильтрация, сортировка и пагинация;
- Swagger;
- unit- и integration-тесты.

## Установка

1. Установите Visual Studio 2022.
2. В Visual Studio Installer отметьте компонент **ASP.NET and web development**.
3. Установите .NET 8 SDK.
4. Установите Docker Desktop.

## Запуск базы данных

Откройте PowerShell в папке решения:

```powershell
docker compose up -d
```

Основная БД: порт `5432`.
Тестовая БД: порт `5433`.

## Запуск API в Visual Studio

1. Откройте `ProductCatalog.sln`.
2. В Solution Explorer нажмите правой кнопкой по `ProductCatalog.Api`.
3. Выберите **Set as Startup Project**.
4. Нажмите `Ctrl+F5`.
5. Откроется Swagger: `https://localhost:7080/swagger`.

Миграции выполняются автоматически при старте.

## API

Получить каталог:

```http
GET /api/products?page=1&pageSize=10
```

Поиск и фильтрация:

```http
GET /api/products?search=phone&categoryId=1&minPrice=100&maxPrice=100000&sortBy=price&sortDirection=desc&page=1&pageSize=20
```

Допустимые `sortBy`: `name`, `price`, `createdAt`.

Получить товар:

```http
GET /api/products/1
```

Создать товар:

```http
POST /api/products
Content-Type: application/json
```

```json
{
  "name": "Механическая клавиатура",
  "description": "Клавиатура с RGB-подсветкой",
  "price": 7490,
  "categoryId": 1
}
```

Обновить:

```http
PUT /api/products/1
```

Удалить:

```http
DELETE /api/products/1
```

Категории:

```http
GET /api/categories
POST /api/categories
```

## Тесты

Сначала выполните:

```powershell
docker compose up -d
```

Затем:

```powershell
dotnet test
```

Либо в Visual Studio: **Test → Test Explorer → Run All Tests**.

## Архитектура

```text
Frontend
   ↓ HTTP/JSON
ProductsController
   ↓
ProductService
   ↓
ProductRepository
   ↓ SQL через Dapper
PostgreSQL
```

## Что показать на защите

1. FluentMigrator создаёт таблицы, связи, ограничения и индексы.
2. Все запросы к БД написаны на SQL.
3. Dapper передаёт параметры отдельно от SQL, что защищает от SQL-инъекций.
4. Поиск выполняется через `ILIKE`.
5. GIN trigram-индексы ускоряют поиск по названию и описанию.
6. Фильтры выполняются в PostgreSQL.
7. Пагинация реализована через `LIMIT` и `OFFSET`.
8. Поля сортировки выбираются только из белого списка.
9. FluentValidation проверяет входящие запросы.
10. Unit-тесты проверяют валидацию, integration-тесты — работу с PostgreSQL.
