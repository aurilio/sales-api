using System.ComponentModel.DataAnnotations;

namespace Ambev.DeveloperEvaluation.WebApi.Features.Sales.GetSale;
/// <summary>
/// Request model for retrieving a specific sale record by ID.
/// </summary>
public class GetSaleRequest
{
    /// <summary>
    /// The unique identifier of the sale record to retrieve.
    /// </summary>
    [Required]
    public Guid Id { get; set; }
}