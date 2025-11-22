using ProductManagement.Application.Exceptions;

namespace ProductManagement.API.Middlewares;

public class ExceptionHandlingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<ExceptionHandlingMiddleware> _logger;

    public ExceptionHandlingMiddleware(
        RequestDelegate next,
        ILogger<ExceptionHandlingMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext httpContext)
    {
        try
        {
            await _next(httpContext);
        }
        catch (Exception exception)
        {
            await HandleExceptionAsync(httpContext, exception);
        }
    }

    private Task HandleExceptionAsync(HttpContext httpContext, Exception exception)
    {
        httpContext.Response.ContentType = "application/json";

        var response = httpContext.Response;

        response.StatusCode = exception switch
        {
            UnauthorizedException => StatusCodes.Status401Unauthorized,
            NotFoundException => StatusCodes.Status404NotFound,
            _ => StatusCodes.Status500InternalServerError
        };

        var errorResponse = new
        {
            StatusCode = response.StatusCode,
            Message = exception.Message,
            Timestamp = DateTime.UtcNow
        };

        return httpContext.Response.WriteAsJsonAsync(errorResponse);
    }
}