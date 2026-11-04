using System.ClientModel;
using AuroraScienceHub.Framework.Ai;
using AuroraScienceHub.Framework.UnitTests.Ai.TestDoubles;
using Shouldly;
using Xunit;

namespace AuroraScienceHub.Framework.UnitTests.Ai;

/// <summary>
/// Tests for <see cref="AiFailureClassifier"/>
/// </summary>
public sealed class AiFailureClassifierTests
{
    [Theory(DisplayName = "HTTP statuses of the provider map to outcomes")]
    [InlineData(401, AiRequestOutcome.Unauthorized)]
    [InlineData(403, AiRequestOutcome.Unauthorized)]
    [InlineData(402, AiRequestOutcome.InsufficientBalance)]
    [InlineData(408, AiRequestOutcome.Timeout)]
    [InlineData(429, AiRequestOutcome.RateLimited)]
    [InlineData(500, AiRequestOutcome.Unavailable)]
    [InlineData(503, AiRequestOutcome.Unavailable)]
    [InlineData(400, AiRequestOutcome.Rejected)]
    [InlineData(422, AiRequestOutcome.Rejected)]
    public void Classify_Status_MapsToOutcome(int status, AiRequestOutcome expected)
        => AiFailureClassifier.Classify(new ClientResultException("failed", new FakePipelineResponse(status)))
            .ShouldBe(expected);

    [Fact(DisplayName = "A request without a response means the provider is unavailable")]
    public void Classify_NoResponse_Unavailable()
        => AiFailureClassifier.Classify(new ClientResultException("no response", response: null)).ShouldBe(AiRequestOutcome.Unavailable);

    [Fact(DisplayName = "A timeout of the SDK is a timeout")]
    public void Classify_SdkTimeout_Timeout()
        => AiFailureClassifier.Classify(new TaskCanceledException("The operation exceeded the configured timeout")).ShouldBe(AiRequestOutcome.Timeout);

    [Fact(DisplayName = "When retries are exhausted the last attempt decides")]
    public void Classify_Aggregate_UsesLastAttempt()
        => AiFailureClassifier.Classify(new AggregateException(
                new ClientResultException("server", new FakePipelineResponse(503)),
                new ClientResultException("limit", new FakePipelineResponse(429))))
            .ShouldBe(AiRequestOutcome.RateLimited);

    [Fact(DisplayName = "Any other failure means the provider is unavailable")]
    public void Classify_Other_Unavailable()
        => AiFailureClassifier.Classify(new HttpRequestException("connection refused")).ShouldBe(AiRequestOutcome.Unavailable);

    [Fact(DisplayName = "The provider's own error is found for the logs, also after retries")]
    public void FindProviderError_ReturnsLastAttempt()
    {
        // Arrange
        var rejected = new ClientResultException("Model Not Exist", new FakePipelineResponse(400));

        // Act & Assert
        AiFailureClassifier.FindProviderError(new AggregateException(new HttpRequestException("first"), rejected)).ShouldBe(rejected);
        AiFailureClassifier.FindProviderError(new TaskCanceledException()).ShouldBeNull();
    }
}
