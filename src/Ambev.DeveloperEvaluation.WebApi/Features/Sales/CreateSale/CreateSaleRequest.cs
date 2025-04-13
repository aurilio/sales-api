using System.ComponentModel.DataAnnotations;

namespace Ambev.DeveloperEvaluation.WebApi.Features.Sales.CreateSale;

/// <summary>
/// Request model for creating a new sale record.
/// </summary>
public class CreateSaleRequest
{
    /// <summary>
    /// The number of the sale.
    /// </summary>
    [Required]
    public required string SaleNumber { get; set; }

    /// <summary>
    /// The date when the sale was made.
    /// </summary>
    [Required]
    public DateTime SaleDate { get; set; }

    /// <summary>
    /// Information about the customer (external identifier).
    /// </summary>
    [Required]
    public required string CustomerId { get; set; }

    /// <summary>
    /// The total amount of the sale.
    /// </summary>
    [Required]
    [Range(0.01, double.MaxValue)]
    public decimal TotalAmount { get; set; }

    /// <summary>
    /// The branch where the sale was made.
    /// </summary>
    [Required]
    public required string BranchId { get; set; }

    /// <summary>
    /// A list of products included in the sale.
    /// </summary>
    [Required]
    public List<SaleItemRequest> Items { get; set; } = new List<SaleItemRequest>();

    /// <summary>
    /// Indicates if the sale is cancelled or not.
    /// </summary>
    public bool IsCancelled { get; set; }
}