using AuroraScienceHub.Framework.Ai.Tools;

namespace AuroraScienceHub.Framework.Ai.Assistant;

/// <summary>
/// Assistant that answers questions with a language model
/// </summary>
public interface IAiAssistant
{
    /// <summary>
    /// Asks the model the question of the <paramref name="request"/>
    /// </summary>
    /// <remarks>
    /// A failure of the provider is reported in <see cref="AiResult.Outcome"/>. Errors of the application code are not
    /// converted to provider outcomes.
    /// </remarks>
    /// <exception cref="AiToolNameConflictException">A tool name is used more than once</exception>
    /// <exception cref="OperationCanceledException"><paramref name="cancellationToken"/> is cancelled</exception>
    Task<AiResult> AskAsync(AiRequest request, CancellationToken cancellationToken);
}
