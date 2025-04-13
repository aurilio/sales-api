using System.ComponentModel.DataAnnotations;

namespace Ambev.DeveloperEvaluation.WebApi.Features.Sales.UpdateSale;

/// <summary>
/// Represents an item in the sale for the update request.
/// </summary>
public class UpdateSaleItemRequest
{
    /// <summary>
    /// The external identifier of the product.
    /// </summary>
    [Required]
    public required string ProductId { get; set; }

    /// <summary>
    /// The quantity of the product sold.
    /// </summary>
    [Range(1, 20)]
    public int? Quantity { get; set; }

    /// <summary>
    /// The unit price of the product at the time of sale.
    /// </summary>
    [Range(0.01, double.MaxValue)]
    public decimal? UnitPrice { get; set; }

    /// <summary>
    /// The discount applied to this item (if any).
    /// </summary>
    public decimal? Discount { get; set; }
}