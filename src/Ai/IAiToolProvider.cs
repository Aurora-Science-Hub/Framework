using Microsoft.Extensions.AI;

namespace AuroraScienceHub.Framework.Ai;

/// <summary>
/// Tools the assistant may call while it answers
/// </summary>
internal interface IAiToolProvider
{
    /// <summary>
    /// Tools available now; those of a server that cannot be reached are left out
    /// </summary>
    /// <remarks>
    /// Never throws for an unavailable server, only when <paramref name="cancellationToken"/> is cancelled.
    /// </remarks>
    Task<IReadOnlyList<AITool>> GetToolsAsync(CancellationToken cancellationToken);
}
