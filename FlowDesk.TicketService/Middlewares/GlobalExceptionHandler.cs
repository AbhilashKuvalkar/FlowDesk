using FlowDesk.TicketService.Domain.Exceptions;
using FluentValidation;
using Microsoft.AspNetCore.Diagnostics;

namespace FlowDesk.TicketService.Middlewares;

public class GlobalExceptionHandler : IExceptionHandler
{
    private readonly ILogger<GlobalExceptionHandler> _logger;

    public GlobalExceptionHandler(ILogger<GlobalExceptionHandler> logger)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public async ValueTask<bool> TryHandleAsync(HttpContext httpContext, Exception exception, CancellationToken cancellationToken)
    {
        var (statusCode, message) = exception switch
        {
            TicketNotFoundException ex => (StatusCodes.Status404NotFound, ex.Message),
            AgentNotFoundException ex => (StatusCodes.Status404NotFound, ex.Message),
            InvalidOperationException ex => (StatusCodes.Status409Conflict, ex.Message),
            ValidationException ex => (StatusCodes.Status400BadRequest, string.Join(", ", ex.Errors.Select(x => x.ErrorMessage))),
            _ => (StatusCodes.Status500InternalServerError, "An unexpected error occurred.")
        };

        _logger.LogError(exception, "Handled Exception: {Message}", message);

        httpContext.Response.StatusCode = statusCode;

        await httpContext.Response.WriteAsJsonAsync(new
        {
            statusCode = statusCode,
            error = message
        }, cancellationToken);

        return true;
    }
}
