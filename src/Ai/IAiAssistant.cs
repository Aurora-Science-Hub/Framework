namespace AuroraScienceHub.Framework.Ai;

/// <summary>
/// Assistant that answers questions with a language model
/// </summary>
public interface IAiAssistant
{
    /// <summary>
    /// Asks the model the question of the <paramref name="request"/>
    /// </summary>
    /// <remarks>
    /// Never throws for a failure of the provider: it is reported in <see cref="AiResult.Outcome"/>. Throws
    /// <see cref="OperationCanceledException"/> only when <paramref name="cancellationToken"/> is cancelled.
    /// </remarks>
    Task<AiResult> AskAsync(AiRequest request, CancellationToken cancellationToken);
}
