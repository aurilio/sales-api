using FluentValidation;

namespace Ambev.DeveloperEvaluation.Application.Sales.UpdateSale;

public class UpdateSaleCommandValidator : AbstractValidator<UpdateSaleCommand>
{
    public UpdateSaleCommandValidator()
    {
        RuleFor(x => x.Id)
            .NotEmpty()
            .WithMessage("Sale ID is required.")
            .NotEqual(Guid.Empty)
            .WithMessage("Sale ID must be a valid GUID.");

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
            .ForEach(items => items.SetValidator(new UpdateSaleItemCommandValidator()))
            .When(x => x.Items != null);

        RuleFor(x => x.IsCancelled)
            .NotNull()
            .WithMessage("IsCancelled must be specified.")
            .When(x => x.IsCancelled.HasValue);
    }
}