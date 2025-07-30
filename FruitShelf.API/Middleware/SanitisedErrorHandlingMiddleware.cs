namespace FruitShelf.Api.Middleware;

/// <summary>
/// Handles unexpected exceptions and converts to an HTTP status,
/// but intentionally conceals internal server errors from
/// potentially hostile external clients.
/// </summary>
/// <param name="next"></param>
/// <param name="logger"></param>
/// <param name="env"></param>
public class SanitisedErrorHandlingMiddleware(
    RequestDelegate next,
    ILogger<SanitisedErrorHandlingMiddleware> logger,
    IWebHostEnvironment env)
{
    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await next(context);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "An unhandled exception has occurred.");
            await HandleExceptionAsync(context, ex);
        }
    }

    private Task HandleExceptionAsync(HttpContext context, Exception exception)
    {
        context.Response.ContentType = "application/json";
        context.Response.StatusCode = exception switch
        {
            ArgumentNullException _ => StatusCodes.Status400BadRequest,
            ArgumentException _ => StatusCodes.Status400BadRequest,
            UnauthorizedAccessException _ => StatusCodes.Status401Unauthorized,
            _ => StatusCodes
                .Status400BadRequest, // Avoid exposing that we potentially have an internal fault, by return a 400.
        };

        // Vary the information returned depending on environment.
        
        var response = new
        {
            message = env.IsDevelopment() ? exception.Message : "Request was invalid",
            details = env.IsDevelopment() ? exception.StackTrace : null
        };

        return context.Response.WriteAsJsonAsync(response);
    }
}