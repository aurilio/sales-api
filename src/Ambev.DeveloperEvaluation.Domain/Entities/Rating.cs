namespace Ambev.DeveloperEvaluation.Domain.Entities;

/// <summary>
/// Represents the rating of a product.
/// </summary>
public class Rating
{
    /// <summary>
    /// The rating value (e.g., average rating).
    /// </summary>
    public decimal Rate { get; set; }

    /// <summary>
    /// The number of ratings for the product.
    /// </summary>
    public int Count { get; set; }
}