
using System.Text.Json;

namespace LibraryManagementSystem.Api.Middleware;

public sealed class ExceptionMiddleware
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
        catch (Exception exception)
        {
            _logger.LogError(
                exception,
                "An error occurred while processing the request.");

            if (context.Response.HasStarted)
            {
                throw;
            }

            context.Response.Clear();
            context.Response.ContentType = "application/json";

            var (statusCode, message) = exception switch
            {
                KeyNotFoundException =>
                    (StatusCodes.Status404NotFound,
                     exception.Message),

                InvalidOperationException =>
                    (StatusCodes.Status400BadRequest,
                     exception.Message),

                ArgumentException =>
                    (StatusCodes.Status400BadRequest,
                     exception.Message),

                _ =>
                    (StatusCodes.Status500InternalServerError,
                     "An unexpected server error occurred.")
            };

            context.Response.StatusCode = statusCode;

            var response = new
            {
                statusCode,
                message
            };

            await context.Response.WriteAsync(
                JsonSerializer.Serialize(response));
        }
    }
}
