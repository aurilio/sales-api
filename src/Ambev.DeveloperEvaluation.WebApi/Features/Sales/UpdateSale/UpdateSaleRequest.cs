using System.ComponentModel.DataAnnotations;

namespace Ambev.DeveloperEvaluation.WebApi.Features.Sales.UpdateSale;

/// <summary>
/// Request model for updating an existing sale record.
/// </summary>
public class UpdateSaleRequest
{
    /// <summary>
    /// The number of the sale.
    /// </summary>
    public string SaleNumber { get; set; } = string.Empty;

    /// <summary>
    /// The date when the sale was made.
    /// </summary>
    public DateTime? SaleDate { get; set; }

    /// <summary>
    /// Information about the customer (external identifier).
    /// </summary>
    public string CustomerId { get; set; } = string.Empty;

    /// <summary>
    /// The total amount of the sale.
    /// </summary>
    [Range(0.01, double.MaxValue)]
    public decimal? TotalAmount { get; set; }

    /// <summary>
    /// The branch where the sale was made.
    /// </summary>
    public string BranchId { get; set; } = string.Empty;

    /// <summary>
    /// A list of products included in the sale.
    /// </summary>
    public List<UpdateSaleItemRequest> Items { get; set; } = new List<UpdateSaleItemRequest>();

    /// <summary>
    /// Indicates if the sale is cancelled or not.
    /// </summary>
    public bool? IsCancelled { get; set; }
}