using Ambev.DeveloperEvaluation.Domain.Entities;
using Ambev.DeveloperEvaluation.Domain.Repositories;
using Ambev.DeveloperEvaluation.Messaging.Events;
using Ambev.DeveloperEvaluation.Messaging.Interfaces;
using AutoMapper;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Ambev.DeveloperEvaluation.Application.Sales.CreateSale;

/// <summary>
/// Handler for processing CreateSaleCommand requests
/// </summary>
public class CreateSaleHandler : IRequestHandler<CreateSaleCommand, CreateSaleResult>
{
    private readonly ISaleRepository _saleRepository;
    private readonly IMapper _mapper;
    private readonly ILogger<CreateSaleHandler> _logger;
    private readonly IDomainEventPublisher _domainEventPublisher;

    /// <summary>
    /// Initializes a new instance of CreateSaleHandler.
    /// </summary>
    /// <param name="saleRepository">The sale repository.</param>
    /// <param name="mapper">The AutoMapper instance.</param>
    /// <param name="logger">Logger instance for logging operations.</param>
    public CreateSaleHandler(
        ISaleRepository saleRepository,
        IMapper mapper,
        ILogger<CreateSaleHandler> logger,
        IDomainEventPublisher domainEventPublisher)
    {
        _saleRepository = saleRepository;
        _mapper = mapper;
        _logger = logger;
        _domainEventPublisher = domainEventPublisher;
    }

    /// <summary>
    /// Handles the CreateSaleCommand request.
    /// </summary>
    /// <param name="command">The CreateSale command.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The result of the sale creation.</returns>
    public async Task<CreateSaleResult> Handle(CreateSaleCommand command, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Handling CreateSaleCommand for SaleNumber: {SaleNumber}", command.SaleNumber);

        var saleItems = MapSaleItems(command);
        var sale = BuildSale(command, saleItems);

        _logger.LogDebug("Saving sale to repository.");
        var savedSale = await _saleRepository.CreateAsync(sale, cancellationToken);

        var result = _mapper.Map<CreateSaleResult>(savedSale);

        _logger.LogInformation("Sale created successfully. ID: {SaleId}", result.Id);
        await _domainEventPublisher.PublishAsync(new CreatedEvent(sale.Id));

        return result;
    }

    private static List<SaleItem> MapSaleItems(CreateSaleCommand command)
    {
        return command.Items
            .Select(item => new SaleItem(item.ProductId, item.Quantity, item.ProductDetails))
            .ToList();
    }

    private static Sale BuildSale(CreateSaleCommand command, List<SaleItem> saleItems)
    {
        return new Sale(
            saleNumber: command.SaleNumber,
            saleDate: command.SaleDate,
            customerId: command.CustomerId,
            customerName: command.CustomerName,
            branch: command.Branch,
            items: saleItems
        );
    }
}