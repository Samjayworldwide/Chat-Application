using System.Diagnostics.CodeAnalysis;
using ChatApp.Application.commons;
using Microsoft.AspNetCore.Diagnostics;

namespace ChatApp.WebApi.Middleware;

[SuppressMessage("ReSharper", "ConvertToPrimaryConstructor")]
[SuppressMessage("Performance", "CA1873:Avoid potentially expensive logging")]
public class GlobalExceptionHandlerMiddleware : IExceptionHandler
{
    private readonly ILogger<GlobalExceptionHandlerMiddleware> _logger;

    private readonly string _time = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");

    public GlobalExceptionHandlerMiddleware(ILogger<GlobalExceptionHandlerMiddleware> logger)
    {
        _logger = logger;
    }

    public async ValueTask<bool> TryHandleAsync(HttpContext httpContext, Exception exception,
        CancellationToken cancellationToken)
    {
        _logger.LogError(exception, "Unhandled exception occurred at {}", _time);

        httpContext.Response.StatusCode = StatusCodes.Status500InternalServerError;

        httpContext.Response.ContentType = "application/json";

        await httpContext.Response.WriteAsJsonAsync(Result<string>.Failure("An unexpected error occurred"),
            cancellationToken: cancellationToken);

        return true;
    }
}