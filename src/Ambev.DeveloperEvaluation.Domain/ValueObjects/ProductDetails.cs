namespace Ambev.DeveloperEvaluation.Domain.ValueObjects;

/// <summary>
/// Represents a snapshot of product details at the time of the sale.
/// Used to ensure historical consistency in sale records.
/// </summary>
public class ProductDetails
{
    /// <summary>
    /// Gets the name/title of the product.
    /// </summary>
    public string Title { get; }

    /// <summary>
    /// Gets the category under which the product is classified.
    /// </summary>
    public string Category { get; }

    /// <summary>
    /// Gets the unit price of the product at the time of sale.
    /// </summary>
    public decimal Price { get; }

    /// <summary>
    /// Gets the image URL of the product.
    /// </summary>
    public string Image { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="ProductDetails"/> class.
    /// </summary>
    /// <param name="title">The name of the product.</param>
    /// <param name="category">The product's category.</param>
    /// <param name="price">The price per unit.</param>
    /// <param name="image">The URL of the product's image.</param>
    public ProductDetails(string title, string category, decimal price, string image)
    {
        Title = title;
        Category = category;
        Price = price;
        Image = image;
    }
}