using Ambev.DeveloperEvaluation.Common.Pagination;
using Ambev.DeveloperEvaluation.Domain.Entities;
using Ambev.DeveloperEvaluation.Domain.Repositories;
using AutoMapper;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Ambev.DeveloperEvaluation.Application.Sales.ListSales;

/// <summary>
/// Handler for processing ListSaleQuery requests
/// </summary>
public class ListSaleHandler : IRequestHandler<ListSaleQuery, PaginatedList<Sale>>
{
    private readonly ISaleRepository _saleReadRepository;
    private readonly IMapper _mapper;
    private readonly ILogger<ListSaleHandler> _logger;

    /// <summary>
    /// Initializes a new instance of <see cref="ListSaleHandler"/>.
    /// </summary>
    /// <param name="saleReadRepository">The sale read repository.</param>
    /// <param name="mapper">The AutoMapper instance.</param>
    /// <param name="logger">The logger instance.</param>
    public ListSaleHandler(ISaleRepository saleReadRepository, IMapper mapper, ILogger<ListSaleHandler> logger)
    {
        _saleReadRepository = saleReadRepository;
        _mapper = mapper;
        _logger = logger;
    }

    /// <summary>
    /// Handles the <see cref="ListSaleQuery"/> request.
    /// </summary>
    /// <param name="request">The query with pagination, ordering, and filters.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>A paginated list of sales matching the query parameters.</returns>
    public async Task<PaginatedList<Sale>> Handle(ListSaleQuery request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Listing sales: Page={Page}, Size={Size}, OrderBy={OrderBy}, Filters={FiltersCount}",
            request.Page, request.Size, request.OrderBy ?? "default", request.Filters?.Count ?? 0);

        var paginatedResult = await _saleReadRepository.ListAsync(request.Page, request.Size, request.OrderBy, request.Filters, cancellationToken);

        _logger.LogInformation("Sales listed successfully. TotalItems={TotalCount}, CurrentPage={CurrentPage}",
            paginatedResult.TotalCount, paginatedResult.CurrentPage);

        return paginatedResult;
    }
}