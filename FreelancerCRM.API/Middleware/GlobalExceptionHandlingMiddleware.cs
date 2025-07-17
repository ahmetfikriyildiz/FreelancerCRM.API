using System.Net;
using System.Text.Json;
using FreelancerCRM.API.Models;

namespace FreelancerCRM.API.Middleware;

public class GlobalExceptionHandlingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<GlobalExceptionHandlingMiddleware> _logger;
    private readonly IWebHostEnvironment _environment;

    public GlobalExceptionHandlingMiddleware(
        RequestDelegate next,
        ILogger<GlobalExceptionHandlingMiddleware> logger,
        IWebHostEnvironment environment)
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
            await HandleExceptionAsync(context, ex);
        }
    }

    private async Task HandleExceptionAsync(HttpContext context, Exception exception)
    {
        context.Response.ContentType = "application/json";

        var response = new ApiErrorResponse();

        switch (exception)
        {
            case UnauthorizedAccessException:
                context.Response.StatusCode = (int)HttpStatusCode.Unauthorized;
                response.Message = "Unauthorized access";
                response.StatusCode = HttpStatusCode.Unauthorized;
                break;

            case ArgumentException:
                context.Response.StatusCode = (int)HttpStatusCode.BadRequest;
                response.Message = exception.Message;
                response.StatusCode = HttpStatusCode.BadRequest;
                break;

            case InvalidOperationException:
                context.Response.StatusCode = (int)HttpStatusCode.BadRequest;
                response.Message = exception.Message;
                response.StatusCode = HttpStatusCode.BadRequest;
                break;

            case KeyNotFoundException:
                context.Response.StatusCode = (int)HttpStatusCode.NotFound;
                response.Message = "Resource not found";
                response.StatusCode = HttpStatusCode.NotFound;
                break;

            case Microsoft.EntityFrameworkCore.DbUpdateConcurrencyException:
                context.Response.StatusCode = (int)HttpStatusCode.Conflict;
                response.Message = "Concurrency conflict occurred";
                response.StatusCode = HttpStatusCode.Conflict;
                break;

            case Microsoft.EntityFrameworkCore.DbUpdateException:
                context.Response.StatusCode = (int)HttpStatusCode.BadRequest;
                response.Message = "Database operation failed";
                response.StatusCode = HttpStatusCode.BadRequest;
                break;

            default:
                context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
                response.Message = _environment.IsDevelopment() 
                    ? exception.Message 
                    : "An unexpected error occurred";
                response.StatusCode = HttpStatusCode.InternalServerError;
                break;
        }

        // Development ortamında detaylı hata bilgisi
        if (_environment.IsDevelopment())
        {
            response.Details = exception.StackTrace;
            response.ExceptionType = exception.GetType().Name;
        }

        // Log the exception
        _logger.LogError(exception, "An unhandled exception occurred: {Message}", exception.Message);

        var jsonResponse = JsonSerializer.Serialize(response, new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        });

        await context.Response.WriteAsync(jsonResponse);
    }
}

public class ApiErrorResponse
{
    public string Message { get; set; } = string.Empty;
    public HttpStatusCode StatusCode { get; set; }
    public string? Details { get; set; }
    public string? ExceptionType { get; set; }
    public DateTime Timestamp { get; set; } = DateTime.UtcNow;
} 