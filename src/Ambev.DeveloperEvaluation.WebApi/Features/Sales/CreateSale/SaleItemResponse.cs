﻿namespace Ambev.DeveloperEvaluation.WebApi.Features.Sales.CreateSale;

/// <summary>
/// Represents an item in the sale for the response.
/// </summary>
public class SaleItemResponse
{
    /// <summary>
    /// The external identifier of the product.
    /// </summary>
    public string ProductId { get; set; } = string.Empty;

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
    public decimal ItemTotal { get; set; }
}