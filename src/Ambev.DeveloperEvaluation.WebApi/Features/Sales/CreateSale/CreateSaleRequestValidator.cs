using FluentValidation;

namespace Ambev.DeveloperEvaluation.WebApi.Features.Sales.CreateSale;

public class CreateSaleRequestValidator : AbstractValidator<CreateSaleRequest>
{
    public CreateSaleRequestValidator()
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
            .WithMessage("Customer ID is required.")
            .Must(BeAValidGuid).WithMessage("Customer ID must be a valid GUID.");

        RuleFor(x => x.CustomerName)
            .NotEmpty()
            .WithMessage("Customer name is required.")
            .MaximumLength(100) // Defina um tamanho máximo apropriado
            .WithMessage("Customer name cannot exceed 100 characters.");

        RuleFor(x => x.Branch)
            .NotEmpty()
            .WithMessage("Branch is required.")
            .MaximumLength(50)
            .WithMessage("Branch cannot exceed 50 characters.");

        RuleFor(x => x.Items)
            .NotEmpty()
            .WithMessage("Sale must have at least one item.")
            .ForEach(items => items.SetValidator(new SaleItemRequestValidator()));
    }

    private bool BeAValidGuid(Guid customerId)
    {
        return customerId != Guid.Empty;
    }
}