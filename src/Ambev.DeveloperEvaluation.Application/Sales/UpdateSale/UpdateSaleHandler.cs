﻿using Ambev.DeveloperEvaluation.Common.Exceptions;
using Ambev.DeveloperEvaluation.Domain.Entities;
using Ambev.DeveloperEvaluation.Domain.Repositories;
using Ambev.DeveloperEvaluation.Messaging.Events;
using Ambev.DeveloperEvaluation.Messaging.Interfaces;
using AutoMapper;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Ambev.DeveloperEvaluation.Application.Sales.UpdateSale;

/// <summary>
/// Handler for processing UpdateSaleCommand requests
/// </summary>
public class UpdateSaleHandler : IRequestHandler<UpdateSaleCommand, UpdateSaleResult>
{
    private readonly ISaleRepository _saleRepository;
    private readonly IMapper _mapper;
    private readonly ILogger<UpdateSaleHandler> _logger;
    private readonly IDomainEventPublisher _domainEventPublisher;

    /// <summary>
    /// Initializes a new instance of <see cref="UpdateSaleHandler"/>.
    /// </summary>
    /// <param name="saleRepository">The sale repository.</param>
    /// <param name="mapper">The AutoMapper instance.</param>
    /// <param name="logger">The logger instance.</param>
    public UpdateSaleHandler(
        ISaleRepository saleRepository,
        IMapper mapper,
        ILogger<UpdateSaleHandler> logger,
        IDomainEventPublisher domainEventPublisher)
    {
        _saleRepository = saleRepository;
        _mapper = mapper;
        _logger = logger;
        _domainEventPublisher = domainEventPublisher;
    }

    /// <summary>
    /// Handles the UpdateSaleCommand request
    /// </summary>
    /// <param name="command">The UpdateSale command</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>The result of the update operation</returns>
    public async Task<UpdateSaleResult> Handle(UpdateSaleCommand command, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Starting UpdateSaleCommand handler for Sale ID: {SaleId}", command.Id);

        if (command.Items == null || command.Items.Count() <= 0)
            throw new DomainException("A sale must have at least one item.");

        var saleToUpdate = await _saleRepository.GetByIdAsync(command.Id, cancellationToken);

        if (saleToUpdate == null)
        {
            _logger.LogWarning("Sale with ID {SaleId} not found.", command.Id);
            throw new NotFoundException("Sale", command.Id);
        }

        var updatedItemIds = command.Items
            .Where(i => i.Id.HasValue)
            .Select(i => i.Id!.Value)
            .ToList();

        saleToUpdate.RemoveMissingItems(updatedItemIds);

        foreach (var updatedItem in command.Items)
        {
            var existingItem = saleToUpdate.Items.FirstOrDefault(i => i.Id == updatedItem.Id);

            if (existingItem != null)
            {
                existingItem.Update(
                    updatedItem.ProductId,
                    updatedItem.Quantity,
                    updatedItem.ProductDetails
                );
            }
            else
            {
                var newItem = new SaleItem(
                    Guid.Empty,
                    saleToUpdate.Id,
                    updatedItem.ProductId,
                    updatedItem.Quantity,
                    updatedItem.ProductDetails
                );

                saleToUpdate.AddItem(newItem);
            }
        }

        saleToUpdate.UpdateSale(
            command.SaleNumber,
            command.SaleDate,
            command.CustomerId,
            command.CustomerName,
            command.Branch,
            command.IsCancelled
        );

        saleToUpdate.SaleDate = DateTime.SpecifyKind(saleToUpdate.SaleDate, DateTimeKind.Utc);

        _logger.LogDebug("Persisting updated sale to repository...");
        var updatedSale = await _saleRepository.UpdateAsync(saleToUpdate, cancellationToken);

        if (command.IsCancelled)
            await _domainEventPublisher.PublishAsync(new CancelledEvent(updatedSale.Id));

        _logger.LogInformation("Sale updated successfully with ID: {SaleId}", updatedSale.Id);
        await _domainEventPublisher.PublishAsync(new ModifiedEvent(updatedSale.Id));

        return _mapper.Map<UpdateSaleResult>(updatedSale);
    }
}