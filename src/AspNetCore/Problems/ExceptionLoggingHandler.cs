using AuroraScienceHub.Framework.Exceptions;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace AuroraScienceHub.Framework.AspNetCore.Problems;

/// <summary>
/// Exception handler that logs exceptions using the provided logger.
/// </summary>
internal sealed class ExceptionLoggingHandler : IExceptionHandler
{
    private readonly ILogger _logger;

    public ExceptionLoggingHandler(ILogger<ExceptionLoggingHandler> logger)
    {
        _logger = logger;
    }

    public ValueTask<bool> TryHandleAsync(
        HttpContext httpContext,
        Exception exception,
        CancellationToken cancellationToken)
    {
        const string message = "An unhandled exception has occurred while executing the request";

        if (exception is ValidationException or OperationCanceledException)
        {
            _logger.LogInformation(exception, message);
        }
        else
        {
            _logger.LogError(exception, message);
        }

        // Always return false to allow other handlers to process the exception
        return ValueTask.FromResult(false);
    }
}
