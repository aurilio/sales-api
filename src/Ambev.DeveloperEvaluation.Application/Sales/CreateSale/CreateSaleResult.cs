namespace Ambev.DeveloperEvaluation.Application.Sales.CreateSale;

/// <summary>
/// Result for the create sale operation.
/// </summary>
public class CreateSaleResult
{
    /// <summary>
    /// The unique identifier of the created sale record.
    /// </summary>
    public Guid Id { get; set; }
}