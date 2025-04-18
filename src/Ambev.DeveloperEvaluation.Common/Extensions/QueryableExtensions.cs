using System.Linq.Expressions;

namespace Ambev.DeveloperEvaluation.Common.Extensions;

/// <summary>
/// Provides dynamic ordering capabilities for LINQ queries by property name.
/// </summary>
public static class QueryableExtensions
{
    /// <summary>
    /// Dynamically applies OrderBy on a queryable source using a string property name.
    /// </summary>
    /// <typeparam name="T">The type of the source elements.</typeparam>
    /// <param name="source">The queryable source.</param>
    /// <param name="propertyName">The property to order by.</param>
    /// <returns>An ordered queryable by the specified property.</returns>
    public static IOrderedQueryable<T> OrderByDynamic<T>(this IQueryable<T> source, string propertyName)
    {
        var param = Expression.Parameter(typeof(T), "x");
        var property = Expression.PropertyOrField(param, propertyName);
        var lambda = Expression.Lambda(property, param);

        var method = typeof(Queryable).GetMethods()
            .First(m => m.Name == "OrderBy" && m.GetParameters().Length == 2)
            .MakeGenericMethod(typeof(T), property.Type);

        return (IOrderedQueryable<T>)method.Invoke(null, new object[] { source, lambda })!;
    }
}