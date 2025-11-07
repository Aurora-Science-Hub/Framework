using OpenAI.Chat;

namespace AuroraScienceHub.Framework.Ai.Gpt;

/// <summary>
/// GPT client interface
/// </summary>
public interface IGptClient
{
    /// <summary>
    /// Chat GPT client
    /// </summary>
    ChatClient Chat { get; }

    /// <summary>
    /// Ask a question asynchronously
    /// </summary>
    Task<string?> AskAsync(string message, CancellationToken cancellationToken);
}
