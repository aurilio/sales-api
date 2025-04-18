using Ambev.DeveloperEvaluation.Messaging.Common;

namespace Ambev.DeveloperEvaluation.Messaging.Interfaces;

/// <summary>
/// Interface for publishing domain events to external systems (e.g., logs, brokers).
/// </summary>
public interface IDomainEventPublisher
{
    /// <summary>
    /// Publishes a domain event.
    /// </summary>
    /// <param name="domainEvent">The domain event to publish.</param>
    Task PublishAsync(DomainEventBase domainEvent);
}