using FluentValidation;
using ProductCatalog.Api.Contracts.Products;

namespace ProductCatalog.Api.Validation;

public sealed class UpdateProductRequestValidator : AbstractValidator<UpdateProductRequest>
{
    public UpdateProductRequestValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Название товара обязательно.")
            .MaximumLength(200).WithMessage("Название не должно превышать 200 символов.");

        RuleFor(x => x.Description)
            .NotNull().WithMessage("Описание не должно быть null.")
            .MaximumLength(4000).WithMessage("Описание не должно превышать 4000 символов.");

        RuleFor(x => x.Price)
            .GreaterThan(0).WithMessage("Цена должна быть больше нуля.")
            .LessThanOrEqualTo(100_000_000).WithMessage("Цена слишком большая.");

        RuleFor(x => x.CategoryId)
            .GreaterThan(0).WithMessage("Необходимо выбрать категорию.");
    }
}
