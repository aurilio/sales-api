namespace Ambev.DeveloperEvaluation.WebApi.Features.Sales.CreateSale;

/// <summary>
/// Response model for creating a new sale record.
/// </summary>
public class CreateSaleResponse
{
    /// <summary>
    /// The unique identifier of the created sale record.
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
    /// The external identifier of the customer.
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
    /// A list of items included in the sale.
    /// </summary>
    public List<SaleItemResponse> Items { get; set; } = new List<SaleItemResponse>();

    /// <summary>
    /// Indicates if the sale is cancelled or not.
    /// </summary>
    public bool IsCancelled { get; set; }
}