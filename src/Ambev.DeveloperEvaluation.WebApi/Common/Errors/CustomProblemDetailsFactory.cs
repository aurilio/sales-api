using Ambev.DeveloperEvaluation.Common.Exceptions;
using FluentValidation.Results;

namespace Ambev.DeveloperEvaluation.WebApi.Common.Errors;

/// <summary>
/// Centralized factory for creating standardized <see cref="CustomValidationProblemDetails"/> objects
/// used to represent API errors in a consistent and structured format.
/// </summary>
public static class CustomProblemDetailsFactory
{
    /// <summary>
    /// Creates a <see cref="CustomValidationProblemDetails"/> object for validation errors thrown by FluentValidation.
    /// </summary>
    /// <param name="errors">The collection of validation failures.</param>
    /// <returns>An object representing the validation error details.</returns>
    public static object CreateValidationProblemDetails(IEnumerable<ValidationFailure> errors)
    {
        var firstError = errors.FirstOrDefault();

        return new CustomValidationProblemDetails(
            Type: "validation_error",
            Error: "Invalid input data",
            Detail: firstError != null
                ? $"The '{firstError.PropertyName}' field {firstError.ErrorMessage.ToLowerInvariant()}"
                : "One or more validation errors occurred."
        );
    }

    /// <summary>
    /// Creates a <see cref="CustomValidationProblemDetails"/> object for scenarios where a resource is not found.
    /// </summary>
    /// <param name="exception">The exception containing resource name and identifier.</param>
    /// <returns>An object representing the not-found error details.</returns>
    public static object CreateNotFoundProblemDetails(NotFoundException exception)
    {
        var resource = string.IsNullOrWhiteSpace(exception.ResourceName)
            ? "Resource"
            : exception.ResourceName;

        var identifier = exception.Identifier?.ToString();
        if (string.IsNullOrWhiteSpace(identifier))
            identifier = "unknown";

        return new CustomValidationProblemDetails(
            Type: "resource_not_found",
            Error: $"{resource} not found",
            Detail: $"{resource} with identifier '{identifier}' was not found."
        );
    }

    /// <summary>
    /// Creates a <see cref="CustomValidationProblemDetails"/> object for generic or custom error cases.
    /// </summary>
    /// <param name="type">A string representing the type or category of the error (e.g., "domain_error").</param>
    /// <param name="error">A concise error title describing the issue.</param>
    /// <param name="detail">A detailed explanation of the error.</param>
    /// <returns>An object representing the generic error details.</returns>
    public static object CreateGenericProblemDetails(string type, string error, string detail)
    {
        return new CustomValidationProblemDetails(
            Type: string.IsNullOrWhiteSpace(type) ? "generic_error" : type,
            Error: string.IsNullOrWhiteSpace(error) ? "An error occurred" : error,
            Detail: string.IsNullOrWhiteSpace(detail) ? "An unknown error has occurred." : detail
        );
    }
}