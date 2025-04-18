using Microsoft.AspNetCore.Mvc;

namespace Ambev.DeveloperEvaluation.WebApi.Common.Errors;

/// <summary>
/// Custom representation of a standardized problem details response,
/// used to communicate structured error information to clients.
/// Extends <see cref="ProblemDetails"/> to include domain-specific fields.
/// </summary>
public class CustomValidationProblemDetails : ProblemDetails
{
    /// <summary>
    /// A short, human-readable summary of the problem.
    /// Example: "Invalid input data", "Business rule violation".
    /// </summary>
    public string Error { get; init; }

    /// <summary>
    /// Initializes a new instance of <see cref="CustomValidationProblemDetails"/>.
    /// </summary>
    /// <param name="Type">The unique identifier for the type of error.</param>
    /// <param name="Error">A summary of the error.</param>
    /// <param name="Detail">A detailed explanation of the problem.</param>
    public CustomValidationProblemDetails(string Type, string Error, string Detail)
    {
        this.Type = Type;
        this.Detail = Detail;
        this.Error = Error;
    }
}