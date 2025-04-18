using Ambev.DeveloperEvaluation.Common.Exceptions;
using Ambev.DeveloperEvaluation.Domain.Repositories;
using AutoMapper;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Ambev.DeveloperEvaluation.Application.Sales.GetSale;

/// <summary>
/// Handler for processing GetSaleQuery requests
/// </summary>
public class GetSaleHandler : IRequestHandler<GetSaleQuery, GetSaleResult>
{
    private readonly ISaleRepository _saleReadRepository;
    private readonly IMapper _mapper;
    private readonly ILogger<GetSaleHandler> _logger;

    /// <summary>
    /// Initializes a new instance of GetSaleHandler
    /// </summary>
    /// <param name="saleReadRepository">The sale read repository</param>
    /// <param name="mapper">The AutoMapper instance</param>
    /// <param name="logger">The logger instance</param>
    public GetSaleHandler(ISaleRepository saleReadRepository, IMapper mapper, ILogger<GetSaleHandler> logger)
    {
        _saleReadRepository = saleReadRepository;
        _mapper = mapper;
        _logger = logger;
    }

    /// <summary>
    /// Handles the GetSaleQuery request
    /// </summary>
    /// <param name="request">The GetSale query</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>The sale details if found</returns>
    public async Task<GetSaleResult> Handle(GetSaleQuery request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Starting GetSaleQuery handler for SaleId: {SaleId}", request.Id);

        _logger.LogDebug("Fetching sale with ID: {SaleId}", request.Id);
        var sale = await _saleReadRepository.GetByIdAsync(request.Id, cancellationToken);

        if (sale == null)
        {
            _logger.LogWarning("Sale with ID {SaleId} not found.", request.Id);
            throw new NotFoundException("Sale", request.Id);
        }

        _logger.LogInformation("Successfully retrieved sale with ID: {SaleId}", request.Id);
        var result = _mapper.Map<GetSaleResult>(sale);

        return result;
    }
}