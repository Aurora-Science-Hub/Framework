using AuroraScienceHub.Framework.Exceptions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace AuroraScienceHub.Framework.AspNetCore.Problems.Enrichers;

/// <summary>
/// Problem enricher for a specific validation exception
/// </summary>
internal sealed class ValidationProblemEnricher : ProblemDetailsEnricher<ValidationException>
{
    private const string ReasonKey = "errors";

    protected override void Enrich(ProblemDetails problem, ValidationException exception)
    {
        problem.Status = StatusCodes.Status400BadRequest;

        if (exception.Reason.Any())
        {
            problem.Extensions.Add(ReasonKey, exception.Reason);
        }
    }
}
