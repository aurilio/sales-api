using Ambev.DeveloperEvaluation.Application.Sales.ListSales;
using AutoMapper;

namespace Ambev.DeveloperEvaluation.WebApi.Features.Sales.ListSales;

public class ListSaleRequestToQueryConverter : ITypeConverter<ListSaleRequest, ListSaleQuery>
{
    private readonly IHttpContextAccessor _httpContextAccessor;

    public ListSaleRequestToQueryConverter(IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;
    }

    public ListSaleQuery Convert(ListSaleRequest source, ListSaleQuery destination, ResolutionContext context)
    {
        var queryCollection = _httpContextAccessor.HttpContext?.Request?.Query;

        var filters = queryCollection?
            .Where(q => !q.Key.StartsWith("_"))
            .ToDictionary(q => q.Key, q => q.Value.ToString()) ?? new();

        return new ListSaleQuery(source.Page, source.Size, source.OrderBy, filters);
    }
}