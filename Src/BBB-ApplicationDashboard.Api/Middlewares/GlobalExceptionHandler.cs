namespace BBB_ApplicationDashboard.Api.Middlewares;

using BBB_ApplicationDashboard.Infrastructure.Exceptions.Common;
using FluentValidation;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;

public class GlobalExceptionHandler(ILogger<GlobalExceptionHandler> logger) : IExceptionHandler
{
    private static string? GetClientIpAddress(HttpContext context)
    {
        if (
            context.Request.Headers.TryGetValue("X-Forwarded-For", out var xff)
            && !string.IsNullOrWhiteSpace(xff)
        )
            return xff.ToString().Split(',')[0].Trim();
        return context.Connection.RemoteIpAddress?.ToString();
    }

    public async ValueTask<bool> TryHandleAsync(
        HttpContext httpContext,
        Exception exception,
        CancellationToken cancellationToken
    )
    {
        var requestPath = httpContext.Request.Path;
        var requestMethod = httpContext.Request.Method;
        var clientIp = GetClientIpAddress(httpContext);
        var userAgent = httpContext.Request.Headers.UserAgent.ToString();
        var traceId = httpContext.TraceIdentifier;

        logger.LogError(
            exception,
            "💥 {Event} | {Method} {Path} | IP:{ClientIP} | UA:{UserAgent} | Trace:{TraceId} | {ExceptionType}: {ExceptionMessage}",
            "UnhandledException",
            requestMethod,
            requestPath,
            clientIp,
            userAgent,
            traceId,
            exception.GetType().Name,
            exception.Message
        );

        //! Log additional details for unhandled exceptions
        if (!IsKnownExceptionType(exception))
        {
            logger.LogError(
                exception,
                "🧩 {Event} | Trace:{TraceId} | Inner:{Inner} | Source:{Source}",
                "UnhandledException.Details",
                traceId,
                exception.InnerException?.Message ?? "None",
                exception.Source
            );
        }

        (string Detail, string Title, int statusCode) = exception switch
        {
            InternalServerException => (
                exception.Message,
                exception.GetType().Name,
                httpContext.Response.StatusCode = StatusCodes.Status500InternalServerError
            ),
            BadRequestException => (
                exception.Message,
                exception.GetType().Name,
                httpContext.Response.StatusCode = StatusCodes.Status400BadRequest
            ),
            NotFoundException => (
                exception.Message,
                exception.GetType().Name,
                httpContext.Response.StatusCode = StatusCodes.Status404NotFound
            ),
            ConflictException => (
                exception.Message,
                exception.GetType().Name,
                httpContext.Response.StatusCode = StatusCodes.Status409Conflict
            ),
            UnauthorizedException => (
                exception.Message,
                exception.GetType().Name,
                httpContext.Response.StatusCode = StatusCodes.Status401Unauthorized
            ),
            _ => (
                "Something went wrong. Please try again later or contact administrators.",
                "Internal Server Error",
                httpContext.Response.StatusCode = StatusCodes.Status500InternalServerError
            ),
        };

        var problemDetails = new ProblemDetails
        {
            Title = Title,
            Detail = Detail,
            Status = statusCode,
            Instance = requestPath,
        };
        problemDetails.Extensions["traceId"] = traceId;

        //! Unified errors array for all error types: [{ property, message }]
        var errors = new List<object>();
        if (exception is ValidationException validationException)
        {
            foreach (var e in validationException.Errors)
            {
                errors.Add(new { property = e.PropertyName, message = e.ErrorMessage });
            }
        }
        //! Always include errors key (empty array when not validation)
        problemDetails.Extensions["errors"] = errors;

        await httpContext.Response.WriteAsJsonAsync(problemDetails, cancellationToken);
        return true;
    }

    private static bool IsKnownExceptionType(Exception exception)
    {
        return exception switch
        {
            InternalServerException => true,
            BadRequestException => true,
            NotFoundException => true,
            ConflictException => true,
            _ => false,
        };
    }
}
