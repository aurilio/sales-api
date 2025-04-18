﻿using Ambev.DeveloperEvaluation.Common.Pagination;
using Ambev.DeveloperEvaluation.Domain.Entities;
using MediatR;

namespace Ambev.DeveloperEvaluation.Application.Sales.ListSales;

public class ListSaleQuery : IRequest<PaginatedList<Sale>>
{
    public int Page { get; set; }
    public int Size { get; set; }
    public string? OrderBy { get; set; }
    public Dictionary<string, string>? Filters { get; set; }

    public ListSaleQuery(int page, int size, string? orderBy, Dictionary<string, string>? filters = null)
    {
        Page = page;
        Size = size;
        OrderBy = orderBy;
        Filters = filters ?? new();
    }
}