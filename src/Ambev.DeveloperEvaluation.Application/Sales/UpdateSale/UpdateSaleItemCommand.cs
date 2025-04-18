using Ambev.DeveloperEvaluation.Domain.ValueObjects;

namespace Ambev.DeveloperEvaluation.Application.Sales.UpdateSale;

/// <summary>
/// Represents an item in the sale for the update command.
/// </summary>
public class UpdateSaleItemCommand
{
    /// <summary>
    /// Product identifier.
    /// </summary>
    public Guid ProductId { get; set; }

    /// <summary>
    /// Quantity of the product.
    /// </summary>
    public int Quantity { get; set; }

    /// <summary>
    /// Unit price of the product.
    /// </summary>
    public decimal UnitPrice { get; set; }

    /// <summary>
    /// Discount applied to the item.
    /// </summary>
    public decimal Discount { get; set; }

    /// <summary>
    /// Total amount for the item.
    /// </summary>
    public decimal TotalAmount { get; set; }

    /// <summary>
    /// Product detail snapshot.
    /// </summary>
    public ProductDetails ProductDetails { get; set; } = default!;
}