using Ambev.DeveloperEvaluation.Messaging.Common;

namespace Ambev.DeveloperEvaluation.Messaging.Events;

/// <summary>
/// Event raised when a sale is cancelled.
/// </summary>
public class CancelledEvent : DomainEventBase
{
    public CancelledEvent(Guid aggregateId, string? correlationId = null)
        : base(aggregateId, correlationId)
    {
    }
}