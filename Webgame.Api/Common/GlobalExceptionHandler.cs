using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;

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

        // Log with trace id and exception details
        _logger.LogError(exception,
            "Unhandled exception. TraceId={TraceId} Path={Path} Method={Method}",
            traceId,
            httpContext.Request.Path.Value,
            httpContext.Request.Method);

        var problem = new ProblemDetails
        {
            Status = StatusCodes.Status500InternalServerError,
            Title = "unexpected_error",
            Detail = _env.IsDevelopment() ? exception.ToString() : "An unexpected error occurred.",
            Type = "https://errors.webgame/unexpected_error"
        };

        problem.Extensions["traceId"] = traceId;

        httpContext.Response.StatusCode = problem.Status.Value;
        httpContext.Response.ContentType = "application/problem+json";

        await httpContext.Response.WriteAsJsonAsync(problem, cancellationToken);
        return true; // exception handled
    }
}

