using Ambev.DeveloperEvaluation.Application.Sales.CreateSale;
using Ambev.DeveloperEvaluation.Domain.ValueObjects;
using AutoMapper;

namespace Ambev.DeveloperEvaluation.WebApi.Features.Sales.CreateSale;

/// <summary>
/// Profile for mapping between API CreateSale request and Application CreateSale command.
/// Also maps between Application CreateSale result and a potential API response (if needed).
/// </summary>
public class CreateSaleProfile : Profile
{
    /// <summary>
    /// Initializes the mappings for CreateSale feature.
    /// </summary>
    public CreateSaleProfile()
    {
        CreateMap<CreateSaleRequest, CreateSaleCommand>();
        CreateMap<SaleItemRequest, CreateSaleItemCommand>()
            .ForMember(dest => dest.ProductDetails, opt => opt.MapFrom(src => new ProductDetails(
                src.ProductDetails.Title,
                src.ProductDetails.Category,
                src.ProductDetails.Price,
                src.ProductDetails.Image
            )));

        CreateMap<CreateSaleResult, CreateSaleResponse>();
        CreateMap<CreateSaleItemResult, SaleItemResponse>();
        CreateMap<ProductDetails, ProductDetailsResponse>();
    }
}