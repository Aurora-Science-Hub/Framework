using AuroraScienceHub.Framework.Exceptions;
using AuroraScienceHub.Framework.Utilities.System;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace AuroraScienceHub.Framework.AspNetCore.Problems.Enrichers;

/// <summary>
/// Problem enricher for a default exception types
/// </summary>
internal sealed class DefaultExceptionProblemEnricher : ProblemDetailsEnricher<Exception>
{
    private static readonly IReadOnlyDictionary<Type, int> s_exceptionStatusCodes = new Dictionary<Type, int>
    {
        [typeof(ArgumentNullException)] = StatusCodes.Status400BadRequest,
        [typeof(ArgumentException)] = StatusCodes.Status400BadRequest,
        [typeof(FormatException)] = StatusCodes.Status400BadRequest,
        [typeof(ValidationException)] = StatusCodes.Status400BadRequest,
        [typeof(AccessDeniedException)] = StatusCodes.Status403Forbidden,
        [typeof(EntityNotFoundException)] = StatusCodes.Status404NotFound,
    };

    /// <summary>
    /// Gets the status code mapped to the exact type of the exception
    /// </summary>
    public static bool TryGetStatusCode(Exception exception, out int statusCode)
        => s_exceptionStatusCodes.TryGetValue(exception.GetType(), out statusCode);

    protected override void Enrich(ProblemDetails problem, Exception exception)
    {
        if (TryGetStatusCode(exception, out var statusCode))
        {
            problem.Status = statusCode;
        }

        problem.Detail = exception.GetFullMessage();
    }
}
