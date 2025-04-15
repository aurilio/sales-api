using Ambev.DeveloperEvaluation.Application.Sales.UpdateSale;
using Ambev.DeveloperEvaluation.WebApi.Features.Sales.CreateSale;
using Ambev.DeveloperEvaluation.WebApi.Features.Sales.GetSale;
using AutoMapper;

namespace Ambev.DeveloperEvaluation.WebApi.Features.Sales.UpdateSale;

/// <summary>
/// Profile for mapping between API UpdateSale request and Application UpdateSale command.
/// Also maps between Application UpdateSale result and a potential API response (if needed).
/// </summary>
public class UpdateSaleProfile : Profile
{
    /// <summary>
    /// Initializes the mappings for UpdateSale feature.
    /// </summary>
    public UpdateSaleProfile()
    {
        CreateMap<UpdateSaleRequest, UpdateSaleCommand>()
            .ForMember(dest => dest.Items, opt => opt.MapFrom(src => src.Items));

        CreateMap<SaleItemRequest, UpdateSaleItemCommand>();

        CreateMap<UpdateSaleItemRequest, UpdateSaleItemCommand>()
            .ForMember(dest => dest.ProductId, opt => opt.MapFrom(src => src.ProductId))
            .ForMember(dest => dest.Quantity, opt => opt.MapFrom(src => src.Quantity))
            .ForMember(dest => dest.UnitPrice, opt => opt.MapFrom(src => src.UnitPrice))
            .ForMember(dest => dest.Discount, opt => opt.MapFrom(src => src.Discount));

        CreateMap<UpdateSaleResult, UpdateSaleResponse>()
            .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Sale.Id))
            .ForMember(dest => dest.IsCancelled, opt => opt.MapFrom(src => src.Sale.IsCancelled));
    }
}