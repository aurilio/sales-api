using FluentValidation;

namespace Ambev.DeveloperEvaluation.WebApi.Features.Sales.CreateSale;

/// <summary>
/// Validator for the <see cref="ProductDetailsRequest"/> class.
/// Ensures that product snapshot information is complete and well-formed.
/// </summary>
public class ProductDetailsRequestValidator : AbstractValidator<ProductDetailsRequest>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="ProductDetailsRequestValidator"/> class.
    /// Defines validation rules for product snapshot fields including title, category, price, and image.
    /// </summary>
    public ProductDetailsRequestValidator()
    {
        RuleFor(p => p.Title)
            .NotEmpty()
            .WithMessage("Product title is required.")
            .MaximumLength(255)
            .WithMessage("Product title cannot exceed 255 characters.");

        RuleFor(p => p.Category)
            .NotEmpty()
            .WithMessage("Product category is required.")
            .MaximumLength(100)
            .WithMessage("Product category cannot exceed 100 characters.");

        RuleFor(p => p.Price)
            .GreaterThan(0)
            .WithMessage("Product price must be greater than zero.");

        RuleFor(p => p.Image)
            .NotEmpty()
            .WithMessage("Product image URL is required.");
    }
}