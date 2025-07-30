using System.Diagnostics;

namespace FruitShelf.Api.Middleware;

public class RequestLoggingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<RequestLoggingMiddleware> _logger;

    public RequestLoggingMiddleware(RequestDelegate next, ILogger<RequestLoggingMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        var sw = Stopwatch.StartNew();
        var request = context.Request;

        _logger.LogInformation("Starting {Method} {Path}{QueryString}", request.Method, request.Path,
            request.QueryString);

        try
        {
            await _next(context);
        }
        catch (Exception ex)
        {
            // Log unhandled exception along with timing
            sw.Stop();
            _logger.LogError(ex, "Exception thrown in {Method} {Path} after {Elapsed} ms",
                request.Method, request.Path, sw.ElapsedMilliseconds);
            throw;
        }

        sw.Stop();

        var response = context.Response;
        _logger.LogInformation("Completed {StatusCode} in {Elapsed} ms for {Method} {Path}{QueryString}",
            response.StatusCode, sw.ElapsedMilliseconds, request.Method, request.Path, request.QueryString);
    }
}