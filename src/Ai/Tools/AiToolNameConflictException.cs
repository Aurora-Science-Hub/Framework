using AuroraScienceHub.Framework.Ai.Assistant;

namespace AuroraScienceHub.Framework.Ai.Tools;

/// <summary>
/// The same tool name is used more than once: by two <see cref="IAiToolProvider"/> implementations, or by a request
/// and a provider
/// </summary>
/// <remarks>
/// The conflict is a configuration error, not a failure of the provider, so it is thrown to the caller instead of
/// being reported as an <see cref="AiRequestOutcome"/>.
/// </remarks>
public sealed class AiToolNameConflictException : Exception
{
    public AiToolNameConflictException(IEnumerable<string> toolNames)
        : this(Materialize(toolNames))
    {
    }

    private AiToolNameConflictException(IReadOnlyList<string> toolNames)
        : base($"Several AI tools share the same name: {string.Join(", ", toolNames)}.")
    {
        ToolNames = toolNames;
    }

    /// <summary>
    /// Names used more than once
    /// </summary>
    public IReadOnlyList<string> ToolNames { get; }

    // The names are joined for the message and kept for the caller, so they are read once
    private static string[] Materialize(IEnumerable<string> toolNames) => [.. toolNames];
}
