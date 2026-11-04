namespace AuroraScienceHub.Framework.Ai.Options;

/// <summary>
/// The provider configured in <see cref="AiOptions.Provider"/> has no client implementation
/// </summary>
public sealed class AiProviderNotSupportedException : Exception
{
    public AiProviderNotSupportedException(AiProvider provider)
        : base($"The AI provider '{provider}' is not supported.")
    {
        Provider = provider;
    }

    /// <summary>
    /// The provider that has no client implementation
    /// </summary>
    public AiProvider Provider { get; }
}
