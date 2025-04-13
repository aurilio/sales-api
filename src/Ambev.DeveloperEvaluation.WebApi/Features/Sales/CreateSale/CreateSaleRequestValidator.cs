using FluentValidation;

namespace Ambev.DeveloperEvaluation.WebApi.Features.Sales.CreateSale;

public class CreateSaleRequestValidator : AbstractValidator<CreateSaleRequest>
{
    public CreateSaleRequestValidator()
    {
        RuleFor(x => x.SaleNumber)
            .NotEmpty()
            .WithMessage("Sale number is required.")
            .MaximumLength(50)
            .WithMessage("Sale number cannot exceed 50 characters.");

        RuleFor(x => x.SaleDate)
            .NotEmpty()
            .WithMessage("Sale date is required.")
            .LessThanOrEqualTo(DateTime.Now)
            .WithMessage("Sale date cannot be in the future.");

        RuleFor(x => x.CustomerId)
            .NotEmpty()
            .WithMessage("Customer ID is required.")
            .MaximumLength(50)
            .WithMessage("Customer ID cannot exceed 50 characters.");

        RuleFor(x => x.TotalAmount)
            .GreaterThan(0)
            .WithMessage("Total amount must be greater than zero.");

        RuleFor(x => x.BranchId)
            .NotEmpty()
            .WithMessage("Branch ID is required.")
            .MaximumLength(50)
            .WithMessage("Branch ID cannot exceed 50 characters.");

        RuleFor(x => x.Items)
            .NotEmpty()
            .WithMessage("Sale must have at least one item.")
            .ForEach(items => items.SetValidator(new SaleItemRequestValidator()));

        RuleFor(x => x.IsCancelled)
            .NotNull()
            .WithMessage("IsCancelled must be specified.");
    }
}