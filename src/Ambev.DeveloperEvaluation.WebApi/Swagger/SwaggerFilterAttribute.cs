namespace Ambev.DeveloperEvaluation.WebApi.Swagger;

/// <summary>
/// Defines filterable fields for Swagger documentation on list endpoints.
/// </summary>
[AttributeUsage(AttributeTargets.Method, AllowMultiple = false)]
public class SwaggerFilterAttribute : Attribute
{
    /// <summary>
    /// The list of fields allowed for filtering.
    /// </summary>
    public string[] Fields { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="SwaggerFilterAttribute"/> class.
    /// </summary>
    /// <param name="fields">The names of the fields that can be used for filtering in the endpoint.</param>
    public SwaggerFilterAttribute(params string[] fields)
    {
        Fields = fields;
    }
}