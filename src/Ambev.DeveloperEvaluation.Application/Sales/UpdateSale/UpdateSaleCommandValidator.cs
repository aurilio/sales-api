using FluentValidation;

namespace Ambev.DeveloperEvaluation.Application.Sales.UpdateSale;

public class UpdateSaleCommandValidator : AbstractValidator<UpdateSaleCommand>
{
    public UpdateSaleCommandValidator()
    {
        RuleFor(x => x.Id)
            .NotEmpty()
            .WithMessage("Sale ID is required.")
            .NotEqual(Guid.Empty)
            .WithMessage("Sale ID must be a valid GUID.");

        RuleFor(x => x.SaleNumber)
            .MaximumLength(50)
            .WithMessage("Sale number cannot exceed 50 characters.")
            .When(x => !string.IsNullOrEmpty(x.SaleNumber));

        RuleFor(x => x.SaleDate)
            .LessThanOrEqualTo(DateTime.Now.ToLocalTime())
            .WithMessage("Sale date cannot be in the future.")
            .When(x => x.SaleDate.HasValue);

        RuleFor(x => x.CustomerId)
            .Must(BeAValidGuid).WithMessage("Customer ID must be a valid GUID.")
            .When(x => x.CustomerId.HasValue);

        RuleFor(x => x.CustomerName)
            .MaximumLength(100)
            .WithMessage("Customer name cannot exceed 100 characters.")
            .When(x => !string.IsNullOrEmpty(x.CustomerName));

        RuleFor(x => x.Branch)
            .MaximumLength(50)
            .WithMessage("Branch cannot exceed 50 characters.")
            .When(x => !string.IsNullOrEmpty(x.Branch));

        RuleFor(x => x.Items)
            .ForEach(items => items.SetValidator(new UpdateSaleItemCommandValidator()))
            .When(x => x.Items != null);

        RuleFor(x => x.IsCancelled)
            .NotNull()
            .WithMessage("IsCancelled must be specified.")
            .When(x => x.IsCancelled.HasValue);
    }

    private bool BeAValidGuid(Guid? customerId)
    {
        return !customerId.HasValue || customerId.Value != Guid.Empty;
    }
}