using System.ComponentModel.DataAnnotations;

namespace Ambev.DeveloperEvaluation.WebApi.Features.Sales.UpdateSale;

/// <summary>
/// Request model for updating an existing sale record.
/// </summary>
public class UpdateSaleRequest
{
    /// <summary>
    /// Updated sale number.
    /// </summary>
    [Required]
    public string SaleNumber { get; set; } = string.Empty;

    /// <summary>
    /// Updated date when the sale was made.
    /// </summary>
    [Required]
    public DateTime SaleDate { get; set; }

    /// <summary>
    /// Updated identifier of the customer.
    /// </summary>
    [Required]
    public Guid CustomerId { get; set; }

    /// <summary>
    /// Updated name of the customer.
    /// </summary>
    public string CustomerName { get; set; } = string.Empty;

    /// <summary>
    /// Updated branch name where the sale occurred.
    /// </summary>
    [Required]
    public string Branch { get; set; } = string.Empty;

    /// <summary>
    /// Indicates if the sale is cancelled or not.
    /// </summary>
    public bool? IsCancelled { get; set; }

    /// <summary>
    /// A list of products included in the sale.
    /// </summary>
    public List<UpdateSaleItemRequest> Items { get; set; } = new ();
}