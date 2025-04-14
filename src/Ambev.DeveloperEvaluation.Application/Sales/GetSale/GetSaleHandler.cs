using Ambev.DeveloperEvaluation.Domain.Repositories;
using AutoMapper;
using FluentValidation;
using MediatR;

namespace Ambev.DeveloperEvaluation.Application.Sales.GetSale;

/// <summary>
/// Handler for processing GetSaleQuery requests
/// </summary>
public class GetSaleHandler : IRequestHandler<GetSaleQuery, GetSaleResult>
{
    private readonly ISaleRepository _saleReadRepository;
    private readonly IMapper _mapper;

    /// <summary>
    /// Initializes a new instance of GetSaleHandler
    /// </summary>
    /// <param name="saleReadRepository">The sale read repository</param>
    /// <param name="mapper">The AutoMapper instance</param>
    public GetSaleHandler(ISaleRepository saleReadRepository, IMapper mapper)
    {
        _saleReadRepository = saleReadRepository;
        _mapper = mapper;
    }

    /// <summary>
    /// Handles the GetSaleQuery request
    /// </summary>
    /// <param name="request">The GetSale query</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>The sale details if found</returns>
    public async Task<GetSaleResult> Handle(GetSaleQuery request, CancellationToken cancellationToken)
    {
        var validator = new GetSaleQueryValidator();
        var validationResult = await validator.ValidateAsync(request, cancellationToken);

        if (!validationResult.IsValid)
            throw new ValidationException(validationResult.Errors);

        var sale = await _saleReadRepository.GetByIdAsync(request.Id, cancellationToken);

        if (sale == null)
            throw new KeyNotFoundException($"Venda com ID {request.Id} não encontrada.");

        return _mapper.Map<GetSaleResult>(sale);
    }
}