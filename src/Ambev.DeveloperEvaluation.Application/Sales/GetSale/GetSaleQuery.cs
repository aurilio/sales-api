using Ambev.DeveloperEvaluation.Common.Validation;
using MediatR;

namespace Ambev.DeveloperEvaluation.Application.Sales.GetSale;

/// <summary>
/// Query for retrieving a specific sale record by its ID.
/// </summary>
public class GetSaleQuery : IRequest<GetSaleResult>
{
    /// <summary>
    /// The unique identifier of the sale record to retrieve.
    /// </summary>
    public Guid Id { get; }

    public GetSaleQuery(Guid id)
    {
        Id = id;
    }

    public ValidationResultDetail Validate()
    {
        var validator = new GetSaleQueryValidator();
        var result = validator.Validate(this);
        return new ValidationResultDetail
        {
            IsValid = result.IsValid,
            Errors = result.Errors.Select(o => (ValidationErrorDetail)o)
        };
    }
}