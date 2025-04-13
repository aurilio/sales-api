using Ambev.DeveloperEvaluation.Domain.Entities;
using FluentValidation;

namespace Ambev.DeveloperEvaluation.Domain.Validation;

public class SaleItemValidator : AbstractValidator<SaleItem>
{
    public SaleItemValidator()
    {
        RuleFor(item => item.ProductId)
            .NotEmpty()
            .WithMessage("Product ID is required.")
            .MaximumLength(50)
            .WithMessage("Product ID cannot exceed 50 characters.");

        RuleFor(item => item.Quantity)
            .GreaterThan(0)
            .WithMessage("Quantity must be greater than zero.");

        RuleFor(item => item.UnitPrice)
            .GreaterThanOrEqualTo(0)
            .WithMessage("Unit price must be greater than or equal to zero.");

        RuleFor(item => item.Discount)
            .GreaterThanOrEqualTo(0)
            .WithMessage("Discount must be greater than or equal to zero.");
    }
}