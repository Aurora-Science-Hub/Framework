namespace AuroraScienceHub.Framework.Ai;

/// <summary>
/// Two <see cref="IAiToolProvider"/> implementations returned tools with the same name
/// </summary>
/// <remarks>
/// The conflict is a configuration error, not a failure of the provider, so it is thrown to the caller instead of
/// being reported as an <see cref="AiRequestOutcome"/>.
/// </remarks>
public sealed class AiToolNameConflictException : Exception
{
    public AiToolNameConflictException(IEnumerable<string> toolNames)
        : base($"Several AI tools share the same name: {string.Join(", ", toolNames)}.")
    {
        ToolNames = [.. toolNames];
    }

    /// <summary>
    /// Names returned by more than one provider
    /// </summary>
    public IReadOnlyList<string> ToolNames { get; }
}
