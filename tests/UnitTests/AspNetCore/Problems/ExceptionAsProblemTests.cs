using System.Diagnostics;
using System.Text.Json;
using AuroraScienceHub.Framework.AspNetCore.Problems;
using AuroraScienceHub.Framework.Exceptions;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Shouldly;

namespace AuroraScienceHub.Framework.UnitTests.AspNetCore.Problems;

/// <summary>
/// Tests for <see cref="ServiceCollectionExtensions.AddExceptionAsProblem"/> and
/// <see cref="ServiceCollectionExtensions.UseExceptionAsProblem"/> through the ASP.NET Core exception handler pipeline
/// </summary>
public sealed class ExceptionAsProblemTests
{
    private const string ErrorsKey = "errors";
    private const string StatusKey = "status";
    private const string DetailKey = "detail";

    [Fact(DisplayName = "ValidationException returns 400 with its validation errors")]
    public async Task Handle_ValidationException_Returns400WithErrors()
    {
        // Arrange
        var exception = new ValidationException(new ValidationExceptionReason("name", "Name is required"));

        // Act
        var (statusCode, problem) = await HandleAsync(exception);

        // Assert
        statusCode.ShouldBe(StatusCodes.Status400BadRequest);
        problem.GetProperty(StatusKey).GetInt32().ShouldBe(StatusCodes.Status400BadRequest);
        problem.GetProperty(ErrorsKey).GetProperty("name")[0].GetString().ShouldBe("Name is required");
    }

    [Fact(DisplayName = "EntityNotFoundException returns 404 with its message")]
    public async Task Handle_EntityNotFoundException_Returns404()
    {
        // Arrange
        var exception = new EntityNotFoundException("Order not found");

        // Act
        var (statusCode, problem) = await HandleAsync(exception);

        // Assert
        statusCode.ShouldBe(StatusCodes.Status404NotFound);
        problem.GetProperty(StatusKey).GetInt32().ShouldBe(StatusCodes.Status404NotFound);
        problem.GetProperty(DetailKey).GetString().ShouldBe("Order not found");
    }

    [Fact(DisplayName = "AccessDeniedException returns 403 with its message")]
    public async Task Handle_AccessDeniedException_Returns403()
    {
        // Arrange
        var exception = new AccessDeniedException("Access to the order is denied");

        // Act
        var (statusCode, problem) = await HandleAsync(exception);

        // Assert
        statusCode.ShouldBe(StatusCodes.Status403Forbidden);
        problem.GetProperty(StatusKey).GetInt32().ShouldBe(StatusCodes.Status403Forbidden);
        problem.GetProperty(DetailKey).GetString().ShouldBe("Access to the order is denied");
    }

    [Fact(DisplayName = "Unmapped ValidationException subclass returns 400 with its validation errors")]
    public async Task Handle_CustomValidationException_Returns400WithErrors()
    {
        // Arrange
        var exception = new CustomValidationException(new ValidationExceptionReason("threshold", "Out of range"));

        // Act
        var (statusCode, problem) = await HandleAsync(exception);

        // Assert
        statusCode.ShouldBe(StatusCodes.Status400BadRequest);
        problem.GetProperty(StatusKey).GetInt32().ShouldBe(StatusCodes.Status400BadRequest);
        problem.GetProperty(ErrorsKey).GetProperty("threshold")[0].GetString().ShouldBe("Out of range");
    }

    [Fact(DisplayName = "Unmapped exception returns 500")]
    public async Task Handle_UnmappedException_Returns500()
    {
        // Arrange
        var exception = new InvalidOperationException("Unexpected state");

        // Act
        var (statusCode, problem) = await HandleAsync(exception);

        // Assert
        statusCode.ShouldBe(StatusCodes.Status500InternalServerError);
        problem.GetProperty(StatusKey).GetInt32().ShouldBe(StatusCodes.Status500InternalServerError);
    }

    private static async Task<(int StatusCode, JsonElement Problem)> HandleAsync(Exception exception)
    {
        var services = new ServiceCollection()
            .AddLogging()
            .AddMetrics()
            .AddSingleton(new DiagnosticListener(nameof(ExceptionAsProblemTests)))
            .AddExceptionAsProblem();
        await using var serviceProvider = services.BuildServiceProvider();

        var app = new ApplicationBuilder(serviceProvider);
        app.UseExceptionAsProblem(useExceptionPage: false);
        app.Run(_ => throw exception);
        var pipeline = app.Build();

        await using var scope = serviceProvider.CreateAsyncScope();
        using var body = new MemoryStream();
        var context = new DefaultHttpContext { RequestServices = scope.ServiceProvider };
        context.Request.Method = HttpMethods.Get;
        context.Response.Body = body;

        await pipeline(context);

        body.Position = 0;
        using var document = await JsonDocument.ParseAsync(body, cancellationToken: TestContext.Current.CancellationToken);

        return (context.Response.StatusCode, document.RootElement.Clone());
    }

    private sealed class CustomValidationException : ValidationException
    {
        public CustomValidationException(ValidationExceptionReason reason)
            : base("Custom validation failed", reason)
        {
        }
    }
}
