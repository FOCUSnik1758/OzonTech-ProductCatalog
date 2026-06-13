using FluentValidation;
using ProductCatalog.Api.Contracts.Categories;

namespace ProductCatalog.Api.Validation;

public sealed class CreateCategoryRequestValidator : AbstractValidator<CreateCategoryRequest>
{
    public CreateCategoryRequestValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Название категории обязательно.")
            .MaximumLength(100).WithMessage("Название категории не должно превышать 100 символов.");
    }
}
