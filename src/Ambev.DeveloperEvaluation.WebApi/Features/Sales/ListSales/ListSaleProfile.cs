using Ambev.DeveloperEvaluation.Application.Sales.ListSales;
using AutoMapper;

namespace Ambev.DeveloperEvaluation.WebApi.Features.Sales.ListSales;

/// <summary>
/// Profile for mapping between API ListSale request and Application ListSales query.
/// Also maps between Application PaginatedList of Sale entities and a potential API response (though this might be done directly in the controller).
/// </summary>
public class ListSaleProfile : Profile
{
    /// <summary>
    /// Initializes the mappings for ListSale feature.
    /// </summary>
    public ListSaleProfile()
    {
        CreateMap<ListSaleRequest, ListSaleQuery>();
    }
}