namespace AuroraScienceHub.Framework.Ai.Assistant;

/// <summary>
/// Outcome of a request to the language model
/// </summary>
public enum AiRequestOutcome
{
    /// <summary>
    /// The model answered
    /// </summary>
    Success,

    /// <summary>
    /// The model did not answer within the time budget
    /// </summary>
    Timeout,

    /// <summary>
    /// The provider rejected the API key
    /// </summary>
    Unauthorized,

    /// <summary>
    /// The provider account has no funds left
    /// </summary>
    InsufficientBalance,

    /// <summary>
    /// The provider limited the request rate
    /// </summary>
    RateLimited,

    /// <summary>
    /// The provider could not be reached or failed
    /// </summary>
    Unavailable,

    /// <summary>
    /// The provider rejected the request itself (e.g. invalid parameters)
    /// </summary>
    Rejected,

    /// <summary>
    /// The model answered with no text
    /// </summary>
    Empty,
}
