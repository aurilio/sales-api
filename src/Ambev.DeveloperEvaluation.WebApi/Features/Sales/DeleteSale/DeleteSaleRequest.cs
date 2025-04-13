using System.ComponentModel.DataAnnotations;

namespace Ambev.DeveloperEvaluation.WebApi.Features.Sales.DeleteSale;

/// <summary>
/// Request model for deleting a sale record.
/// </summary>
public class DeleteSaleRequest
{
    /// <summary>
    /// The unique identifier of the sale record to delete.
    /// </summary>
    [Required]
    public Guid Id { get; set; }
}