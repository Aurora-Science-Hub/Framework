using Microsoft.Extensions.AI;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace AuroraScienceHub.Framework.Ai;

/// <summary>
/// Runs a call to the language model within the time budget and turns its outcome into an <see cref="AiResult"/>
/// </summary>
internal sealed class AiRequestExecutor
{
    private readonly IOptionsMonitor<AiOptions> _options;
    private readonly TimeProvider _timeProvider;
    private readonly ILogger<AiRequestExecutor> _logger;

    public AiRequestExecutor(IOptionsMonitor<AiOptions> options, TimeProvider timeProvider, ILogger<AiRequestExecutor> logger)
    {
        _options = options;
        _timeProvider = timeProvider;
        _logger = logger;
    }

    /// <summary>
    /// Runs <paramref name="call"/>; never throws for a failure of the provider, only when
    /// <paramref name="cancellationToken"/> is cancelled
    /// </summary>
    /// <param name="operation">Name of the operation for the logs</param>
    /// <param name="call">The call; returns the answer text and the usage</param>
    /// <param name="cancellationToken">Cancellation token of the caller</param>
    public async Task<AiResult> ExecuteAsync(
        string operation,
        Func<CancellationToken, Task<(string? Text, UsageDetails? Usage)>> call,
        CancellationToken cancellationToken)
    {
        using var timeout = new CancellationTokenSource(_options.CurrentValue.RequestTimeout, _timeProvider);
        using var linked = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken, timeout.Token);
        var startedAt = _timeProvider.GetTimestamp();

        AiResult result;
        try
        {
            var (text, usage) = await call(linked.Token);
            var aiUsage = usage is null ? null : new AiUsage(usage.InputTokenCount, usage.OutputTokenCount);
            result = string.IsNullOrWhiteSpace(text)
                ? AiResult.Failed(AiRequestOutcome.Empty, aiUsage)
                : new AiResult(AiRequestOutcome.Success, text, aiUsage);
        }
        catch (Exception exception) when (cancellationToken.IsCancellationRequested)
        {
            _logger.LogInformation(
                "AI {Operation} aborted after {ElapsedMs} ms",
                operation,
                _timeProvider.GetElapsedTime(startedAt).TotalMilliseconds);

            if (exception is OperationCanceledException)
            {
                throw;
            }

            throw new OperationCanceledException("The AI request was aborted", exception, cancellationToken);
        }
        catch (AiToolNameConflictException)
        {
            // A configuration error, not a failure of the provider: the caller must see it
            throw;
        }
        catch (Exception exception)
        {
            var outcome = AiFailureClassifier.Classify(exception);
            var providerError = AiFailureClassifier.FindProviderError(exception);
            _logger.LogWarning(
                "AI {Operation} failed: {Outcome} ({ExceptionType}, HTTP {ProviderStatus}: {ProviderMessage}) after {ElapsedMs} ms",
                operation,
                outcome,
                exception.GetType().Name,
                providerError?.Status,
                providerError?.Message,
                _timeProvider.GetElapsedTime(startedAt).TotalMilliseconds);
            return AiResult.Failed(outcome);
        }

        _logger.LogInformation(
            "AI {Operation} finished: {Outcome} in {ElapsedMs} ms, {InputTokens} input and {OutputTokens} output tokens",
            operation,
            result.Outcome,
            _timeProvider.GetElapsedTime(startedAt).TotalMilliseconds,
            result.Usage?.InputTokens,
            result.Usage?.OutputTokens);

        return result;
    }
}
