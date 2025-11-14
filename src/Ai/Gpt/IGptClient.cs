namespace AuroraScienceHub.Framework.Ai.Gpt;

/// <summary>
/// GPT client interface
/// </summary>
public interface IGptClient
{
    /// <summary>
    /// Ask a question asynchronously
    /// </summary>
    Task<string?> AskAsync(string message, CancellationToken cancellationToken = default);

    /// <summary>
    /// Sends a request to the GPT client and deserializes the response into the specified type.
    /// </summary>
    /// <remarks>
    /// The GPT client is instructed to respond in valid JSON format.
    /// </remarks>
    Task<TResult> RequestAndDeserializeAsync<TResult>(
        string systemMessage,
        string prompt,
        CancellationToken cancellationToken)
        where TResult : class;
}
