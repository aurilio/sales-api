using Ambev.DeveloperEvaluation.Messaging.Common;
using Ambev.DeveloperEvaluation.Messaging.Interfaces;
using Microsoft.Extensions.Logging;

namespace Ambev.DeveloperEvaluation.Messaging.LogPublisher;

/// <summary>
/// Publishes domain events by logging them, simulating integration with external systems.
/// </summary>
public class DomainEventLoggerPublisher : IDomainEventPublisher
{
    private readonly ILogger<DomainEventLoggerPublisher> _logger;

    /// <summary>
    /// Initializes a new instance of the <see cref="DomainEventLoggerPublisher"/> class.
    /// </summary>
    /// <param name="logger">The logger instance used to write event data.</param>
    public DomainEventLoggerPublisher(ILogger<DomainEventLoggerPublisher> logger)
    {
        _logger = logger;
    }

    /// <inheritdoc />
    public Task PublishAsync(DomainEventBase domainEvent)
    {
        var eventType = domainEvent.GetType().Name;
        var logMessage = new
        {
            Event = eventType,
            domainEvent.AggregateId,
            domainEvent.OccurredOn,
            domainEvent.CorrelationId
        };

        _logger.LogInformation("📢 Domain event published: {@Event}", logMessage);
        return Task.CompletedTask;
    }
}