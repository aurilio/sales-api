namespace Ambev.DeveloperEvaluation.Domain.Entities;

/// <summary>
/// Represents a customer.
/// </summary>
public class Customer
{
    /// <summary>
    /// The unique identifier of the customer (External Identity).
    /// </summary>
    public Guid Id { get; set; }

    /// <summary>
    /// The full name of the customer.
    /// </summary>
    public string Name { get; set; } = string.Empty;
}