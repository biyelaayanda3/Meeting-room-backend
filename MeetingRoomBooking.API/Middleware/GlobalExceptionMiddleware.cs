using MeetingRoomBooking.Domain.Exceptions;
using System.Text.Json;

namespace MeetingRoomBooking.API.Middleware;

public class GlobalExceptionMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<GlobalExceptionMiddleware> _logger;
    private readonly IHostEnvironment _environment;

    public GlobalExceptionMiddleware(RequestDelegate next, ILogger<GlobalExceptionMiddleware> logger,
        IHostEnvironment environment)
    {
        _next = next;
        _logger = logger;
        _environment = environment;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex,"Unhandled exception for {Method} {Path}", context.Request.Method, context.Request.Path);
            await HandleExceptionAsync(context, ex);
        }
    }

    private async Task HandleExceptionAsync(HttpContext context, Exception ex)
    {
        context.Response.ContentType = "application/json";

        //deconstruction for readability
        var (statusCode, message) = ex switch
        {
            NotFoundException =>
                (404, ex.Message),
            BookingConflictException =>
                (409, ex.Message),
            ValidationException =>
                (400, ex.Message),
            DuplicateException =>
                (409, ex.Message),
            UnauthorizedAccessException =>
                (401, "Unauthorized"),
            _ => (500, _environment.IsDevelopment() ? ex.Message : "An unexpected error occurred")
            //In Dev we show the actual exception message but in Prod we hide details for hackers, just show a generic message
        };

        context.Response.StatusCode = statusCode;

        var response = new
        {
            statusCode,
            message,
            timestamp = DateTime.UtcNow
        };

        await context.Response.WriteAsync( JsonSerializer.Serialize(
            response, new JsonSerializerOptions{PropertyNamingPolicy = JsonNamingPolicy.CamelCase}));
    }
}
