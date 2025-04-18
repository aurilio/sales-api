using Ambev.DeveloperEvaluation.Domain.ValueObjects;

namespace Ambev.DeveloperEvaluation.Application.Sales.CreateSale;

/// <summary>
/// Represents an individual item in a sale, returned as part of the CreateSaleResult.
/// </summary>
public class CreateSaleItemResult
{
    /// <summary>
    /// Gets or sets the identifier of the product that was sold.
    /// </summary>
    public Guid ProductId { get; set; }

    /// <summary>
    /// Gets or sets the quantity of the product sold.
    /// </summary>
    public int Quantity { get; set; }

    /// <summary>
    /// Gets or sets the unit price of the product at the time of the sale.
    /// </summary>
    public decimal UnitPrice { get; set; }

    /// <summary>
    /// Gets or sets the discount applied to the item, expressed as a percentage (0 to 1).
    /// </summary>
    public decimal Discount { get; set; }

    /// <summary>
    /// Gets or sets the total amount for the item after applying the discount.
    /// </summary>
    public decimal TotalAmount { get; set; }

    /// <summary>
    /// Gets the date and time when the item was created.
    /// </summary>
    public DateTime CreatedAt { get; set; }

    /// <summary>
    /// Gets the date and time when the item was last updated.
    /// </summary>
    public DateTime UpdatedAt { get; private set; }

    /// <summary>
    /// Gets or sets the product details as a snapshot at the moment of the sale.
    /// </summary>
    public ProductDetails ProductDetails { get; set; } = default!;
}