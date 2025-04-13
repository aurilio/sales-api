using FluentValidation;

namespace Ambev.DeveloperEvaluation.WebApi.Features.Sales.UpdateSale;

public class UpdateSaleRequestValidator : AbstractValidator<UpdateSaleRequest>
{
    public UpdateSaleRequestValidator()
    {
        RuleFor(x => x.SaleNumber)
            .MaximumLength(50)
            .WithMessage("Sale number cannot exceed 50 characters.")
            .When(x => x.SaleNumber != null);

        RuleFor(x => x.SaleDate)
            .LessThanOrEqualTo(DateTime.Now)
            .WithMessage("Sale date cannot be in the future.")
            .When(x => x.SaleDate.HasValue);

        RuleFor(x => x.CustomerId)
            .MaximumLength(50)
            .WithMessage("Customer ID cannot exceed 50 characters.")
            .When(x => x.CustomerId != null);

        RuleFor(x => x.TotalAmount)
            .GreaterThan(0)
            .WithMessage("Total amount must be greater than zero.")
            .When(x => x.TotalAmount.HasValue);

        RuleFor(x => x.BranchId)
            .MaximumLength(50)
            .WithMessage("Branch ID cannot exceed 50 characters.")
            .When(x => x.BranchId != null);

        RuleFor(x => x.Items)
            .ForEach(items => items.SetValidator(new UpdateSaleItemRequestValidator()))
            .When(x => x.Items != null);

        RuleFor(x => x.IsCancelled)
            .NotNull()
            .WithMessage("IsCancelled must be specified.")
            .When(x => x.IsCancelled.HasValue);
    }
}