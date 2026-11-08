using AuroraScienceHub.Framework.AspNetCore.Problems;
using AuroraScienceHub.Framework.AspNetCore.Problems.Enrichers;
using AuroraScienceHub.Framework.Exceptions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging.Abstractions;
using Shouldly;

namespace AuroraScienceHub.Framework.UnitTests.AspNetCore.Problems;

/// <summary>
/// Tests for <see cref="ProblemDetailsFormatter"/>
/// </summary>
public sealed class ProblemDetailsFormatterTests
{
    public static TheoryData<bool> EnricherOrders => new() { true, false };

    [Theory(DisplayName = "Status code of a ValidationException subclass does not depend on the enricher order")]
    [MemberData(nameof(EnricherOrders))]
    public void Format_EntityNotFoundException_Returns404RegardlessOfEnricherOrder(bool defaultEnricherFirst)
    {
        // Arrange
        IProblemDetailsEnricher[] enrichers = defaultEnricherFirst
            ? [new DefaultExceptionProblemEnricher(), new ValidationProblemEnricher()]
            : [new ValidationProblemEnricher(), new DefaultExceptionProblemEnricher()];
        var formatter = new ProblemDetailsFormatter(enrichers, NullLogger<ProblemDetailsFormatter>.Instance);
        var problem = new ProblemDetails();
        var context = new DefaultHttpContext();

        // Act
        formatter.Format(problem, new EntityNotFoundException("Order not found"), context);

        // Assert
        problem.Status.ShouldBe(StatusCodes.Status404NotFound);
        context.Response.StatusCode.ShouldBe(StatusCodes.Status404NotFound);
    }
}
