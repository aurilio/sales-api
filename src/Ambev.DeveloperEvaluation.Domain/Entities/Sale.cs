using Ambev.DeveloperEvaluation.Common.Validation;
using Ambev.DeveloperEvaluation.Domain.Common;
using Ambev.DeveloperEvaluation.Domain.Validation;

namespace Ambev.DeveloperEvaluation.Domain.Entities;

/// <summary>
/// Represents a sale record in the system.
/// This entity follows domain-driven design principles and includes business rules validation.
/// </summary>
public class Sale : BaseEntity
{
    /// <summary>
    /// The unique number or identifier for the sale.
    /// Must not be null or empty and should have a valid format if applicable.
    /// </summary>
    public string SaleNumber { get; set; } = string.Empty;

    /// <summary>
    /// The date when the sale occurred.
    /// Must be a valid past or current date.
    /// </summary>
    public DateTime SaleDate { get; set; }

    /// <summary>
    /// The external identifier of the customer who made the sale.
    /// </summary>
    public Guid CustomerId { get; set; }

    /// <summary>
    /// The denormalized name of the customer.
    /// </summary>
    public string CustomerName { get; set; } = string.Empty;

    /// <summary>
    /// The total amount of the sale.
    /// Must be greater than zero.
    /// </summary>
    public decimal TotalAmount { get; private set; }

    /// <summary>
    /// The branch where the sale was made.
    /// </summary>
    public string Branch { get; set; } = string.Empty;

    /// <summary>
    /// Indicates whether the sale has been cancelled.
    /// Defaults to false.
    /// </summary>
    public bool IsCancelled { get; set; } = false;

    /// <summary>
    /// The collection of items included in this sale.
    /// Must not be null and should contain at least one item for a valid sale.
    /// </summary>
    public IList<SaleItem> Items { get; set; } = new List<SaleItem>();

    /// <summary>
    /// Gets the date and time when the sale record was created.
    /// </summary>
    public DateTime CreatedAt { get; private set; }

    /// <summary>
    /// Gets the date and time of the last update to the sale's information.
    /// </summary>
    public DateTime? UpdatedAt { get; private set; }

    /// <summary>
    /// Initializes a new instance of the <see cref="Sale"/> class.
    /// Sets the creation timestamp.
    /// </summary>
    public Sale()
    {
        CreatedAt = DateTime.UtcNow;
    }

    /// <summary>
    /// Adds a new item to the sale and recalculates the total amount.
    /// </summary>
    /// <param name="item">The <see cref="SaleItem"/> to add.</param>
    public void AddItem(SaleItem item)
    {
        if (item == null)
        {
            throw new ArgumentNullException(nameof(item), "Sale item cannot be null.");
        }
        Items.Add(item);
        CalculateTotalAmount();
    }

    /// <summary>
    /// Removes an item from the sale and recalculates the total amount.
    /// </summary>
    /// <param name="item">The <see cref="SaleItem"/> to remove.</param>
    /// <returns>True if the item was successfully removed; otherwise, false.</returns>
    public bool RemoveItem(SaleItem item)
    {
        var removed = Items.Remove(item);
        if (removed)
        {
            CalculateTotalAmount();
        }
        return removed;
    }

    /// <summary>
    /// Recalculates the total amount of the sale based on the current items.
    /// </summary>
    private void CalculateTotalAmount()
    {
        TotalAmount = Items.Sum(item => item.TotalAmount);
        UpdatedAt = DateTime.UtcNow;
    }

    /// <summary>
    /// Performs validation of the sale entity using the <see cref="SaleValidator"/> rules.
    /// </summary>
    /// <returns>
    /// A <see cref="ValidationResultDetail"/> containing:
    /// - IsValid: Indicates whether all validation rules passed
    /// - Errors: Collection of validation errors if any rules failed
    /// </returns>
    /// <remarks>
    /// <listheader>The validation includes checking:</listheader>
    /// <list type="bullet">SaleNumber format and length</list>
    /// <list type="bullet">SaleDate validity (not in the future)</list>
    /// <list type="bullet">CustomerId not null or empty</list>
    /// <list type="bullet">TotalAmount greater than zero</list>
    /// <list type="bullet">BranchId not null or empty</list>
    /// <list type="bullet">Items collection not null and not empty</list>
    /// <list type="bullet">Validation of each <see cref="SaleItem"/> in the <see cref="Items"/> collection</list>
    /// </remarks>
    public ValidationResultDetail Validate()
    {
        var validator = new SaleValidator();
        var result = validator.Validate(this);

        foreach (var item in Items)
        {
            var itemValidationResult = item.Validate();
            if (!itemValidationResult.IsValid)
            {
                foreach (var error in itemValidationResult.Errors)
                {
                    result.Errors.Add(new FluentValidation.Results.ValidationFailure($"Items[{Items.IndexOf(item)}].{error.Error}", error.Detail));
                }
            }
        }

        return new ValidationResultDetail
        {
            IsValid = result.IsValid && !result.Errors.Any(),
            Errors = result.Errors.Select(o => (ValidationErrorDetail)o).ToList()
        };
    }

    /// <summary>
    /// Marks the sale as cancelled.
    /// Sets the <see cref="IsCancelled"/> flag to true and updates the <see cref="UpdatedAt"/> timestamp.
    /// </summary>
    public void Cancel()
    {
        IsCancelled = true;
        UpdatedAt = DateTime.UtcNow;
    }

    public void UpdateUpdatedAt()
    {
        UpdatedAt = DateTime.UtcNow;
    }
}