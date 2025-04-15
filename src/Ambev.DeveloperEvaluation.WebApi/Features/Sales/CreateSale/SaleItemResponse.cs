namespace Ambev.DeveloperEvaluation.WebApi.Features.Sales.CreateSale;

/// <summary>
/// Represents an item in the sale for the response.
/// </summary>
public class SaleItemResponse
{
    /// <summary>
    /// The unique identifier of the sale item.
    /// </summary>
    public Guid Id { get; set; }

    /// <summary>
    /// The external identifier of the product.
    /// </summary>
    public int ProductId { get; set; }

    /// <summary>
    /// The denormalized title or name of the product.
    /// </summary>
    public string ProductName { get; set; } = string.Empty;

    /// <summary>
    /// The quantity of the product sold.
    /// </summary>
    public int Quantity { get; set; }

    /// <summary>
    /// The unit price of the product at the time of sale.
    /// </summary>
    public decimal UnitPrice { get; set; }

    /// <summary>
    /// The discount applied to this item.
    /// </summary>
    public decimal Discount { get; set; }

    /// <summary>
    /// The total amount for this item.
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
}