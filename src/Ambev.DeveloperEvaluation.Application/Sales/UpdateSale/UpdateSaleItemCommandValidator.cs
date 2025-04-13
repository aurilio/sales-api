using FluentValidation;

namespace Ambev.DeveloperEvaluation.Application.Sales.UpdateSale;

public class UpdateSaleItemCommandValidator : AbstractValidator<UpdateSaleItemCommand>
{
    public UpdateSaleItemCommandValidator()
    {
        RuleFor(x => x.ProductId)
            .NotEmpty()
            .WithMessage("Product ID is required for each item.")
            .MaximumLength(50)
            .WithMessage("Product ID cannot exceed 50 characters for each item.")
            .When(x => x.ProductId != null);

        RuleFor(x => x.Quantity)
            .GreaterThan(0)
            .WithMessage("Quantity must be greater than zero for each item.")
            .LessThanOrEqualTo(20)
            .WithMessage("Quantity cannot exceed 20 for each item.")
            .When(x => x.Quantity.HasValue);

        RuleFor(x => x.UnitPrice)
            .GreaterThan(0)
            .WithMessage("Unit price must be greater than zero for each item.")
            .When(x => x.UnitPrice.HasValue);

        RuleFor(x => x.Discount)
            .GreaterThanOrEqualTo(0)
            .WithMessage("Discount cannot be negative for each item.")
            .When(x => x.Discount.HasValue);
    }
}