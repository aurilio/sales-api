using Ambev.DeveloperEvaluation.Messaging.Common;

namespace Ambev.DeveloperEvaluation.Messaging.Events;

/// <summary>
/// Event raised when a sale is created.
/// </summary>
public class CreatedEvent : DomainEventBase
{
    public CreatedEvent(Guid aggregateId, string? correlationId = null)
        : base(aggregateId, correlationId)
    {
    }
}
