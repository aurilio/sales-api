using Ambev.DeveloperEvaluation.Common.Exceptions;
using Ambev.DeveloperEvaluation.WebApi.Common.Errors;
using FluentValidation;
using Microsoft.EntityFrameworkCore;
using Serilog;
using System.Text.Json;

namespace Ambev.DeveloperEvaluation.WebApi.Middleware;

/// <summary>
/// Middleware responsible for capturing known exceptions during request processing
/// and returning standardized responses in ProblemDetails format.
/// </summary>
public class ValidationExceptionMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<ValidationExceptionMiddleware> _logger;

    /// <summary>
    /// Initializes a new instance of <see cref="ValidationExceptionMiddleware"/>.
    /// </summary>
    /// <param name="next">The next middleware in the pipeline.</param>
    public ValidationExceptionMiddleware(
        RequestDelegate next,
        ILogger<ValidationExceptionMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    /// <summary>
    /// Executes the middleware, intercepting validation, domain, argument and not found exceptions.
    /// </summary>
    /// <param name="context">The HTTP request context.</param>
    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (ValidationException ex)
        {
            await HandleValidationExceptionAsync(context, ex);
        }
        catch (NotFoundException ex)
        {
            await HandleNotFoundExceptionAsync(context, ex);
        }
        catch (DomainException ex)
        {
            await HandleDomainExceptionAsync(context, ex);
        }
        catch (ArgumentException ex)
        {
            await HandleArgumentExceptionAsync(context, ex);
        }
        catch (DbUpdateConcurrencyException ex)
        {
            await HandleConcurrencyExceptionAsync(context, ex);
        }
    }

    /// <summary>
    /// Handles validation exceptions thrown by FluentValidation.
    /// </summary>
    private Task HandleValidationExceptionAsync(HttpContext context, ValidationException exception)
    {
        _logger.LogWarning(exception,
            "Validation error at {Path} | TraceId: {TraceId} | Errors: {@Errors}",
            context.Request.Path,
            context.TraceIdentifier,
            exception.Errors);

        var response = CustomProblemDetailsFactory.CreateValidationProblemDetails(exception.Errors);

        context.Response.ContentType = "application/json";
        context.Response.StatusCode = StatusCodes.Status400BadRequest;

        return WriteResponseAsync(context, response);
    }

    /// <summary>
    /// Handles not found exceptions when a resource is missing.
    /// </summary>
    private Task HandleNotFoundExceptionAsync(HttpContext context, NotFoundException exception)
    {
        _logger.LogWarning(exception,
            "Resource not found at {Path} | TraceId: {TraceId} | Resource: {Resource} | Identifier: {Identifier}",
            context.Request.Path,
            context.TraceIdentifier,
            exception.ResourceName,
            exception.Identifier ?? "unknown");

        var response = CustomProblemDetailsFactory.CreateNotFoundProblemDetails(exception);

        context.Response.ContentType = "application/json";
        context.Response.StatusCode = StatusCodes.Status404NotFound;

        return WriteResponseAsync(context, response);
    }

    /// <summary>
    /// Handles domain exceptions related to business rule violations.
    /// </summary>
    private Task HandleDomainExceptionAsync(HttpContext context, DomainException exception)
    {
        _logger.LogWarning(exception,
            "Domain rule violation at {Path} | TraceId: {TraceId} | Message: {Message}",
            context.Request.Path,
            context.TraceIdentifier,
            exception.Message);

        var response = CustomProblemDetailsFactory.CreateGenericProblemDetails(
            type: "DomainError",
            error: "Business rule violation",
            detail: exception.Message
        );

        context.Response.ContentType = "application/json";
        context.Response.StatusCode = StatusCodes.Status400BadRequest;

        return WriteResponseAsync(context, response);
    }

    /// <summary>
    /// Handles invalid argument exceptions for method calls.
    /// </summary>
    private Task HandleArgumentExceptionAsync(HttpContext context, ArgumentException exception)
    {
        _logger.LogWarning(exception,
            "Invalid argument at {Path} | TraceId: {TraceId} | Param: {ParamName} | Message: {Message}",
            context.Request.Path,
            context.TraceIdentifier,
            exception.ParamName ?? "N/A",
            exception.Message);

        var response = CustomProblemDetailsFactory.CreateGenericProblemDetails(
            type: "ArgumentError",
            error: "Invalid parameter or argument",
            detail: exception.Message
        );

        context.Response.ContentType = "application/json";
        context.Response.StatusCode = StatusCodes.Status400BadRequest;

        return WriteResponseAsync(context, response);
    }

    /// <summary>
    /// Handles optimistic concurrency exceptions (conflicts during update).
    /// </summary>
    private Task HandleConcurrencyExceptionAsync(HttpContext context, DbUpdateConcurrencyException exception)
    {
        _logger.LogWarning(exception,
            "Concurrency conflict at {Path} | TraceId: {TraceId} | Message: {Message}",
            context.Request.Path,
            context.TraceIdentifier,
            exception.Message);

        var response = CustomProblemDetailsFactory.CreateGenericProblemDetails(
            type: "ConcurrencyConflict",
            error: "Conflict detected",
            detail: "The resource you're trying to update has been modified by another operation. Please refresh and try again."
        );

        context.Response.ContentType = "application/json";
        context.Response.StatusCode = StatusCodes.Status409Conflict;

        return WriteResponseAsync(context, response);
    }

    /// <summary>
    /// Serializes and sends a standardized JSON response to the client.
    /// </summary>
    private static Task WriteResponseAsync(HttpContext context, object response)
    {
        var options = new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            WriteIndented = true
        };

        return context.Response.WriteAsync(JsonSerializer.Serialize(response, options));
    }
}