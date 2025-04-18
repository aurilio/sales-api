namespace Ambev.DeveloperEvaluation.Common.Exceptions;

/// <summary>
/// Represents an exception thrown when a specified resource is not found in the system.
/// This exception is reusable and agnostic of resource type.
/// </summary>
public class NotFoundException : Exception
{
    /// <summary>
    /// Gets the name of the resource that was not found.
    /// </summary>
    public string ResourceName { get; }

    /// <summary>
    /// Gets the identifier value used to attempt to find the resource.
    /// </summary>
    public object Identifier { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="NotFoundException"/> class with a specific resource name and identifier.
    /// </summary>
    /// <param name="resourceName">The name of the resource that could not be found.</param>
    /// <param name="id">The identifier value used in the lookup.</param>
    public NotFoundException(string resourceName, object id)
        : base($"{resourceName} with identifier {id} was not found.")
    {
        ResourceName = resourceName;
        Identifier = id;
    }
}