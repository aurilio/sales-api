using Ambev.DeveloperEvaluation.Application.Sales.UpdateSale;
using Ambev.DeveloperEvaluation.Domain.ValueObjects;
using Ambev.DeveloperEvaluation.WebApi.Features.Sales.CreateSale;
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
        CreateMap<UpdateSaleRequest, UpdateSaleCommand>();
        CreateMap<SaleItemRequest, UpdateSaleItemCommand>()
            .ForMember(dest => dest.ProductDetails, opt => opt.MapFrom(src => new ProductDetails(
                src.ProductDetails.Title,
                src.ProductDetails.Category,
                src.ProductDetails.Price,
                src.ProductDetails.Image
            )));

        CreateMap<UpdateSaleResult, UpdateSaleResponse>();
        CreateMap<UpdateSaleItemResult, SaleItemResponse>();
        CreateMap<ProductDetails, ProductDetailsResponse>();

        CreateMap<UpdateSaleItemRequest, UpdateSaleItemCommand>();

        CreateMap<ProductDetailsRequest, ProductDetails>()
        .ConstructUsing(src => new ProductDetails(
            src.Title,
            src.Category,
            src.Price,
            src.Image
        ));
    }
}