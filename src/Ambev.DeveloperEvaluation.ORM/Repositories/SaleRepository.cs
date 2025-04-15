using Ambev.DeveloperEvaluation.Common.Pagination;
using Ambev.DeveloperEvaluation.Domain.Entities;
using Ambev.DeveloperEvaluation.Domain.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace Ambev.DeveloperEvaluation.ORM.Repositories;

/// <summary>
/// Implementation of ISaleRepository using Entity Framework Core
/// </summary>
public class SaleRepository : ISaleRepository
{
    private readonly DefaultContext _context;

    /// <summary>
    /// Initializes a new instance of SaleRepository
    /// </summary>
    /// <param name="context">The database context</param>
    public SaleRepository(DefaultContext context)
    {
        _context = context;
    }

    /// <inheritdoc />
    public async Task<Sale> CreateAsync(Sale sale, CancellationToken cancellationToken = default)
    {
        await _context.Sales.AddAsync(sale, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);
        return sale;
    }

    /// <inheritdoc />
    public async Task<Sale?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _context.Sales
            .Include(s => s.Items)
            .FirstOrDefaultAsync(s => s.Id == id, cancellationToken);
    }

    /// <inheritdoc />
    public IQueryable<Sale> GetAllQueryable()
    {
        return _context.Sales.AsQueryable().Include(s => s.Items);
    }

    /// <inheritdoc />
    public async Task<PaginatedList<Sale>> ListAsync(int pageNumber, int pageSize, string? orderBy = null, CancellationToken cancellationToken = default)
    {
        var query = GetAllQueryable();
        var totalCount = await query.CountAsync(cancellationToken);

        if (!string.IsNullOrWhiteSpace(orderBy))
        {
            query = ApplyOrdering(query, orderBy);
        }
        else
        {
            query = query.OrderByDescending(s => s.SaleDate); // Default ordering
        }

        var items = await query
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);

        return new PaginatedList<Sale>(items, totalCount, pageNumber, pageSize);
    }

    /// <inheritdoc />
    public async Task<Sale?> UpdateAsync(Sale sale, CancellationToken cancellationToken = default)
    {
        var existingSale = await _context.Sales
            .Include(s => s.Items)
            .FirstOrDefaultAsync(s => s.Id == sale.Id, cancellationToken);

        if (existingSale == null)
            return null;

        _context.Entry(existingSale).CurrentValues.SetValues(sale);

        //TODO: Update related items (this is a basic implementation, consider more sophisticated logic)
        existingSale.Items.Clear();
        if (sale.Items != null)
        {
            foreach (var item in sale.Items)
            {
                item.Id = existingSale.Id;
                _context.Entry(item).State = item.Id == default ? EntityState.Added : EntityState.Modified;
            }
        }

        await _context.SaveChangesAsync(cancellationToken);
        return existingSale;
    }

    /// <inheritdoc />
    public async Task<bool> DeleteAsync(Guid id, CancellationToken cancellationToken = default)
    {
        //TODO: Não está trazendo itens
        var sale = await _context.Sales.FindAsync(id, cancellationToken);
        if (sale == null)
            return false;

        _context.Sales.Remove(sale);
        await _context.SaveChangesAsync(cancellationToken);
        return true;
    }

    private IQueryable<Sale> ApplyOrdering(IQueryable<Sale> query, string orderBy)
    {
        var properties = orderBy.Split(',');
        var first = true;

        foreach (var property in properties)
        {
            var trimmedProperty = property.Trim();
            var orderDirection = "asc";

            if (trimmedProperty.EndsWith(" desc", System.StringComparison.OrdinalIgnoreCase))
            {
                trimmedProperty = trimmedProperty.Substring(0, trimmedProperty.Length - 5).Trim();
                orderDirection = "desc";
            }
            else if (trimmedProperty.EndsWith(" asc", System.StringComparison.OrdinalIgnoreCase))
            {
                trimmedProperty = trimmedProperty.Substring(0, trimmedProperty.Length - 4).Trim();
                orderDirection = "asc";
            }

            if (first)
            {
                query = orderDirection == "asc" ? query.OrderBy(s => EF.Property<object>(s, trimmedProperty)) : query.OrderByDescending(s => EF.Property<object>(s, trimmedProperty));
                first = false;
            }
            else
            {
                query = orderDirection == "asc" ? (query as IOrderedQueryable<Sale>)!.ThenBy(s => EF.Property<object>(s, trimmedProperty)) : (query as IOrderedQueryable<Sale>)!.ThenByDescending(s => EF.Property<object>(s, trimmedProperty));
            }
        }

        return query;
    }
}