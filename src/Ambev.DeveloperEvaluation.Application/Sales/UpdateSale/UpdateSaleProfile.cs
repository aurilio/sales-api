using Ambev.DeveloperEvaluation.Domain.Entities;
using AutoMapper;

namespace Ambev.DeveloperEvaluation.Application.Sales.UpdateSale;

/// <summary>
/// Profile for mapping between UpdateSaleCommand and Sale entity, and Sale entity to UpdateSaleResult.
/// </summary>
public class UpdateSaleProfile : Profile
{
    /// <summary>
    /// Initializes the mappings for UpdateSale operation.
    /// </summary>
    public UpdateSaleProfile()
    {
        CreateMap<UpdateSaleCommand, Sale>()
            .ForMember(dest => dest.Items, opt => opt.MapFrom(src => src.Items)); // Map the collection of SaleItem commands

        CreateMap<UpdateSaleItemCommand, SaleItem>();

        CreateMap<Sale, UpdateSaleResult>();
    }
}