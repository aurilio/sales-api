using Ambev.DeveloperEvaluation.Common.Exceptions;
using Ambev.DeveloperEvaluation.Domain.Repositories;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Ambev.DeveloperEvaluation.Application.Sales.DeleteSale;

/// <summary>
/// Handler for processing DeleteSaleCommand requests
/// </summary>
public class DeleteSaleHandler : IRequestHandler<DeleteSaleCommand, DeleteSaleResponse>
{
    private readonly ISaleRepository _saleRepository;
    private readonly ILogger<DeleteSaleHandler> _logger;

    /// <summary>
    /// Initializes a new instance of DeleteSaleHandler
    /// </summary>
    /// <param name="saleRepository">The sale repository</param>
    public DeleteSaleHandler(ISaleRepository saleRepository, ILogger<DeleteSaleHandler> logger)
    {
        _saleRepository = saleRepository;
        _logger = logger;
    }

    /// <summary>
    /// Handles the DeleteSaleCommand request
    /// </summary>
    /// <param name="request">The DeleteSale command</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>The result of the delete operation</returns>
    public async Task<DeleteSaleResponse> Handle(DeleteSaleCommand request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Starting DeleteSaleCommand handler for SaleId: {SaleId}", request.Id);

        _logger.LogDebug("Attempting to delete sale with ID: {SaleId}", request.Id);
        var success = await _saleRepository.DeleteAsync(request.Id, cancellationToken);

        if (!success)
        {
            _logger.LogWarning("Sale with ID {SaleId} was not found.", request.Id);
            throw new NotFoundException("Sale", request.Id);
        }

        _logger.LogInformation("Sale with ID {SaleId} successfully deleted.", request.Id);
        return new DeleteSaleResponse { Message = $"Sale with ID {request.Id} was deleted successfully" };
    }
}