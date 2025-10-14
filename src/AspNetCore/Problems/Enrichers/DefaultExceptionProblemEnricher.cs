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

    protected override void Enrich(ProblemDetails problem, Exception exception)
    {
        if (s_exceptionStatusCodes.TryGetValue(exception.GetType(), out var statusCode))
        {
            problem.Status = statusCode;
        }

        problem.Detail = exception.GetFullMessage();
    }
}
