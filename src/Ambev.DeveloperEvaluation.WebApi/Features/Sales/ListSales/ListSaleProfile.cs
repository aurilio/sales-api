using Ambev.DeveloperEvaluation.Application.Sales.ListSales;
using Ambev.DeveloperEvaluation.Domain.Entities;
using Ambev.DeveloperEvaluation.WebApi.Features.Sales.CreateSale;
using Ambev.DeveloperEvaluation.WebApi.Features.Sales.GetSale;
using AutoMapper;

namespace Ambev.DeveloperEvaluation.WebApi.Features.Sales.ListSales;

/// <summary>
/// Profile for mapping between API ListSale request and Application ListSales query.
/// Also maps between Application PaginatedList of Sale entities and a potential API response (though this might be done directly in the controller).
/// </summary>
public class ListSaleProfile : Profile
{
    public ListSaleProfile()
    {
        CreateMap<ListSaleRequest, ListSaleQuery>()
            .ConvertUsing<ListSaleRequestToQueryConverter>();

        CreateMap<SaleItem, SaleItemResponse>();

        CreateMap<Sale, GetSaleResponse>()
            .ForMember(dest => dest.Items, opt => opt.MapFrom(src => src.Items));
    }
}