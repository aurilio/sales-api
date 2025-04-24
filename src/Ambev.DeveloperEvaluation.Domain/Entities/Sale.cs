using Ambev.DeveloperEvaluation.Common.Validation;
using Ambev.DeveloperEvaluation.Domain.Common;
using Ambev.DeveloperEvaluation.Domain.Validation;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

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

    [ConcurrencyCheck]
    [DatabaseGenerated(DatabaseGeneratedOption.Computed)]
    [Column("xmin", TypeName = "xid")]
    public uint Version { get; private set; }

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
    public Sale(string saleNumber, DateTime saleDate, Guid customerId, string customerName, string branch)
    {
        ValidateSaleInputs(saleNumber, customerId, branch);
        SetValues(saleNumber, saleDate, customerId, customerName, branch);

        CreatedAt = DateTime.UtcNow;
        CalculateTotalAmount();
    }

    private void SetValues(string saleNumber, DateTime saleDate, Guid customerId, string customerName, string branch)
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
    public void UpdateSale(string saleNumber, DateTime saleDate, Guid customerId, string customerName, string branch, bool isCancelled)
    {
        ValidateSaleInputs(saleNumber, customerId, branch);
        SetValues(saleNumber, saleDate, customerId, customerName, branch);

        if (isCancelled)
            IsCancelled = true;

        UpdatedAt = DateTime.UtcNow;
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

    private static void ValidateSaleInputs(string saleNumber, Guid customerId, string branch)
    {
        if (string.IsNullOrWhiteSpace(saleNumber))
            throw new ArgumentException("Sale number is required.", nameof(saleNumber));

        if (customerId == Guid.Empty)
            throw new ArgumentException("CustomerId is required.", nameof(customerId));

        if (string.IsNullOrWhiteSpace(branch))
            throw new ArgumentException("Branch is required.", nameof(branch));
    }

    public void AddItem(SaleItem item)
    {
        if (item == null)
            throw new ArgumentNullException(nameof(item));

        if (_items.Any(i => i.ProductId == item.ProductId))
            throw new DomainException("Item with this product already exists.");

        _items.Add(item);
        CalculateTotalAmount();
    }

    public void RemoveItem(Guid itemId)
    {
        var item = Items.FirstOrDefault(i => i.Id == itemId);
        if (item != null)
        {
            _items.Remove(item);
        }
    }

    public void SyncItems(List<SaleItem> updatedItems)
    {
        // Atualiza ou adiciona
        foreach (var updatedItem in updatedItems)
        {
            var existing = Items.FirstOrDefault(i => i.Id == updatedItem.Id);
            if (existing != null)
            {

                existing.Update(updatedItem.ProductId, updatedItem.Quantity, updatedItem.ProductDetails);
            }
            else
                AddItem(updatedItem);
        }

        var updatedIds = updatedItems.Select(x => x.Id).ToHashSet();
        var toRemove = Items.Where(i => !updatedIds.Contains(i.Id)).ToList();
        foreach (var item in toRemove)
        {
            RemoveItem(item.Id);
        }
    }

    public void RemoveMissingItems(IEnumerable<Guid> remainingIds)
    {
        _items.RemoveAll(i => !remainingIds.Contains(i.Id));
        CalculateTotalAmount();
    }
}