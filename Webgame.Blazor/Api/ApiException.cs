using System.Net;

namespace Webgame.Blazor.Api;

public sealed class ApiException : Exception
{
    public HttpStatusCode StatusCode { get; }
    public ApiProblemDetails? Problem { get; }

    public ApiException(HttpStatusCode statusCode, ApiProblemDetails? problem)
        : base(problem?.Title ?? $"API Error ({(int)statusCode})")
    {
        StatusCode = statusCode;
        Problem = problem;
    }

    public string UserMessage =>
        StatusCode == HttpStatusCode.Unauthorized
            ? "Your session expired. Please log in again."
            : Problem?.Detail
            ?? Problem?.Title
            ?? $"Request failed ({(int)StatusCode}).";

    public string Trace =>
        Problem?.TraceId ?? "(no traceId)";

    public string StatusText =>
        $"{(int)StatusCode} {StatusCode}";

    public override string ToString()
    {
        return $"{StatusText}: {UserMessage} (trace: {Trace})";
    }
}