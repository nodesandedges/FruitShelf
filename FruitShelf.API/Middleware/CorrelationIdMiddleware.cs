using Serilog.Context;

namespace FruitShelf.Api.Middleware;

/// <summary>
/// Use existing or add Corelation ID, setting it on the response and using it in the SeriLog logging.
/// </summary>
public class CorrelationIdMiddleware : IMiddleware
{
    private const string HeaderName = "X-Correlation-ID";

    public async Task InvokeAsync(HttpContext context, RequestDelegate next)
    {
        var correlationId = context.Request.Headers[HeaderName].FirstOrDefault() ?? Guid.NewGuid().ToString();
        context.Items[HeaderName] = correlationId;

        context.Response.OnStarting(() =>
        {
            context.Response.Headers["X-Correlation-ID"] = correlationId;
            return Task.CompletedTask;
        });

        // Add the extracted/create correlation id to all the SeriLog logs.
        LogContext.PushProperty("CorrelationId", correlationId);
        await next(context);
    }
}