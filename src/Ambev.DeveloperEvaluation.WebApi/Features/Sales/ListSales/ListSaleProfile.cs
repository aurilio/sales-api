using Ambev.DeveloperEvaluation.Application.Sales.ListSales;
using Ambev.DeveloperEvaluation.Common.Pagination;
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
    private readonly AutoMapper.IConfigurationProvider _configurationProvider;

    /// <summary>
    /// Initializes the mappings for ListSale feature.
    /// Parameterless constructor required by AutoMapper.
    /// </summary>
    public ListSaleProfile()
    {
        CreateMap<ListSaleRequest, ListSaleQuery>();

        CreateMap<Sale, GetSaleResponse>()
            .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
            .ForMember(dest => dest.SaleNumber, opt => opt.MapFrom(src => src.SaleNumber))
            .ForMember(dest => dest.SaleDate, opt => opt.MapFrom(src => src.SaleDate))
            .ForMember(dest => dest.CustomerId, opt => opt.MapFrom(src => src.CustomerId))
            .ForMember(dest => dest.CustomerName, opt => opt.MapFrom(src => src.CustomerName))
            .ForMember(dest => dest.TotalAmount, opt => opt.MapFrom(src => src.TotalAmount))
            .ForMember(dest => dest.Branch, opt => opt.MapFrom(src => src.Branch))
            .ForMember(dest => dest.IsCancelled, opt => opt.MapFrom(src => src.IsCancelled))
            .ForMember(dest => dest.CreatedAt, opt => opt.MapFrom(src => src.CreatedAt))
            .ForMember(dest => dest.UpdatedAt, opt => opt.MapFrom(src => src.UpdatedAt))
            .ForMember(dest => dest.Items, opt => opt.MapFrom(src => src.Items.Select(item => new SaleItemResponse
            {
                Id = item.Id,
                ProductId = item.ProductId,
                ProductName = item.ProductName,
                Quantity = item.Quantity,
                UnitPrice = item.UnitPrice,
                Discount = item.Discount,
                TotalAmount = item.TotalAmount,
                CreatedAt = item.CreatedAt,
                UpdatedAt = item.UpdatedAt
            })));
    }

    /// <summary>
    /// Constructor that accepts IConfigurationProvider for more complex mappings.
    /// </summary>
    public ListSaleProfile(AutoMapper.IConfigurationProvider configurationProvider) : this() // Chama o construtor sem parâmetros primeiro
    {
        _configurationProvider = configurationProvider;

        CreateMap<PaginatedList<Sale>, IEnumerable<GetSaleResponse>>()
            .ConvertUsing(list => list.Select(sale => _configurationProvider.CreateMapper().Map<GetSaleResponse>(sale)));
    }
}