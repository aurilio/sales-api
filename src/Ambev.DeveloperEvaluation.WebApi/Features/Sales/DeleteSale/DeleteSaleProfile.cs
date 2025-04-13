using Ambev.DeveloperEvaluation.Application.Sales.DeleteSale;
using AutoMapper;

namespace Ambev.DeveloperEvaluation.WebApi.Features.Sales.DeleteSale;

/// <summary>
/// Profile for mapping between API DeleteSale route parameter and Application DeleteSale command.
/// Also maps between Application DeleteSale result and a potential API response (if needed).
/// </summary>
public class DeleteSaleProfile : Profile
{
    /// <summary>
    /// Initializes the mappings for DeleteSale feature.
    /// </summary>
    public DeleteSaleProfile()
    {
        CreateMap<Guid, DeleteSaleCommand>()
            .ConstructUsing(id => new DeleteSaleCommand(id));
    }
}