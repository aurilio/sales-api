namespace Ambev.DeveloperEvaluation.WebApi.Features.Sales.CreateSale;

/// <summary>
/// Represents the product snapshot details returned in a sale item response.
/// </summary>
public class ProductDetailsResponse
{
    /// <summary>
    /// Gets or sets the name or title of the product.
    /// </summary>
    public string Title { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the category of the product.
    /// </summary>
    public string Category { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the price of the product at the time of sale.
    /// </summary>
    public decimal Price { get; set; }

    /// <summary>
    /// Gets or sets the image URL of the product.
    /// </summary>
    public string Image { get; set; } = string.Empty;
}