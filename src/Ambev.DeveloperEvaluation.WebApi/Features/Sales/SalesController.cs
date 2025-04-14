using Ambev.DeveloperEvaluation.Application.Sales.CreateSale;
using Ambev.DeveloperEvaluation.Application.Sales.DeleteSale;
using Ambev.DeveloperEvaluation.Application.Sales.GetSale;
using Ambev.DeveloperEvaluation.Application.Sales.ListSales;
using Ambev.DeveloperEvaluation.Application.Sales.UpdateSale;
using Ambev.DeveloperEvaluation.Application.Users.DeleteUser;
using Ambev.DeveloperEvaluation.WebApi.Common;
using Ambev.DeveloperEvaluation.WebApi.Features.Sales.CreateSale;
using Ambev.DeveloperEvaluation.WebApi.Features.Sales.DeleteSale;
using Ambev.DeveloperEvaluation.WebApi.Features.Sales.GetSale;
using Ambev.DeveloperEvaluation.WebApi.Features.Sales.ListSales;
using Ambev.DeveloperEvaluation.WebApi.Features.Sales.UpdateSale;
using AutoMapper;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Ambev.DeveloperEvaluation.WebApi.Features.Sales;

/// <summary>
/// Controller for managing sales records
/// </summary>
[ApiController]
[Route("api/[controller]")]
public class SalesController : BaseController
{
    private readonly IMediator _mediator;
    private readonly IMapper _mapper;

    /// <summary>
    /// Initializes a new instance of the <see cref="SalesController"/> class.
    /// </summary>
    /// <param name="mediator">The mediator instance.</param>
    /// <param name="mapper">The AutoMapper instance.</param>
    public SalesController(IMediator mediator, IMapper mapper)
    {
        _mediator = mediator;
        _mapper = mapper;
    }

    /// <summary>
    /// Creates a new sale record.
    /// </summary>
    /// <param name="request">The sale creation request.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The details of the created sale record.</returns>
    [HttpPost]
    [ProducesResponseType(typeof(ApiResponseWithData<CreateSaleResponse>), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> CreateSale([FromBody] CreateSaleRequest request, CancellationToken cancellationToken)
    {
        var validator = new CreateSaleRequestValidator();
        var validationResult = await validator.ValidateAsync(request, cancellationToken);
        
        if (!validationResult.IsValid)
            return BadRequest(validationResult.Errors);

        var command = _mapper.Map<CreateSaleCommand>(request);
        var response = await _mediator.Send(command, cancellationToken);
        var apiResponse = _mapper.Map<CreateSaleResponse>(response);

        return Created<CreateSaleResponse>(
                routeName: nameof(GetSale),
                routeValues: new { id = apiResponse.Id },
                data: _mapper.Map<CreateSaleResponse>(response),
                message: "Sale created successfully"
            );
    }

    /// <summary>
    /// Retrieves a specific sale record by its ID.
    /// </summary>
    /// <param name="id">The unique identifier of the sale record.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The details of the requested sale record.</returns>
    [HttpGet("{id}")]
    [ProducesResponseType(typeof(ApiResponseWithData<GetSaleResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetSale([FromRoute] Guid id, CancellationToken cancellationToken)
    {
        var request = new GetSaleRequest { Id = id };
        var validator = new GetSaleRequestValidator();
        var validationResult = await validator.ValidateAsync(request, cancellationToken);
        
        if (!validationResult.IsValid)
            return BadRequest(validationResult.Errors);

        var query = _mapper.Map<GetSaleQuery>(request.Id);
        var response = await _mediator.Send(query, cancellationToken);
        var apiResponse = _mapper.Map<GetSaleResponse>(response);

        return Ok(new ApiResponseWithData<GetSaleResponse>
        {
            Success = true,
            Message = "Sale retrieved successfully",
            Data = apiResponse
        });
    }

    /// <summary>
    /// Updates an existing sale record.
    /// </summary>
    /// <param name="id">The unique identifier of the sale record to update.</param>
    /// <param name="request">The sale update request.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The details of the updated sale record.</returns>
    [HttpPut("{id}")]
    [ProducesResponseType(typeof(ApiResponseWithData<UpdateSaleResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> UpdateSale([FromRoute] Guid id, [FromBody] UpdateSaleRequest request, CancellationToken cancellationToken)
    {
        var validator = new UpdateSaleRequestValidator();
        var validationResult = await validator.ValidateAsync(request, cancellationToken);
        
        if (!validationResult.IsValid)
            return BadRequest(validationResult.Errors);

        var command = _mapper.Map<UpdateSaleCommand>(request);
        command.Id = id;

        var response = await _mediator.Send(command, cancellationToken);
        var apiResponse = _mapper.Map<UpdateSaleResponse>(response);

        return Ok(new ApiResponseWithData<UpdateSaleResponse>
        {
            Success = true,
            Message = "Sale updated successfully",
            Data = apiResponse
        });
    }

    /// <summary>
    /// Deletes a specific sale record by its ID.
    /// </summary>
    /// <param name="id">The unique identifier of the sale record to delete.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>A success response if the sale record was deleted.</returns>
    [HttpDelete("{id}")]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> DeleteSale([FromRoute] Guid id, CancellationToken cancellationToken)
    {
        var request = new DeleteSaleRequest { Id = id };
        var validator = new DeleteSaleRequestValidator();
        var validationResult = await validator.ValidateAsync(request, cancellationToken);
        
        if (!validationResult.IsValid)
            return BadRequest(validationResult.Errors);

        var command = _mapper.Map<DeleteUserCommand>(request.Id);
        var response = await _mediator.Send(command, cancellationToken);
        var apiResponse = _mapper.Map<DeleteSaleResponse>(response);

        return Ok(new ApiResponseWithData<DeleteSaleResponse>
        {
            Data = apiResponse,
            Success = true,
            Message = "Sale deleted successfully"
        });
    }

    /// <summary>
    /// Retrieves a list of all sale records with pagination and ordering.
    /// </summary>
    /// <param name="request">The listing request parameters.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>A paginated list of sale records.</returns>
    [HttpGet]
    [ProducesResponseType(typeof(ApiResponseWithData<PaginatedResponse<GetSaleResponse>>), StatusCodes.Status200OK)]
    public async Task<IActionResult> ListSales([FromQuery] ListSaleRequest request, CancellationToken cancellationToken)
    {
        var validator = new ListSaleRequestValidator();
        var validationResult = await validator.ValidateAsync(request, cancellationToken);
        
        if (!validationResult.IsValid)
            return BadRequest(validationResult.Errors);

        var query = _mapper.Map<ListSaleQuery>(request);
        var response = await _mediator.Send(query, cancellationToken);

        var getSaleResponses = _mapper.Map<IEnumerable<GetSaleResponse>>(response);
        var paginatedResponse = new PaginatedResponse<GetSaleResponse>
        {
            CurrentPage = response. CurrentPage,
            TotalPages = response.TotalPages,
            TotalCount = response.TotalCount,
            Data = getSaleResponses
        };

        return Ok(new ApiResponseWithData<PaginatedResponse<GetSaleResponse>>
        {
            Success = true,
            Message = "Sales retrieved successfully",
            Data = paginatedResponse
        });
    }
}