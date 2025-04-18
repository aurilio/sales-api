using Ambev.DeveloperEvaluation.Messaging.Interfaces;
using Ambev.DeveloperEvaluation.Messaging.LogPublisher;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;

namespace Ambev.DeveloperEvaluation.IoC.ModuleInitializers;

/// <summary>
/// Registers all dependencies related to the messaging infrastructure.
/// </summary>
public class MessageBrokerModuleInitializer : IModuleInitializer
{
    /// <summary>
    /// Configures services required for domain event publishing (e.g., via logs).
    /// </summary>
    /// <param name="builder">The application builder instance.</param>
    public void Initialize(WebApplicationBuilder builder)
    {
        builder.Services.AddSingleton<IDomainEventPublisher, DomainEventLoggerPublisher>();
        builder.Services.AddLogging();
    }
}