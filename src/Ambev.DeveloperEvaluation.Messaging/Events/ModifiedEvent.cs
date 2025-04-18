using Ambev.DeveloperEvaluation.Messaging.Common;

namespace Ambev.DeveloperEvaluation.Messaging.Events;

/// <summary>
/// Event raised when a sale is modified.
/// </summary>
public class ModifiedEvent : DomainEventBase
{
    public ModifiedEvent(Guid aggregateId, string? correlationId = null)
        : base(aggregateId, correlationId)
    {
    }
}