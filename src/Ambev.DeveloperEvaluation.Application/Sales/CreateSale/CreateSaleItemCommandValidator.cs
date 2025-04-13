using FluentValidation;

namespace Ambev.DeveloperEvaluation.Application.Sales.CreateSale;

public class CreateSaleItemCommandValidator : AbstractValidator<CreateSaleItemCommand>
{
    public CreateSaleItemCommandValidator()
    {
        RuleFor(x => x.ProductId)
            .NotEmpty()
            .WithMessage("Product ID is required for each item.")
            .MaximumLength(50)
            .WithMessage("Product ID cannot exceed 50 characters for each item.");

        RuleFor(x => x.Quantity)
            .GreaterThan(0)
            .WithMessage("Quantity must be greater than zero for each item.")
            .LessThanOrEqualTo(20)
            .WithMessage("Quantity cannot exceed 20 for each item.");

        RuleFor(x => x.UnitPrice)
            .GreaterThan(0)
            .WithMessage("Unit price must be greater than zero for each item.");

        RuleFor(x => x.Discount)
            .GreaterThanOrEqualTo(0)
            .WithMessage("Discount cannot be negative for each item.");
    }
}