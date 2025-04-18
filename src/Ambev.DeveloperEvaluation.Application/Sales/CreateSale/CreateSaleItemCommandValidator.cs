using FluentValidation;

namespace Ambev.DeveloperEvaluation.Application.Sales.CreateSale;

public class CreateSaleItemCommandValidator : AbstractValidator<CreateSaleItemCommand>
{
    public CreateSaleItemCommandValidator()
    {
        RuleFor(x => x.Quantity)
            .GreaterThan(0)
            .InclusiveBetween(1, 20)
            .WithMessage("Quantity must be between 1 and 20.");

        RuleFor(x => x.ProductId)
            .NotEmpty()
            .WithMessage("ProductId is required.");

        RuleFor(x => x.Discount)
            .GreaterThanOrEqualTo(0)
            .WithMessage("Discount cannot be negative for each item.");

        RuleFor(x => x.ProductDetails)
            .NotNull()
            .WithMessage("Product details are required.");

        RuleFor(x => x.ProductDetails.Price)
            .GreaterThan(0)
            .WithMessage("Product price must be greater than zero.");
    }
}