using FluentValidation;

namespace Ambev.DeveloperEvaluation.WebApi.Features.Sales.DeleteSale;

public class DeleteSaleRequestValidator : AbstractValidator<DeleteSaleRequest>
{
    public DeleteSaleRequestValidator()
    {
        RuleFor(x => x.Id)
            .NotEmpty()
            .WithMessage("Sale ID is required.")
            .NotEqual(Guid.Empty)
            .WithMessage("Sale ID must be a valid GUID.");
    }
}