namespace Ambev.DeveloperEvaluation.Application.Sales.GetSale;

/// <summary>
/// Result for the get sale by ID operation.
/// </summary>
public class GetSaleResult
{
    /// <summary>
    /// The unique identifier of the sale record.
    /// </summary>
    public Guid Id { get; set; }

    /// <summary>
    /// The number of the sale.
    /// </summary>
    public string SaleNumber { get; set; } = string.Empty;

    /// <summary>
    /// The date when the sale was made.
    /// </summary>
    public DateTime SaleDate { get; set; }

    /// <summary>
    /// Information about the customer (external identifier).
    /// </summary>
    public Guid CustomerId { get; set; }

    /// <summary>
    /// The denormalized name of the customer.
    /// </summary>
    public string CustomerName { get; set; } = string.Empty;

    /// <summary>
    /// The total amount of the sale.
    /// </summary>
    public decimal TotalAmount { get; set; }

    /// <summary>
    /// The branch where the sale was made.
    /// </summary>
    public string Branch { get; set; } = string.Empty;

    /// <summary>
    /// Indicates if the sale is cancelled or not.
    /// </summary>
    public bool IsCancelled { get; set; }

    /// <summary>
    /// Gets the date and time when the sale record was created.
    /// </summary>
    public DateTime CreatedAt { get; set; }

    /// <summary>
    /// Gets the date and time of the last update to the sale's information.
    /// </summary>
    public DateTime? UpdatedAt { get; set; }

    /// <summary>
    /// A list of products included in the sale.
    /// </summary>
    public List<GetSaleItemResult> Items { get; set; } = new ();
}