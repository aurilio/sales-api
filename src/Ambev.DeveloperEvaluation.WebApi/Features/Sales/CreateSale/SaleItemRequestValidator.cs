using FluentValidation;

namespace Ambev.DeveloperEvaluation.WebApi.Features.Sales.CreateSale;

/// <summary>
/// Validator for the <see cref="SaleItemRequest"/> class.
/// Ensures that each sale item in the request adheres to the business rules and required data structure.
/// </summary>
public class SaleItemRequestValidator : AbstractValidator<SaleItemRequest>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="SaleItemRequestValidator"/> class.
    /// Defines validation rules for sale item data including quantity limits, required fields, and nested product details.
    /// </summary>
    public SaleItemRequestValidator()
    {
        RuleFor(x => x.ProductId)
            .NotEmpty()
            .WithMessage("Product ID is required for each item.")
            .Must(BeAValidGuid).WithMessage("Product ID must be a valid GUID.");

        RuleFor(x => x.Quantity)
            .InclusiveBetween(1, 20)
            .WithMessage("Quantity must be between 1 and 20.");

        RuleFor(x => x.ProductDetails)
            .NotNull()
            .WithMessage("Product details must be provided for each item.")
            .SetValidator(new ProductDetailsRequestValidator());
    }

    private bool BeAValidGuid(Guid customerId)
    {
        return customerId != Guid.Empty;
    }
}