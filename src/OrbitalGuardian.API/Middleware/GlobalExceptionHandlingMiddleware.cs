using System.Text.Json;
using OrbitalGuardian.Domain.Exceptions;
using OrbitalGuardian.Domain.Shared;

namespace OrbitalGuardian.API.Middleware;

/// <summary>Centralizes handling of all unhandled exceptions, mapping domain exceptions to HTTP status codes.</summary>
public class GlobalExceptionHandlingMiddleware
{
    private readonly RequestDelegate _next;

    public GlobalExceptionHandlingMiddleware(RequestDelegate next)
    {
        _next = next;
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

    private static async Task HandleExceptionAsync(HttpContext context, Exception ex)
    {
        var statusCode = ex switch
        {
            UserNotFoundException => StatusCodes.Status404NotFound,
            SpaceObjectNotFoundException => StatusCodes.Status404NotFound,
            InvalidCredentialsException => StatusCodes.Status401Unauthorized,
            DuplicateEmailException => StatusCodes.Status409Conflict,
            InvalidStateTransitionException => StatusCodes.Status422UnprocessableEntity,
            OrbitalGuardianDomainException => StatusCodes.Status400BadRequest,
            NotImplementedException => StatusCodes.Status501NotImplemented,
            _ => StatusCodes.Status500InternalServerError
        };

        if (statusCode >= 500)
            OrbitalLogger.LogError("GlobalExceptionHandler", ex.Message, ex);

        var response = new
        {
            error = ex.Message,
            type = ex.GetType().Name,
            timestamp = DateTime.UtcNow
        };

        context.Response.ContentType = "application/json";
        context.Response.StatusCode = statusCode;

        await context.Response.WriteAsync(JsonSerializer.Serialize(response));
    }
}
