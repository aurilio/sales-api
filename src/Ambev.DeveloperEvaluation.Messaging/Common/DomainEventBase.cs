namespace Ambev.DeveloperEvaluation.Messaging.Common;

/// <summary>
/// Represents the base structure for all domain events.
/// Includes common metadata such as occurrence time and identifiers.
/// </summary>
public abstract class DomainEventBase
{
    /// <summary>
    /// The moment in time when the domain event occurred.
    /// </summary>
    public DateTime OccurredOn { get; } = DateTime.UtcNow;

    /// <summary>
    /// The unique identifier of the aggregate that originated the event.
    /// </summary>
    public Guid AggregateId { get; init; }

    /// <summary>
    /// Correlation ID used to track a set of related operations.
    /// </summary>
    public string? CorrelationId { get; init; }

    /// <summary>
    /// Base constructor requiring the aggregate ID.
    /// </summary>
    /// <param name="aggregateId">The unique identifier of the aggregate root.</param>
    /// <param name="correlationId">An optional correlation identifier for tracing the event flow.</param>
    protected DomainEventBase(Guid aggregateId, string? correlationId = null)
    {
        AggregateId = aggregateId;
        CorrelationId = correlationId;
    }
}