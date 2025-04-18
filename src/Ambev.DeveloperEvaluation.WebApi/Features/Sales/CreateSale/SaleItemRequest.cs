namespace Ambev.DeveloperEvaluation.WebApi.Features.Sales.CreateSale;

/// <summary>
/// Represents an item to be included in a sale request.
/// </summary>
public class SaleItemRequest
{
    /// <summary>
    /// Gets or sets the external identifier of the product.
    /// This should match the ID from the product catalog.
    /// </summary>
    public Guid ProductId { get; set; }

    /// <summary>
    /// Gets or sets the quantity of the product being sold.
    /// Must be between 1 and 20, following business rules.
    /// </summary>
    public int Quantity { get; set; }

    /// <summary>
    /// Gets or sets the denormalized product details (snapshot) at the moment of the sale.
    /// This is required to maintain historical data consistency.
    /// </summary>
    public ProductDetailsRequest ProductDetails { get; set; } = new();
}