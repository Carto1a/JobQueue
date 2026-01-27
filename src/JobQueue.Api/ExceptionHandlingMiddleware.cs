using JobQueue.Domain;
using Microsoft.AspNetCore.Mvc;

namespace JobQueue.Api;

public class ExceptionHandlingMiddleware(RequestDelegate next, ILogger<ExceptionHandlingMiddleware> logger)
{
    private readonly RequestDelegate _next = next;
    private readonly ILogger _logger = logger;

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (Exception exception)
        {
            await HandleException(context, exception);
        }
    }

    private async Task HandleException(HttpContext context, Exception exception)
    {
        var problem = exception switch
        {
            DomainException or ApplicationException => CreateProblem(
                StatusCodes.Status400BadRequest,
                "Application Error",
                exception.Message
            ),
            _ => CreateProblem(
                StatusCodes.Status500InternalServerError,
                "Internal Server Error",
                "An unexpected error occurred"
            )
        };

        switch (problem.Status!.Value)
        {
            case StatusCodes.Status400BadRequest:
                _logger.LogWarning(exception, "Application error: {message}", exception.Message);
                break;
            case StatusCodes.Status500InternalServerError:
                _logger.LogCritical(exception, "Critical error: {message}", exception.Message);
                break;
            default:
                _logger.LogError(exception, "Error: {message}", exception.Message);
                break;
        }

        context.Response.StatusCode = problem.Status.Value;
        context.Response.ContentType = "application/problem+json";
        await context.Response.WriteAsJsonAsync(problem);
    }

    private static ProblemDetails CreateProblem(int status, string title, string detail) =>
        new() { Status = status, Title = title, Detail = detail };
}
