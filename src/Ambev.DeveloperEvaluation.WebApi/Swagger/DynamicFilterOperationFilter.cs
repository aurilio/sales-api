using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;
using System.Reflection;

namespace Ambev.DeveloperEvaluation.WebApi.Swagger;

/// <summary>
/// Adds dynamic filter parameters to Swagger documentation based on the <see cref="SwaggerFilterAttribute"/>.
/// This helps automatically document query filters for endpoints like paginated lists.
/// </summary>
public class DynamicFilterOperationFilter : IOperationFilter
{
    /// <summary>
    /// Applies the dynamic filter parameters to the Swagger operation based on the presence of the <see cref="SwaggerFilterAttribute"/>.
    /// </summary>
    /// <param name="operation">The Swagger operation being documented.</param>
    /// <param name="context">The context for the current Swagger operation.</param>
    public void Apply(OpenApiOperation operation, OperationFilterContext context)
    {
        var actionMethod = context.MethodInfo;

        var filterAttribute = actionMethod
            .GetCustomAttribute<SwaggerFilterAttribute>();

        if (filterAttribute is null)
            return;

        foreach (var field in filterAttribute.Fields)
        {
            var isRange = field.StartsWith("_min") || field.StartsWith("_max");
            var isBoolean = field.Equals("isCancelled", StringComparison.OrdinalIgnoreCase);
            var isNumeric = field.Contains("totalAmount", StringComparison.OrdinalIgnoreCase);

            var type = isBoolean ? "boolean"
                      : isNumeric || isRange ? "number"
                      : "string";

            operation.Parameters.Add(new OpenApiParameter
            {
                Name = field,
                In = ParameterLocation.Query,
                Required = false,
                Schema = new OpenApiSchema { Type = type },
                Description = GenerateDescription(field)
            });
        }
    }

    /// <summary>
    /// Generates a description for a given filter field based on naming conventions.
    /// </summary>
    /// <param name="field">The field name from the filter attribute.</param>
    /// <returns>A human-readable description for Swagger UI.</returns>
    private static string GenerateDescription(string field)
    {
        if (field.StartsWith("_min"))
            return $"Minimum value for '{field.Replace("_min", "")}' filter.";
        if (field.StartsWith("_max"))
            return $"Maximum value for '{field.Replace("_max", "")}' filter.";
        if (field.Equals("isCancelled", StringComparison.OrdinalIgnoreCase))
            return "Filter by cancellation status (true/false).";
        return $"Exact or partial match for the '{field}' field. Use * for wildcard.";
    }
}