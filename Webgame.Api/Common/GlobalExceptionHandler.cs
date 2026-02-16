using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Data.SqlClient;

namespace Webgame.Api.Common;

public sealed class GlobalExceptionHandler : IExceptionHandler
{
    private readonly ILogger<GlobalExceptionHandler> _logger;
    private readonly IHostEnvironment _env;

    public GlobalExceptionHandler(ILogger<GlobalExceptionHandler> logger, IHostEnvironment env)
    {
        _logger = logger;
        _env = env;
    }
    public async ValueTask<bool> TryHandleAsync(
        HttpContext httpContext,
        Exception exception,
        CancellationToken cancellationToken)
    {
        var traceId = httpContext.TraceIdentifier;

        // 1️. Client cancelled / request aborted → don't treat as error
        if (exception is OperationCanceledException)
        {
            _logger.LogInformation(
                "Request cancelled. TraceId={TraceId} Path={Path}",
                traceId,
                httpContext.Request.Path.Value);

            return true; // nothing to write
        }

        int status;
        string code;
        string message;

        // 2️. Known infrastructure exceptions
        switch (exception)
        {
            case DbUpdateException:
                status = StatusCodes.Status409Conflict;
                code = "database.conflict";
                message = "A database constraint was violated.";
                break;

            case SqlException:
                status = StatusCodes.Status503ServiceUnavailable;
                code = "database.unavailable";
                message = "Database is temporarily unavailable.";
                break;

            default:
                status = StatusCodes.Status500InternalServerError;
                code = "unexpected_error";
                message = "An unexpected error occurred.";
                break;
        }

        // Log severity based on type
        if (status == StatusCodes.Status500InternalServerError)
        {
            _logger.LogError(exception,
                "Unhandled exception. TraceId={TraceId} Path={Path} Method={Method}",
                traceId,
                httpContext.Request.Path.Value,
                httpContext.Request.Method);
        }
        else
        {
            _logger.LogWarning(exception,
                "Handled infrastructure exception ({Code}). TraceId={TraceId}",
                code,
                traceId);
        }

        var problem = new ProblemDetails
        {
            Status = status,
            Title = code,
            Detail = _env.IsDevelopment()
                ? exception.Message
                : message,
            Type = $"https://errors.webgame/{code}",
            Instance = httpContext.Request.Path
        };

        problem.Extensions["traceId"] = traceId;

        httpContext.Response.StatusCode = status;
        httpContext.Response.ContentType = "application/problem+json";

        await httpContext.Response.WriteAsJsonAsync(problem, cancellationToken);
        return true;
    }

}

