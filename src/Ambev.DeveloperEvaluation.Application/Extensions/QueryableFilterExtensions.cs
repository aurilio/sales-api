using System.Globalization;
using System.Linq.Expressions;
using System.Reflection;

namespace Ambev.DeveloperEvaluation.Common.Pagination;

/// <summary>
/// Provides extension methods to dynamically apply filters to an IQueryable based on dictionary input.
/// </summary>
public static class QueryableFilterExtensions
{
    /// <summary>
    /// Dynamically applies filters to a queryable source using a dictionary.
    /// </summary>
    /// <typeparam name="T">The type of the queryable elements.</typeparam>
    /// <param name="query">The source queryable.</param>
    /// <param name="filters">A dictionary of filters, where the key is the property name (optionally prefixed with _min or _max) and the value is the filter value.</param>
    /// <returns>The filtered queryable.</returns>
    public static IQueryable<T> ApplyFilters<T>(this IQueryable<T> query, Dictionary<string, string> filters)
    {
        foreach (var (key, value) in filters)
        {
            if (string.IsNullOrWhiteSpace(value))
                continue;

            var parameter = Expression.Parameter(typeof(T), "x");
            Expression? property;
            Expression? comparison;
            Expression? constant;

            if (key.StartsWith("_min", StringComparison.OrdinalIgnoreCase))
            {
                var propName = key[4..];
                property = Expression.PropertyOrField(parameter, propName);
                constant = ConvertToPropertyType(property.Type, value);
                comparison = Expression.GreaterThanOrEqual(property, constant);
            }
            else if (key.StartsWith("_max", StringComparison.OrdinalIgnoreCase))
            {
                var propName = key[4..];
                property = Expression.PropertyOrField(parameter, propName);
                constant = ConvertToPropertyType(property.Type, value);
                comparison = Expression.LessThanOrEqual(property, constant);
            }
            else
            {
                property = Expression.PropertyOrField(parameter, key);

                if (property.Type == typeof(string))
                {
                    var method = GetWildcardStringMethod(value, out var formatted);
                    constant = Expression.Constant(formatted.ToLowerInvariant());
                    var toLowerMethod = typeof(string).GetMethod("ToLower", Type.EmptyTypes)!;
                    var toLowerProperty = Expression.Call(property, toLowerMethod);
                    comparison = Expression.Call(toLowerProperty, method, constant);
                }
                else if (property.Type == typeof(bool))
                {
                    constant = Expression.Constant(bool.Parse(value));
                    comparison = Expression.Equal(property, constant);
                }
                else if (property.Type == typeof(int))
                {
                    constant = Expression.Constant(int.Parse(value));
                    comparison = Expression.Equal(property, constant);
                }
                else if (property.Type == typeof(decimal))
                {
                    constant = Expression.Constant(decimal.Parse(value, CultureInfo.InvariantCulture));
                    comparison = Expression.Equal(property, constant);
                }
                else if (property.Type == typeof(Guid))
                {
                    constant = Expression.Constant(Guid.Parse(value));
                    comparison = Expression.Equal(property, constant);
                }
                else
                {
                    continue;
                }
            }

            var lambda = Expression.Lambda<Func<T, bool>>(comparison, parameter);
            query = query.Where(lambda);
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