using ChinaImportPlatform.Api.Dtos;
using FluentValidation;

namespace ChinaImportPlatform.Api.Validators;

public class CreateCategoryValidator : AbstractValidator<CreateCategoryDto>
{
    public CreateCategoryValidator()
    {
        RuleFor(x => x.Name).NotEmpty().MaximumLength(120);
    }
}

public class CreateProductValidator : AbstractValidator<CreateProductDto>
{
    public CreateProductValidator()
    {
        RuleFor(x => x.CategoryId).NotEmpty();
        RuleFor(x => x.Name).NotEmpty().MaximumLength(200);
        RuleFor(x => x.IndicativePriceCents).GreaterThanOrEqualTo(0);
    }
}

public class CreateTripValidator : AbstractValidator<CreateTripDto>
{
    public CreateTripValidator()
    {
        RuleFor(x => x.Name).NotEmpty();
    }
}
