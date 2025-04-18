using Ambev.DeveloperEvaluation.Domain.Entities;
using AutoMapper;

namespace Ambev.DeveloperEvaluation.Application.Sales.DeleteSale;

/// <summary>
/// Profile for mapping between DeleteSaleCommand and Sale entity (for retrieval purposes), and Sale entity to DeleteSaleResult.
/// </summary>
public class DeleteSaleProfile : Profile
{
    /// <summary>
    /// Initializes the mappings for DeleteSale operation.
    /// </summary>
    public DeleteSaleProfile()
    {
        CreateMap<Sale, DeleteSaleResponse>();
    }
}