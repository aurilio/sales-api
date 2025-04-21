using Ambev.DeveloperEvaluation.Common.Pagination;
using Ambev.DeveloperEvaluation.Domain.Entities;
using Ambev.DeveloperEvaluation.Domain.Repositories;
using Ambev.DeveloperEvaluation.ORM.Extensions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Ambev.DeveloperEvaluation.ORM.Repositories;

/// <summary>
/// Implementation of ISaleRepository using Entity Framework Core.
/// Provides CRUD and pagination operations for Sale entities.
/// </summary>
public class SaleRepository : ISaleRepository
{
    private readonly DefaultContext _context;
    private readonly ILogger<SaleRepository> _logger;

    /// <summary>
    /// Initializes a new instance of the <see cref="SaleRepository"/> class.
    /// </summary>
    /// <param name="context">The database context.</param>
    /// <param name="logger">Logger for observability and debugging.</param>
    public SaleRepository(DefaultContext context, ILogger<SaleRepository> logger)
    {
        _context = context;
        _logger = logger;
    }

    /// <inheritdoc />
    /// <summary>
    /// Creates a new sale in the database.
    /// </summary>
    /// <param name="sale">The sale entity to persist.</param>
    /// <param name="cancellationToken">Token to cancel the operation.</param>
    /// <returns>The persisted sale.</returns>
    public async Task<Sale> CreateAsync(Sale sale, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Creating sale with number {SaleNumber}", sale.SaleNumber);
        await _context.Sales.AddAsync(sale, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);
        _logger.LogInformation("Sale with ID {SaleId} created successfully", sale.Id);
        return sale;
    }

    /// <inheritdoc />
    /// <summary>
    /// Retrieves a sale by ID, including its items, or null if not found.
    /// </summary>
    /// <param name="id">The unique identifier of the sale.</param>
    /// <param name="cancellationToken">Token to cancel the operation.</param>
    /// <returns>The sale with its items if found; otherwise, null.</returns>
    public async Task<Sale?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        _logger.LogDebug("Fetching sale with ID: {SaleId}", id);

        var result = await _context.Sales.AsNoTracking()
                                    .Include(s => s.Items)
                                    .FirstOrDefaultAsync(s => s.Id == id, cancellationToken);

        return result;
    }

    /// <inheritdoc />
    /// <summary>
    /// Returns a queryable collection of sales including their items.
    /// </summary>
    /// <returns>IQueryable of Sales with includes applied.</returns>
    public IQueryable<Sale> GetAllQueryable()
    {
        return _context.Sales.AsQueryable().Include(s => s.Items);
    }

    /// <inheritdoc />
    /// <summary>
    /// Lists paginated sales with optional ordering.
    /// </summary>
    /// <param name="pageNumber">The page number (1-based).</param>
    /// <param name="pageSize">The number of items per page.</param>
    /// <param name="orderBy">Comma-separated ordering fields (e.g. "saleDate desc").</param>
    /// <param name="cancellationToken">Token to cancel the operation.</param>
    /// <returns>Paginated list of sales.</returns>
    public async Task<PaginatedList<Sale>> ListAsync(int pageNumber, int pageSize, string? orderBy = null, Dictionary<string, string>? filters = null, CancellationToken cancellationToken = default)
    {
        _logger.LogDebug("Listing sales with page: {Page}, size: {Size}, order: {Order}", pageNumber, pageSize, orderBy);

        var query = GetAllQueryable();
        var totalCount = await query.CountAsync(cancellationToken);

        if (filters is { Count: > 0 })
        {
            query = query.ApplyFilters(filters);
            _logger.LogDebug("Applied {Count} filters to query.", filters.Count);
        }

        query = string.IsNullOrWhiteSpace(orderBy)
                ? query.OrderByDescending(s => s.SaleDate)
                : query.OrderByDynamic(orderBy!);

        var items = await query
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);

        return new PaginatedList<Sale>(items, totalCount, pageNumber, pageSize);
    }

    /// <inheritdoc />
    /// <summary>
    /// Updates an existing sale in the database.
    /// </summary>
    /// <param name="sale">The sale entity to update.</param>
    /// <param name="cancellationToken">Token to cancel the operation.</param>
    /// <returns>The updated sale.</returns>
    public async Task<Sale?> UpdateAsync(Sale sale, CancellationToken cancellationToken = default)
    {
        _context.Sales.Update(sale);

        _logger.LogDebug("Saving changes to database for sale ID: {SaleId}", sale.Id);
        await _context.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("Sale with ID {SaleId} updated successfully.", sale.Id);

        return sale;
    }

    /// <inheritdoc />
    /// <summary>
    /// Deletes a sale by ID from the database.
    /// </summary>
    /// <param name="id">The ID of the sale to delete.</param>
    /// <param name="cancellationToken">Token to cancel the operation.</param>
    /// <returns>True if deleted; otherwise false.</returns>
    public async Task<bool> DeleteAsync(Guid id, CancellationToken cancellationToken = default)
    {
        _logger.LogDebug("Attempting to delete sale with ID: {SaleId}", id);

        var sale = await _context.Sales.FindAsync(new object[] { id }, cancellationToken);
        if (sale == null)
        {
            _logger.LogWarning("Sale with ID {SaleId} not found.", id);
            return false;
        }

        _context.Sales.Remove(sale);
        await _context.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("Sale with ID {SaleId} deleted successfully.", id);
        return true;
    }
}