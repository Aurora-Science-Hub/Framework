using Microsoft.Extensions.AI;

namespace AuroraScienceHub.Framework.Ai.Tools;

/// <summary>
/// Tools the assistant may call while it answers
/// </summary>
/// <remarks>
/// Every registered implementation is used: <see cref="McpToolProvider"/> supplies the servers configured in
/// <see cref="AiOptions.Mcp"/>, a service may add its own provider, e.g. for an MCP server that needs authentication.
/// Tools with the same name from two providers are reported as <see cref="AiToolNameConflictException"/>.
/// </remarks>
public interface IAiToolProvider
{
    /// <summary>
    /// Tools available now; those of a server that cannot be reached are left out
    /// </summary>
    /// <remarks>
    /// Never throws for an unavailable server, only when <paramref name="cancellationToken"/> is cancelled.
    /// </remarks>
    Task<IReadOnlyList<AITool>> GetToolsAsync(CancellationToken cancellationToken);
}
