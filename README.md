# ProductCatalog — сервис товаров маркетплейса

Учебный REST API на ASP.NET Core 8:

- PostgreSQL;
- Dapper и SQL-запросы без Entity Framework;
- FluentMigrator;
- FluentValidation;
- поиск, фильтрация, сортировка и пагинация;
- Swagger;
- unit- и integration-тесты.

## Запуск базы данных

```powershell
docker compose up -d
```

Основная БД: порт `5432`.
Тестовая БД: порт `5433`.

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

```powershell
docker compose up -d
```

потом:

```powershell
dotnet test
```

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


