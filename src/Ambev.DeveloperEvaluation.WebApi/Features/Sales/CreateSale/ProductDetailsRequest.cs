namespace Ambev.DeveloperEvaluation.WebApi.Features.Sales.CreateSale;

/// <summary>
/// Represents the snapshot of a product's details at the time of the sale.
/// Used for denormalization and historical accuracy within a sale item.
/// </summary>
public class ProductDetailsRequest
{
    /// <summary>
    /// Gets or sets the name or title of the product.
    /// This is used to display and identify the product within the context of the sale.
    /// </summary>
    public string Title { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the category under which the product is classified.
    /// Used for organizational and reporting purposes.
    /// </summary>
    public string Category { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the price of the product at the time of the sale.
    /// This value is denormalized and may differ from the current product catalog.
    /// </summary>
    public decimal Price { get; set; }

    /// <summary>
    /// Gets or sets the image URL of the product at the time of the sale.
    /// Stored for UI consistency in receipts, reports, or visual display.
    /// </summary>
    public string Image { get; set; } = string.Empty;
}
