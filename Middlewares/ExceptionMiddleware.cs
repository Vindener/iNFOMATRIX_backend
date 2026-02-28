using Infomatrix.Api.Domain;

namespace Infomatrix.Api.Middlewares;

public class ExceptionMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<ExceptionMiddleware> _logger;

    public ExceptionMiddleware(
        RequestDelegate next,
        ILogger<ExceptionMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (AuthenticationException ex)
        {
            _logger.LogWarning(
                ex,
                "Authentication failed: {Message}",
                ex.Message);

            context.Response.StatusCode = ex.StatusCode;
            await context.Response
                .WriteAsJsonAsync(new { error = ex.Message });
        }
        catch (DomainException ex)
        {
            _logger.LogWarning(
                ex,
                "Domain validation failed: {Message}",
                ex.Message);

            context.Response.StatusCode = StatusCodes.Status400BadRequest;
            await context.Response
                .WriteAsJsonAsync(new { error = ex.Message });
        }
        catch (Exception ex)
        {
            _logger.LogError(
                ex,
                "Unhandled exception occurred.");

            if (context.Response.HasStarted) return;

            context.Response.StatusCode = StatusCodes.Status500InternalServerError;
            await context.Response
                .WriteAsJsonAsync(new { error = "Internal server error" });
        }
    }
}
