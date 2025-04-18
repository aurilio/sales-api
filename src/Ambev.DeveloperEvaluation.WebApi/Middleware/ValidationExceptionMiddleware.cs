using Ambev.DeveloperEvaluation.Common.Exceptions;
using Ambev.DeveloperEvaluation.Domain.Common;
using Ambev.DeveloperEvaluation.WebApi.Common.Errors;
using FluentValidation;
using Serilog;
using System.Text.Json;

namespace Ambev.DeveloperEvaluation.WebApi.Middleware;

/// <summary>
/// Middleware responsável por capturar exceções esperadas durante o processamento da requisição
/// e retornar respostas padronizadas no formato ProblemDetails.
/// </summary>
public class ValidationExceptionMiddleware
{
    private readonly RequestDelegate _next;

    /// <summary>
    /// Inicializa uma nova instância do <see cref="ValidationExceptionMiddleware"/>.
    /// </summary>
    /// <param name="next">O próximo middleware da pipeline.</param>
    public ValidationExceptionMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    /// <summary>
    /// Executa o middleware, interceptando exceções de validação, domínio, argumentos inválidos e recursos não encontrados.
    /// </summary>
    /// <param name="context">O contexto HTTP da requisição.</param>
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
    }

    /// <summary>
    /// Trata exceções de validação do FluentValidation.
    /// </summary>
    private static Task HandleValidationExceptionAsync(HttpContext context, ValidationException exception)
    {
        Log.Warning(exception,
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
    /// Trata exceções lançadas quando um recurso não é encontrado.
    /// </summary>
    private static Task HandleNotFoundExceptionAsync(HttpContext context, NotFoundException exception)
    {
        Log.Warning(exception,
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
    /// Trata exceções de regras de negócio do domínio.
    /// </summary>
    private static Task HandleDomainExceptionAsync(HttpContext context, DomainException exception)
    {
        Log.Warning(exception,
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
    /// Trata exceções de argumentos inválidos em chamadas de métodos.
    /// </summary>
    private static Task HandleArgumentExceptionAsync(HttpContext context, ArgumentException exception)
    {
        Log.Warning(exception,
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
    /// Serializa e envia a resposta JSON padronizada para o cliente.
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