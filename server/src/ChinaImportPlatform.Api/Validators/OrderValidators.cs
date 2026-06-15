using ChinaImportPlatform.Api.Dtos;
using FluentValidation;

namespace ChinaImportPlatform.Api.Validators;

public class CreateOrderValidator : AbstractValidator<CreateOrderDto>
{
    public CreateOrderValidator()
    {
        RuleFor(x => x.UserId).NotEmpty();
        RuleFor(x => x.Items).NotEmpty().WithMessage("An order must contain at least one item.");

        RuleForEach(x => x.Items).ChildRules(item =>
        {
            item.RuleFor(i => i.Quantity)
                .InclusiveBetween(1, 999).WithMessage("Item quantity must be between 1 and 999.");

            // Custom (no product) lines need a meaningful description.
            item.RuleFor(i => i.CustomDescription)
                .NotEmpty().MinimumLength(20)
                .When(i => i.ProductId is null)
                .WithMessage("Custom orders require a description of at least 20 characters.");
        });
    }
}

public class RejectLineValidator : AbstractValidator<RejectLineDto>
{
    public RejectLineValidator()
    {
        RuleFor(x => x.OrderItemId).NotEmpty();
        RuleFor(x => x.Reason).NotEmpty().WithMessage("A rejection reason is required.");
    }
}

public class MarkReadyForCollectionValidator : AbstractValidator<MarkReadyForCollectionDto>
{
    public MarkReadyForCollectionValidator()
    {
        RuleFor(x => x.CollectionAddress).NotEmpty().WithMessage("A collection address is required.");
        RuleFor(x => x.WindowEnd)
            .GreaterThan(x => x.WindowStart)
            .WithMessage("The collection window end must be after the start.");
    }
}

public class BulkUpdateOrderStatusValidator : AbstractValidator<BulkUpdateOrderStatusDto>
{
    public BulkUpdateOrderStatusValidator()
    {
        RuleFor(x => x.OrderIds).NotEmpty().WithMessage("Select at least one order.");
    }
}
