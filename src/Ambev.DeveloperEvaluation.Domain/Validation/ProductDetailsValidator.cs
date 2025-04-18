using Ambev.DeveloperEvaluation.Domain.ValueObjects;
using FluentValidation;

namespace Ambev.DeveloperEvaluation.Domain.Validation;

/// <summary>
/// Validator for <see cref="ProductDetails"/> value object.
/// </summary>
public class ProductDetailsValidator : AbstractValidator<ProductDetails>
{
    public ProductDetailsValidator()
    {
        RuleFor(x => x.Title)
            .NotEmpty().WithMessage("Product title is required.")
            .MaximumLength(255).WithMessage("Product title must be at most 255 characters.");

        RuleFor(x => x.Category)
            .NotEmpty().WithMessage("Product category is required.")
            .MaximumLength(100).WithMessage("Product category must be at most 100 characters.");

        RuleFor(x => x.Price)
            .GreaterThan(0).WithMessage("Product price must be greater than 0.");

        RuleFor(x => x.Image)
            .NotEmpty().WithMessage("Product image is required.");
    }
}