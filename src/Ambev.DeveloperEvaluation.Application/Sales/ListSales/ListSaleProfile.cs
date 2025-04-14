using Ambev.DeveloperEvaluation.Domain.Entities;
using AutoMapper;

namespace Ambev.DeveloperEvaluation.Application.Sales.ListSales;

/// <summary>
/// Profile for mapping between ListSaleQuery and potentially any filtering parameters,
/// and Sale entity to a simple Sale DTO for listing (if needed).
/// </summary>
public class ListSaleProfile : Profile
{
    /// <summary>
    /// Initializes the mappings for ListSale operation.
    /// </summary>
    public ListSaleProfile()
    {
        // You might map from ListSaleQuery to some filter object if you have one.
        // CreateMap<ListSaleQuery, SaleFilter>();

        // If you need a simplified Sale DTO for listing, map it here.
        //CreateMap<Sale, ListSaleDto>(); // Assuming you create a ListSaleDto
        //CreateMap<SaleItem, ListSaleItemDto>(); // Assuming you create a ListSaleItemDto
    }
}