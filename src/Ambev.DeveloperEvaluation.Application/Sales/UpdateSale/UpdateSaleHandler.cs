using Ambev.DeveloperEvaluation.Domain.Entities;
using Ambev.DeveloperEvaluation.Domain.Repositories;
using AutoMapper;
using FluentValidation;
using MediatR;

namespace Ambev.DeveloperEvaluation.Application.Sales.UpdateSale;

/// <summary>
/// Handler for processing UpdateSaleCommand requests
/// </summary>
public class UpdateSaleHandler : IRequestHandler<UpdateSaleCommand, UpdateSaleResult>
{
    private readonly ISaleRepository _saleRepository;
    private readonly IMapper _mapper;

    /// <summary>
    /// Initializes a new instance of UpdateSaleHandler
    /// </summary>
    /// <param name="saleRepository">The sale repository</param>
    /// <param name="mapper">The AutoMapper instance</param>
    public UpdateSaleHandler(ISaleRepository saleRepository, IMapper mapper)
    {
        _saleRepository = saleRepository;
        _mapper = mapper;
    }

    /// <summary>
    /// Handles the UpdateSaleCommand request
    /// </summary>
    /// <param name="command">The UpdateSale command</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>The result of the update operation</returns>
    public async Task<UpdateSaleResult> Handle(UpdateSaleCommand command, CancellationToken cancellationToken)
    {
        var validator = new UpdateSaleCommandValidator();
        var validationResult = await validator.ValidateAsync(command, cancellationToken);

        if (!validationResult.IsValid)
            throw new ValidationException(validationResult.Errors);

        var saleToUpdate = await _saleRepository.GetByIdAsync(command.Id);

        if (saleToUpdate == null)
            throw new KeyNotFoundException($"Venda com ID {command.Id} não encontrada.");

        _mapper.Map(command, saleToUpdate); // Map properties from command to entity
        saleToUpdate.UpdateUpdatedAt();
        saleToUpdate.TotalAmount = saleToUpdate.Items.Sum(item => item.Quantity * item.UnitPrice - item.Discount);

        // You might need more sophisticated logic to handle updates to the Items collection
        // (e.g., adding, removing, updating existing items). For now, we're assuming
        // the command's Items will replace the existing ones.
        if (command.Items != null)
        {
            saleToUpdate.Items.Clear();
            foreach (var itemCommand in command.Items)
            {
                saleToUpdate.Items.Add(_mapper.Map<SaleItem>(itemCommand));
            }
            saleToUpdate.TotalAmount = saleToUpdate.Items.Sum(item => item.Quantity * item.UnitPrice - item.Discount);
        }

        var response = await _saleRepository.UpdateAsync(saleToUpdate);

        return new UpdateSaleResult { Sale = response };
    }
}