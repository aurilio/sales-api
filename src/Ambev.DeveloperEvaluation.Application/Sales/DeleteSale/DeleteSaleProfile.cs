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
        // You might not need a direct map from DeleteSaleCommand to Sale for deletion,
        // as the ID is usually enough. However, if your DeleteSaleCommand contains
        // properties you want to map (e.g., for logging), you can add it here.
        // CreateMap<DeleteSaleCommand, Sale>();

        CreateMap<Sale, DeleteSaleResponse>();
    }
}