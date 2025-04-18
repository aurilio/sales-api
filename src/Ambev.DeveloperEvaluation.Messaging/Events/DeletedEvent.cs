using Ambev.DeveloperEvaluation.Messaging.Common;

namespace Ambev.DeveloperEvaluation.Messaging.Events;

/// <summary>
/// Event raised when a sale item is deleted or a cancellation action occurs.
/// </summary>
public class DeletedEvent : DomainEventBase
{
    public DeletedEvent(Guid aggregateId, string? correlationId = null)
        : base(aggregateId, correlationId)
    {
    }
}