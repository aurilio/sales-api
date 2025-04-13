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
    public string CustomerId { get; set; } = string.Empty;

    /// <summary>
    /// The total amount of the sale.
    /// </summary>
    public decimal TotalAmount { get; set; }

    /// <summary>
    /// The branch where the sale was made.
    /// </summary>
    public string BranchId { get; set; } = string.Empty;

    /// <summary>
    /// A list of products included in the sale.
    /// </summary>
    public List<GetSaleItemResult> Items { get; set; } = new List<GetSaleItemResult>();

    /// <summary>
    /// Indicates if the sale is cancelled or not.
    /// </summary>
    public bool IsCancelled { get; set; }
}