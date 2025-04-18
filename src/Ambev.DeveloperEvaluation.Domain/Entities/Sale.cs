using Ambev.DeveloperEvaluation.Common.Validation;
using Ambev.DeveloperEvaluation.Domain.Common;
using Ambev.DeveloperEvaluation.Domain.Validation;

namespace Ambev.DeveloperEvaluation.Domain.Entities;

/// <summary>
/// Represents a sale transaction in the system, including customer, items, and metadata.
/// </summary>
public class Sale : BaseEntity
{
    /// <summary>
    /// Unique identifier code for the sale.
    /// </summary>
    public string SaleNumber { get; set; }

    /// <summary>
    /// Date when the sale was made.
    /// </summary>
    public DateTime SaleDate { get; set; }

    /// <summary>
    /// Identifier of the customer who made the purchase.
    /// </summary>
    public Guid CustomerId { get; set; }

    /// <summary>
    /// Denormalized name of the customer (for reporting or display).
    /// </summary>
    public string CustomerName { get; set; }

    /// <summary>
    /// Total value of the sale, automatically calculated from items.
    /// </summary>
    public decimal TotalAmount { get; private set; }

    /// <summary>
    /// Branch or store where the sale took place.
    /// </summary>
    public string Branch { get; set; }

    /// <summary>
    /// Indicates whether the sale has been cancelled.
    /// </summary>
    public bool IsCancelled { get; private set; } = false;

    /// <summary>
    /// Date and time when this sale was created.
    /// </summary>
    public DateTime CreatedAt { get; private set; }

    /// <summary>
    /// Date and time when the sale was last updated.
    /// </summary>
    public DateTime? UpdatedAt { get; private set; }

    /// <summary>
    /// Items included in this sale.
    /// </summary>
    private readonly List<SaleItem> _items = new();
    public IReadOnlyCollection<SaleItem> Items => _items;

    /// <summary>
    /// Constructor for Entity Framework.
    /// </summary>
    protected Sale() { }

    /// <summary>
    /// Initializes a new sale instance with required information and items.
    /// </summary>
    /// <param name="saleNumber">Sale code or identifier.</param>
    /// <param name="saleDate">Date of the sale.</param>
    /// <param name="customerId">Customer’s unique identifier.</param>
    /// <param name="branch">Branch name where the sale occurred.</param>
    /// <param name="items">List of products included in the sale.</param>
    public Sale(string saleNumber, DateTime saleDate, Guid customerId, string customerName, string branch, IEnumerable<SaleItem> items)
    {
        ValidateSaleInputs(saleNumber, customerId, branch, items);
        SetValues(saleNumber, saleDate, customerId, customerName, branch, items);

        CreatedAt = DateTime.UtcNow;
        _items.AddRange(items);
        CalculateTotalAmount();
    }

    private void SetValues(string saleNumber, DateTime saleDate, Guid customerId, string customerName, string branch, IEnumerable<SaleItem> items)
    {
        SaleNumber = saleNumber;
        SaleDate = saleDate;
        CustomerId = customerId;
        CustomerName = customerName;
        Branch = branch;
    }

    /// <summary>
    /// Updates the basic information of the sale such as number, date, customer, and branch.
    /// Automatically updates the <see cref="UpdatedAt"/> timestamp.
    /// </summary>
    /// <param name="saleNumber">The updated sale number.</param>
    /// <param name="saleDate">The updated sale date.</param>
    /// <param name="customerId">The updated customer ID.</param>
    /// <param name="customerName">The updated customer name.</param>
    /// <param name="branch">The updated branch where the sale occurred.</param>
    public void UpdateSale(string saleNumber, DateTime saleDate, Guid customerId, string customerName, string branch, bool isCancelled, IEnumerable<SaleItem> items)
    {
        ValidateSaleInputs(saleNumber, customerId, branch, items);
        SetValues(saleNumber, saleDate, customerId, customerName, branch, items);

        if (isCancelled)
            IsCancelled = true;

        ReplaceItems(items);
        UpdatedAt = DateTime.UtcNow;
    }

    /// <summary>
    /// Replaces all existing sale items with a new list and recalculates the total amount.
    /// This is typically used during updates where the full item list is replaced.
    /// </summary>
    /// <param name="items">The new list of sale items.</param>
    /// <exception cref="DomainException">Thrown if the newItems list is null or empty.</exception>
    public void ReplaceItems(IEnumerable<SaleItem> updatedItems)
    {
        if (updatedItems == null || !updatedItems.Any())
            throw new DomainException("A sale must have at least one item.");

        _items.Clear();
        _items.AddRange(updatedItems);

        CalculateTotalAmount();
    }

    /// <summary>
    /// Validates the sale and its items using defined rules.
    /// </summary>
    /// <returns>Result of the validation process.</returns>
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
                    result.Errors.Add(new FluentValidation.Results.ValidationFailure(
                        $"Items[{Items.ToList().IndexOf(item)}].{error.Error}",
                        error.Detail
                    ));
                }
            }
        }

        return new ValidationResultDetail
        {
            IsValid = result.IsValid && !result.Errors.Any(),
            Errors = result.Errors.Select(e => (ValidationErrorDetail)e).ToList()
        };
    }

    /// <summary>
    /// Recalculates the total amount based on all items and updates the last modified timestamp.
    /// </summary>
    private void CalculateTotalAmount()
    {
        TotalAmount = _items.Sum(i => i.TotalAmount);
    }

    private static void ValidateSaleInputs(string saleNumber, Guid customerId, string branch, IEnumerable<SaleItem> items)
    {
        if (string.IsNullOrWhiteSpace(saleNumber))
            throw new ArgumentException("Sale number is required.", nameof(saleNumber));

        if (customerId == Guid.Empty)
            throw new ArgumentException("CustomerId is required.", nameof(customerId));

        if (string.IsNullOrWhiteSpace(branch))
            throw new ArgumentException("Branch is required.", nameof(branch));

        if (items == null || !items.Any())
            throw new DomainException("A sale must have at least one item.");
    }
}