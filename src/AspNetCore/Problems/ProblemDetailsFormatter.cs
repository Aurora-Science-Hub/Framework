using AuroraScienceHub.Framework.AspNetCore.Problems.Enrichers;
using AuroraScienceHub.Framework.Exceptions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace AuroraScienceHub.Framework.AspNetCore.Problems;

/// <summary>
/// Converts an exception to detailed problems
/// </summary>
/// <remarks>https://datatracker.ietf.org/doc/html/rfc7807</remarks>
/// <remarks>
/// By default, all exceptions in the application are considered InternalServerError.
/// If you need different behavior, implement a specific enricher for the exception type.
/// It can be useful only in infrastructure layers (not in domain layers).
/// If you did not pass the input data validation, use <see cref="ValidationException"/>.
/// </remarks>
internal sealed class ProblemDetailsFormatter
{
    private const int DefaultErrorStatusCode = StatusCodes.Status500InternalServerError;
    private readonly ILogger _logger;
    private readonly IReadOnlyList<IProblemDetailsEnricher> _enrichers;

    public ProblemDetailsFormatter(
        IEnumerable<IProblemDetailsEnricher> enrichers,
        ILogger<ProblemDetailsFormatter> logger)
    {
        _logger = logger;
        _enrichers = enrichers.ToList();
    }

    public void Format(ProblemDetails problem, Exception? exception, HttpContext context)
    {
        try
        {
            foreach (var enricher in _enrichers)
            {
                enricher.Enrich(problem, exception);
            }

            context.Response.StatusCode = problem.Status ?? DefaultErrorStatusCode;
        }
        catch (Exception enricherException)
        {
            _logger.LogError(enricherException, "Can't enrich problem details");
        }
    }
}
