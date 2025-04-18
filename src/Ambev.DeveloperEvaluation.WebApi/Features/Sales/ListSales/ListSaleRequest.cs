using Microsoft.AspNetCore.Mvc;

namespace Ambev.DeveloperEvaluation.WebApi.Features.Sales.ListSales;

/// <summary>
/// Request model for listing sale records with pagination and ordering.
/// </summary>
public class ListSaleRequest
{
    /// <summary>
    /// The page number for pagination (default: 1).
    /// </summary>
    [FromQuery(Name = "_page")]
    public int Page { get; set; } = 1;

    /// <summary>
    /// The number of items per page (default: 10).
    /// </summary>
    [FromQuery(Name = "_size")]
    public int Size { get; set; } = 10;

    /// <summary>
    /// The ordering of results (e.g., "SaleDate desc, SaleNumber asc").
    /// </summary>
    [FromQuery(Name = "_order")]
    public string OrderBy { get; set; } = string.Empty;
}