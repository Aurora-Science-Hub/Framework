using Microsoft.AspNetCore.Mvc;

namespace AuroraScienceHub.Framework.AspNetCore.Problems.Enrichers;

/// <summary>
/// A problem details enricher that can be used to enrich the problem details with additional information from the exception
/// </summary>
public interface IProblemDetailsEnricher
{
    /// <summary>
    /// Enriches the problem details with additional information from the exception
    /// </summary>
    void Enrich(ProblemDetails problem, Exception? exception);
}
