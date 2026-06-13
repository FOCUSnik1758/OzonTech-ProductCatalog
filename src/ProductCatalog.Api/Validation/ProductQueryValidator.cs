using FluentValidation;
using ProductCatalog.Api.Contracts.Products;

namespace ProductCatalog.Api.Validation;

public sealed class ProductQueryValidator : AbstractValidator<ProductQuery>
{
    private static readonly string[] SortFields = ["name", "price", "createdAt"];
    private static readonly string[] SortDirections = ["asc", "desc"];

    public ProductQueryValidator()
    {
        RuleFor(x => x.Page)
            .GreaterThanOrEqualTo(1).WithMessage("Номер страницы должен быть не меньше 1.");

        RuleFor(x => x.PageSize)
            .InclusiveBetween(1, 100).WithMessage("Размер страницы должен быть от 1 до 100.");

        RuleFor(x => x.CategoryId)
            .GreaterThan(0)
            .When(x => x.CategoryId.HasValue)
            .WithMessage("Идентификатор категории должен быть больше 0.");

        RuleFor(x => x.MinPrice)
            .GreaterThanOrEqualTo(0)
            .When(x => x.MinPrice.HasValue)
            .WithMessage("Минимальная цена не может быть отрицательной.");

        RuleFor(x => x.MaxPrice)
            .GreaterThanOrEqualTo(0)
            .When(x => x.MaxPrice.HasValue)
            .WithMessage("Максимальная цена не может быть отрицательной.");

        RuleFor(x => x)
            .Must(x => !x.MinPrice.HasValue ||
                       !x.MaxPrice.HasValue ||
                       x.MinPrice <= x.MaxPrice)
            .WithMessage("Минимальная цена не может быть больше максимальной.");

        RuleFor(x => x.SortBy)
            .Must(value => SortFields.Contains(value, StringComparer.OrdinalIgnoreCase))
            .WithMessage("sortBy может принимать значения: name, price, createdAt.");

        RuleFor(x => x.SortDirection)
            .Must(value => SortDirections.Contains(value, StringComparer.OrdinalIgnoreCase))
            .WithMessage("sortDirection может принимать значения: asc, desc.");
    }
}
