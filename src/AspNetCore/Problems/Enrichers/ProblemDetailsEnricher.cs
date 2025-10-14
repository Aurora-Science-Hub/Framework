using Microsoft.AspNetCore.Mvc;

namespace AuroraScienceHub.Framework.AspNetCore.Problems.Enrichers;

/// <summary>
/// <see cref="ProblemDetails"/> enricher for the specific exception type
/// </summary>
public abstract class ProblemDetailsEnricher<T> : IProblemDetailsEnricher
    where T : Exception
{
    /// <summary>
    /// Enrich the problem for a specific exception
    /// </summary>
    protected abstract void Enrich(ProblemDetails problem, T exception);

    void IProblemDetailsEnricher.Enrich(ProblemDetails problem, Exception? exception)
    {
        if (exception is T targetException)
        {
            Enrich(problem, targetException);
        }
    }
}
