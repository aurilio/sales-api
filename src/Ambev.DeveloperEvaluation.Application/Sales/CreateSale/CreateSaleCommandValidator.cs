using FluentValidation;

namespace Ambev.DeveloperEvaluation.Application.Sales.CreateSale;

public class CreateSaleCommandValidator : AbstractValidator<CreateSaleCommand>
{
    public CreateSaleCommandValidator()
    {
        RuleFor(x => x.SaleNumber)
            .NotEmpty()
            .WithMessage("Sale number is required.")
            .MaximumLength(50)
            .WithMessage("Sale number cannot exceed 50 characters.");

        RuleFor(x => x.SaleDate)
            .NotEmpty()
            .WithMessage("Sale date is required.")
            .LessThanOrEqualTo(DateTime.Now.ToLocalTime())
            .WithMessage("Sale date cannot be in the future.");

        RuleFor(x => x.CustomerId)
            .NotEmpty()
            .WithMessage("Customer ID is required.");

        RuleFor(x => x.CustomerName)
            .NotEmpty()
            .WithMessage("Customer name is required.")
            .MaximumLength(100)
            .WithMessage("Customer name cannot exceed 100 characters.");

        RuleFor(x => x.Branch)
            .NotEmpty()
            .WithMessage("Branch is required.")
            .MaximumLength(50)
            .WithMessage("Branch cannot exceed 50 characters.");

        RuleFor(x => x.Items)
            .NotEmpty()
            .WithMessage("Sale must have at least one item.")
            .ForEach(items => items.SetValidator(new CreateSaleItemCommandValidator()));
    }
}