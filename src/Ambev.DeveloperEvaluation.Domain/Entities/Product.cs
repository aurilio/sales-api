namespace Ambev.DeveloperEvaluation.Domain.Entities;

/// <summary>
/// Represents a product.
/// </summary>
public class Product
{
    /// <summary>
    /// The unique identifier of the product (External Identity), corresponding to the ID from the Product API.
    /// </summary>
    public int Id { get; set; }

    /// <summary>
    /// The title or name of the product.
    /// </summary>
    public string Title { get; set; } = string.Empty;

    /// <summary>
    /// The price of the product.
    /// </summary>
    public decimal Price { get; set; }

    /// <summary>
    /// A brief description of the product.
    /// </summary>
    public string Description { get; set; } = string.Empty;

    /// <summary>
    /// The category the product belongs to.
    /// </summary>
    public string Category { get; set; } = string.Empty;

    /// <summary>
    /// URL or path to the product's image.
    /// </summary>
    public string Image { get; set; } = string.Empty;

    /// <summary>
    /// The rating details of the product.
    /// </summary>
    public Rating Rating { get; set; } = new ();
}