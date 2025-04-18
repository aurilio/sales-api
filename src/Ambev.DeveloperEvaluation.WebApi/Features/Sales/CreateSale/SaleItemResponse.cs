namespace Ambev.DeveloperEvaluation.WebApi.Features.Sales.CreateSale;

/// <summary>
/// Represents the response details for each item in the sale.
/// </summary>
public class SaleItemResponse
{
    /// <summary>
    /// Gets or sets the external product identifier.
    /// </summary>
    public Guid ProductId { get; set; }

    /// <summary>
    /// Gets or sets the quantity of the product sold.
    /// </summary>
    public int Quantity { get; set; }

    /// <summary>
    /// Gets or sets the unit price of the product at the time of sale.
    /// </summary>
    public decimal UnitPrice { get; set; }

    /// <summary>
    /// Gets or sets the discount applied to the item (percentage as decimal).
    /// </summary>
    public decimal Discount { get; set; }

    /// <summary>
    /// Gets or sets the total amount after discount for the item.
    /// </summary>
    public decimal TotalAmount { get; set; }

    /// <summary>
    /// Gets the date and time when this sale item record was created.
    /// </summary>
    public DateTime CreatedAt { get; set; }

    /// <summary>
    /// Gets the date and time of the last update to this sale item's information.
    /// </summary>
    public DateTime? UpdatedAt { get; set; }

    /// <summary>
    /// Gets or sets the product snapshot information at the time of the sale.
    /// </summary>
    public ProductDetailsResponse ProductDetails { get; set; } = new();
}
