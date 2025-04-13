using FluentValidation;

namespace Ambev.DeveloperEvaluation.WebApi.Features.Sales.ListSales;

public class ListSaleRequestValidator : AbstractValidator<ListSaleRequest>
{
    public ListSaleRequestValidator()
    {
        RuleFor(x => x.Page)
            .GreaterThanOrEqualTo(1)
            .WithMessage("Page number must be at least 1.");

        RuleFor(x => x.Size)
            .GreaterThanOrEqualTo(1)
            .LessThanOrEqualTo(100)
            .WithMessage("Page size must be between 1 and 100.");

        RuleFor(x => x.OrderBy)
            .MaximumLength(200)
            .WithMessage("OrderBy string cannot exceed 200 characters.");
    }
}