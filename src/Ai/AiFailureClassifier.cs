using System.ClientModel;

namespace AuroraScienceHub.Framework.Ai;

/// <summary>
/// Maps a failure of a request to the provider to an <see cref="AiRequestOutcome"/>
/// </summary>
internal static class AiFailureClassifier
{
    /// <summary>
    /// Outcome of a request that failed with <paramref name="exception"/>; a cancellation by the caller must be handled
    /// before, it is not a failure
    /// </summary>
    public static AiRequestOutcome Classify(Exception exception)
        => exception switch
        {
            // The SDK reports every attempt when its retries are exhausted; the last one decides
            AggregateException { InnerExceptions.Count: > 0 } aggregate => Classify(aggregate.InnerExceptions[^1]),
            ClientResultException { Status: 401 or 403 } => AiRequestOutcome.Unauthorized,
            ClientResultException { Status: 402 } => AiRequestOutcome.InsufficientBalance,
            ClientResultException { Status: 408 } => AiRequestOutcome.Timeout,
            ClientResultException { Status: 429 } => AiRequestOutcome.RateLimited,
            // Status 0: no response at all (the transport failed)
            ClientResultException { Status: 0 or >= 500 } => AiRequestOutcome.Unavailable,
            ClientResultException => AiRequestOutcome.Rejected,
            OperationCanceledException or TimeoutException => AiRequestOutcome.Timeout,
            _ => AiRequestOutcome.Unavailable,
        };

    /// <summary>
    /// The error the provider answered with (status and message, e.g. "model not found"), if the failure has one; it
    /// never contains the API key
    /// </summary>
    public static ClientResultException? FindProviderError(Exception exception)
        => exception switch
        {
            AggregateException { InnerExceptions.Count: > 0 } aggregate => FindProviderError(aggregate.InnerExceptions[^1]),
            ClientResultException providerError => providerError,
            _ => null,
        };
}
