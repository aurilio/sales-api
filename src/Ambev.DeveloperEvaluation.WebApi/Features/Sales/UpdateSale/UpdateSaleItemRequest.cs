using Ambev.DeveloperEvaluation.WebApi.Features.Sales.CreateSale;
using System.ComponentModel.DataAnnotations;

namespace Ambev.DeveloperEvaluation.WebApi.Features.Sales.UpdateSale;

/// <summary>
/// Represents an item in the sale for the update request.
/// </summary>
public class UpdateSaleItemRequest
{
    /// <summary>
    /// The unique identifier.
    /// </summary>
    public Guid? Id { get; set; }

    /// <summary>
    /// Product identifier.
    /// </summary>
    [Required]
    public Guid ProductId { get; set; }

    /// <summary>
    /// The quantity of the product.
    /// </summary>
    [Range(1, 20)]
    public int Quantity { get; set; }

    /// <summary>
    /// The product details at the time of the sale.
    /// </summary>
    public ProductDetailsRequest ProductDetails { get; set; } = new();
}