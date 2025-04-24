using Ambev.DeveloperEvaluation.Common.Validation;
using Ambev.DeveloperEvaluation.Domain.Common;
using Ambev.DeveloperEvaluation.Domain.Validation;
using Ambev.DeveloperEvaluation.Domain.ValueObjects;

namespace Ambev.DeveloperEvaluation.Domain.Entities;

/// <summary>
/// Represents an item within a sale transaction.
/// </summary>
public class SaleItem : BaseEntity
{

    public Guid SaleId { get; private set; }

    /// <summary>
    /// Gets the identifier of the product from the product catalog.
    /// </summary>
    public Guid ProductId { get; private set; }

    /// <summary>
    /// Gets the quantity of the product sold.
    /// Must be between 1 and 20.
    /// </summary>
    public int Quantity { get; private set; }

    /// <summary>
    /// The unit price of the product at the time of the sale.
    /// Must be greater than or equal to zero.
    /// </summary>
    public decimal UnitPrice { get; private set; }

    /// <summary>
    /// Gets the discount applied to this item based on quantity.
    /// Values: 0, 0.10 (10%), or 0.20 (20%).
    /// </summary>
    public decimal Discount { get; private set; }

    /// <summary>
    /// Gets the total amount for this item after applying the discount.
    /// </summary>
    public decimal TotalAmount { get; private set; }

    /// <summary>
    /// Gets the date and time when the item was created.
    /// </summary>
    public DateTime CreatedAt { get; private set; }

    /// <summary>
    /// Gets the date and time when the item was last updated.
    /// </summary>
    public DateTime? UpdatedAt { get; private set; }

    /// <summary>
    /// Gets the embedded snapshot of the product's details at the time of the sale.
    /// </summary>
    public ProductDetails ProductDetails { get; private set; } = default!;

    /// <summary>
    /// Parameterless constructor for EF Core.
    /// </summary>
    protected SaleItem() { }

    /// <summary>
    /// Initializes a new instance of the <see cref="SaleItem"/> class with specified product, quantity, and product details.
    /// </summary>
    /// <param name="productId">The unique identifier of the product being sold.</param>
    /// <param name="quantity">The number of items sold.</param>
    /// <param name="productDetails">The denormalized product details (title, category, price, image).</param>
    public SaleItem(Guid? id, Guid saleId, Guid productId, int quantity, ProductDetails productDetails)
    {
        Id = id == null ? Guid.NewGuid() : id.Value;
        SaleId = saleId;
        CreatedAt = DateTime.UtcNow;
        SetValues(productId, quantity, productDetails);
    }

    /// <summary>
    /// Updates the current sale item with new product information, quantity, and details.
    /// Automatically recalculates discount, unit price, and total amount.
    /// </summary>
    /// <param name="productId">The updated product identifier.</param>
    /// <param name="quantity">The updated quantity.</param>
    /// <param name="productDetails">The updated product details.</param>
    public void Update(Guid productId, int quantity, ProductDetails productDetails)
    {
        UpdatedAt = DateTime.UtcNow;
        SetValues(productId, quantity, productDetails);
    }

    /// <summary>
    /// Sets the internal state of the sale item based on provided values.
    /// This includes validation, discount calculation, and total value computation.
    /// </summary>
    /// <param name="productId">The product identifier to assign.</param>
    /// <param name="quantity">The quantity to assign.</param>
    /// <param name="productDetails">The product details for pricing and metadata.</param>
    /// <exception cref="DomainException">Thrown when the quantity is zero or exceeds 20 units.</exception>
    /// <exception cref="DomainException">Thrown when productDetails is null.</exception>
    private void SetValues(Guid productId, int quantity, ProductDetails productDetails)
    {
        ValidateSaleInputs(quantity);

        ProductId = productId;
        Quantity = quantity;

        if (productDetails == null)
            throw new ArgumentNullException(nameof(productDetails));

        ProductDetails = new ProductDetails(
            productDetails.Title,
            productDetails.Category,
            productDetails.Price,
            productDetails.Image
        );

        Discount = CalculateDiscount(quantity);
        UnitPrice = ProductDetails.Price * (1 - Discount);
        TotalAmount = Quantity * UnitPrice;
    }

    /// <summary>
    /// Applies discount rules based on quantity tiers:
    /// - 4 to 9 items: 10%
    /// - 10 to 20 items: 20%
    /// - Less than 4: 0%
    /// </summary>
    /// <param name="quantity">The number of identical products.</param>
    /// <returns>The applicable discount percentage (e.g., 0.10 = 10%).</returns>
    private decimal CalculateDiscount(int quantity)
    {
        if (quantity >= 10 && quantity <= 20)
            return 0.20m;
        if (quantity >= 4)
            return 0.10m;
        return 0.0m;
    }

    /// <summary>
    /// Validates the current sale item using the <see cref="SaleItemValidator"/>.
    /// </summary>
    /// <returns>Returns a <see cref="ValidationResultDetail"/> containing the result of the validation.</returns>
    public ValidationResultDetail Validate()
    {
        var validator = new SaleItemValidator();
        var result = validator.Validate(this);

        return new ValidationResultDetail
        {
            IsValid = result.IsValid,
            Errors = result.Errors.Select(e => (ValidationErrorDetail)e).ToList()
        };
    }

    private static void ValidateSaleInputs(int quantity)
    {
        if (quantity <= 0)
            throw new DomainException("Quantity must be greater than zero.");

        if (quantity > 20)
            throw new DomainException("Cannot sell more than 20 identical items.");
    }
}