using Ambev.DeveloperEvaluation.Common.Pagination;
using Ambev.DeveloperEvaluation.Domain.Entities;
using Ambev.DeveloperEvaluation.Domain.Repositories;
using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ambev.DeveloperEvaluation.Application.Sales.ListSales;

/// <summary>
/// Handler for processing ListSaleQuery requests
/// </summary>
public class ListSaleHandler : IRequestHandler<ListSaleQuery, PaginatedList<Sale>>
{
    private readonly ISaleRepository _saleReadRepository;
    private readonly IMapper _mapper;

    /// <summary>
    /// Initializes a new instance of ListSaleHandler
    /// </summary>
    /// <param name="saleReadRepository">The sale read repository</param>
    /// <param name="mapper">The AutoMapper instance</param>
    public ListSaleHandler(ISaleRepository saleReadRepository, IMapper mapper)
    {
        _saleReadRepository = saleReadRepository;
        _mapper = mapper;
    }

    /// <summary>
    /// Handles the ListSaleQuery request
    /// </summary>
    /// <param name="request">The ListSale query</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>A paginated list of sales</returns>
    public async Task<PaginatedList<Sale>> Handle(ListSaleQuery request, CancellationToken cancellationToken)
    {
        var query = _saleReadRepository.GetAllQueryable();

        if (!string.IsNullOrWhiteSpace(request.OrderBy))
        {
            // Example of basic ordering (adjust based on your needs and allowed properties)
            query = query.OrderBy(s => EF.Property<object>(s, request.OrderBy));
        }
        else
        {
            query = query.OrderByDescending(s => s.SaleDate); // Default ordering
        }

        return await PaginatedList<Sale>.CreateAsync(query, request.Page, request.Size);
    }
}