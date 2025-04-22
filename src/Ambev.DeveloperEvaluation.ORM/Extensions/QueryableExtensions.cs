using System.Globalization;
using System.Linq.Expressions;
using System.Reflection;

namespace Ambev.DeveloperEvaluation.ORM.Extensions;

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

    /// <summary>
    /// Dynamically applies filters to a queryable source using a dictionary.
    /// </summary>
    /// <typeparam name="T">The type of the queryable elements.</typeparam>
    /// <param name="query">The source queryable.</param>
    /// <param name="filters">A dictionary of filters, where the key is the property name (optionally prefixed with _min or _max) and the value is the filter value.</param>
    /// <returns>The filtered queryable.</returns>
    public static IQueryable<T> ApplyFilters<T>(this IQueryable<T> query, Dictionary<string, string> filters)
    {
        var properties = typeof(T).GetProperties(BindingFlags.Public | BindingFlags.Instance)
                                  .ToDictionary(p => p.Name, p => p, StringComparer.OrdinalIgnoreCase);

        foreach (var (key, value) in filters)
        {
            if (string.IsNullOrWhiteSpace(value) ||
                key.StartsWith("_page", StringComparison.OrdinalIgnoreCase) ||
                key.StartsWith("_size", StringComparison.OrdinalIgnoreCase) ||
                key.StartsWith("_order", StringComparison.OrdinalIgnoreCase))
            {
                continue;
            }

            var parameter = Expression.Parameter(typeof(T), "x");
            Expression? comparison = null;
            Expression? propertyExpr = null;

            if (key.StartsWith("_min", StringComparison.OrdinalIgnoreCase))
            {
                var propName = key[4..];
                if (!properties.TryGetValue(propName, out var propertyInfo)) continue;

                propertyExpr = Expression.Property(parameter, propertyInfo);
                var constant = ConvertToPropertyType(propertyInfo.PropertyType, value);
                comparison = Expression.GreaterThanOrEqual(propertyExpr, constant);
            }
            else if (key.StartsWith("_max", StringComparison.OrdinalIgnoreCase))
            {
                var propName = key[4..];
                if (!properties.TryGetValue(propName, out var propertyInfo)) continue;

                propertyExpr = Expression.Property(parameter, propertyInfo);
                var constant = ConvertToPropertyType(propertyInfo.PropertyType, value);
                comparison = Expression.LessThanOrEqual(propertyExpr, constant);
            }
            else
            {
                if (!properties.TryGetValue(key, out var propertyInfo)) continue;

                propertyExpr = Expression.Property(parameter, propertyInfo);

                if (propertyInfo.PropertyType == typeof(string))
                {
                    var method = GetWildcardStringMethod(value, out var formatted);
                    var toLowerMethod = typeof(string).GetMethod("ToLower", Type.EmptyTypes)!;
                    var toLowerProperty = Expression.Call(propertyExpr, toLowerMethod);
                    var constant = Expression.Constant(formatted.ToLowerInvariant());
                    comparison = Expression.Call(toLowerProperty, method, constant);
                }
                else if (propertyInfo.PropertyType == typeof(bool))
                {
                    comparison = Expression.Equal(propertyExpr, Expression.Constant(bool.Parse(value)));
                }
                else if (propertyInfo.PropertyType == typeof(int))
                {
                    comparison = Expression.Equal(propertyExpr, Expression.Constant(int.Parse(value)));
                }
                else if (propertyInfo.PropertyType == typeof(decimal))
                {
                    comparison = Expression.Equal(propertyExpr, Expression.Constant(decimal.Parse(value, CultureInfo.InvariantCulture)));
                }
                else if (propertyInfo.PropertyType == typeof(Guid))
                {
                    comparison = Expression.Equal(propertyExpr, Expression.Constant(Guid.Parse(value)));
                }
                else
                {
                    continue;
                }
            }

            if (comparison != null)
            {
                var lambda = Expression.Lambda<Func<T, bool>>(comparison, parameter);
                query = query.Where(lambda);
            }
        }

        return query;
    }

    private static Expression ConvertToPropertyType(Type type, string value)
    {
        if (type == typeof(int))
            return Expression.Constant(int.Parse(value));
        if (type == typeof(decimal))
            return Expression.Constant(decimal.Parse(value, CultureInfo.InvariantCulture));
        if (type == typeof(DateTime))
            return Expression.Constant(DateTime.Parse(value, CultureInfo.InvariantCulture));
        if (type == typeof(Guid))
            return Expression.Constant(Guid.Parse(value));
        if (type == typeof(bool))
            return Expression.Constant(bool.Parse(value));

        throw new NotSupportedException($"Unsupported filter type: {type.Name}");
    }

    private static MethodInfo GetWildcardStringMethod(string value, out string formattedValue)
    {
        if (value.StartsWith("*") && value.EndsWith("*"))
        {
            formattedValue = value.Trim('*');
            return typeof(string).GetMethod("Contains", new[] { typeof(string) })!;
        }
        else if (value.StartsWith("*"))
        {
            formattedValue = value[1..];
            return typeof(string).GetMethod("EndsWith", new[] { typeof(string) })!;
        }
        else if (value.EndsWith("*"))
        {
            formattedValue = value[..^1];
            return typeof(string).GetMethod("StartsWith", new[] { typeof(string) })!;
        }
        else
        {
            formattedValue = value;
            return typeof(string).GetMethod("Equals", new[] { typeof(string) })!;
        }
    }
}