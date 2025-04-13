using System.ComponentModel.DataAnnotations;

namespace Ambev.DeveloperEvaluation.WebApi.Features.Sales.CreateSale;

/// <summary>
/// Represents an item in the sale for the create request.
/// </summary>
public class SaleItemRequest
{
    /// <summary>
    /// The external identifier of the product.
    /// </summary>
    [Required]
    public required string ProductId { get; set; }

    /// <summary>
    /// The quantity of the product sold.
    /// </summary>
    [Required]
    [Range(1, 20)]
    public int Quantity { get; set; }

    /// <summary>
    /// The unit price of the product at the time of sale.
    /// </summary>
    [Required]
    [Range(0.01, double.MaxValue)]
    public decimal UnitPrice { get; set; }

    /// <summary>
    /// The discount applied to this item (if any).
    /// </summary>
    public decimal Discount { get; set; }
}