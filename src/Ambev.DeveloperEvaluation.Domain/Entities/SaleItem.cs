using Ambev.DeveloperEvaluation.Common.Validation;
using Ambev.DeveloperEvaluation.Domain.Common;
using Ambev.DeveloperEvaluation.Domain.Validation;

namespace Ambev.DeveloperEvaluation.Domain.Entities;

/// <summary>
/// Represents an individual item within a sale.
/// </summary>
public class SaleItem : BaseEntity
{
    /// <summary>
    /// The identifier of the product included in this sale item.
    /// Must not be null or empty and should reference a valid product.
    /// </summary>
    public string ProductId { get; set; } = string.Empty;

    /// <summary>
    /// The quantity of the product sold in this item.
    /// Must be greater than zero.
    /// </summary>
    public int Quantity { get; set; }

    /// <summary>
    /// The unit price of the product at the time of the sale.
    /// Must be greater than or equal to zero.
    /// </summary>
    public decimal UnitPrice { get; set; }

    /// <summary>
    /// The discount applied to this sale item (as a percentage or absolute value).
    /// Must be greater than or equal to zero.
    /// </summary>
    public decimal Discount { get; set; }

    /// <summary>
    /// Gets the date and time when this sale item record was created.
    /// </summary>
    public DateTime CreatedAt { get; private set; }

    /// <summary>
    /// Gets the date and time of the last update to this sale item's information.
    /// </summary>
    public DateTime? UpdatedAt { get; private set; }

    /// <summary>
    /// Initializes a new instance of the <see cref="SaleItem"/> class.
    /// Sets the creation timestamp.
    /// </summary>
    public SaleItem()
    {
        CreatedAt = DateTime.UtcNow;
    }

    /// <summary>
    /// Calculates the subtotal for this sale item (Quantity * UnitPrice).
    /// </summary>
    /// <returns>The subtotal amount.</returns>
    public decimal GetSubtotal()
    {
        return Quantity * UnitPrice;
    }

    /// <summary>
    /// Calculates the total price for this sale item after applying the discount.
    /// </summary>
    /// <returns>The total price after discount.</returns>
    public decimal GetTotalPrice()
    {
        var subtotal = GetSubtotal();
        return subtotal - Discount; // Assuming Discount is an absolute value.
                                    // If Discount is a percentage, the calculation would be: subtotal * (1 - Discount / 100).
    }

    /// <summary>
    /// Performs validation of the sale item entity using the <see cref="SaleItemValidator"/> rules.
    /// </summary>
    /// <returns>
    /// A <see cref="ValidationResultDetail"/> containing:
    /// - IsValid: Indicates whether all validation rules passed
    /// - Errors: Collection of validation errors if any rules failed
    /// </returns>
    /// <remarks>
    /// <listheader>The validation includes checking:</listheader>
    /// <list type="bullet">ProductId not null or empty</list>
    /// <list type="bullet">Quantity greater than zero</list>
    /// <list type="bullet">UnitPrice greater than or equal to zero</list>
    /// <list type="bullet">Discount greater than or equal to zero</list>
    /// </remarks>
    public ValidationResultDetail Validate()
    {
        var validator = new SaleItemValidator();
        var result = validator.Validate(this);
        return new ValidationResultDetail
        {
            IsValid = result.IsValid,
            Errors = result.Errors.Select(o => (ValidationErrorDetail)o).ToList()
        };
    }

    /// <summary>
    /// Updates the quantity of this sale item.
    /// </summary>
    /// <param name="newQuantity">The new quantity.</param>
    public void UpdateQuantity(int newQuantity)
    {
        if (newQuantity <= 0)
        {
            throw new ArgumentException("Quantity must be greater than zero.", nameof(newQuantity));
        }
        Quantity = newQuantity;
        UpdatedAt = DateTime.UtcNow;
    }

    /// <summary>
    /// Updates the unit price of this sale item.
    /// </summary>
    /// <param name="newUnitPrice">The new unit price.</param>
    public void UpdateUnitPrice(decimal newUnitPrice)
    {
        if (newUnitPrice < 0)
        {
            throw new ArgumentException("Unit price cannot be negative.", nameof(newUnitPrice));
        }
        UnitPrice = newUnitPrice;
        UpdatedAt = DateTime.UtcNow;
    }

    /// <summary>
    /// Updates the discount applied to this sale item.
    /// </summary>
    /// <param name="newDiscount">The new discount amount.</param>
    public void UpdateDiscount(decimal newDiscount)
    {
        if (newDiscount < 0)
        {
            throw new ArgumentException("Discount cannot be negative.", nameof(newDiscount));
        }
        Discount = newDiscount;
        UpdatedAt = DateTime.UtcNow;
    }
}