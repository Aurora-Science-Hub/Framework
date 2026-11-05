using AuroraScienceHub.Framework.Ai;
using AuroraScienceHub.Framework.Ai.Assistant;
using AuroraScienceHub.Framework.Ai.Conversation;
using AuroraScienceHub.Framework.Ai.Mcp;
using AuroraScienceHub.Framework.Ai.Options;
using AuroraScienceHub.Framework.Ai.Tools;
using Microsoft.Extensions.AI;

namespace AuroraScienceHub.Framework.UnitTests.Ai.TestDoubles;

/// <summary>
/// Tool provider with a configured set of tools
/// </summary>
internal sealed class FakeToolProvider : IAiToolProvider
{
    public List<AITool> Tools { get; } = [];

    public Task<IReadOnlyList<AITool>> GetToolsAsync(CancellationToken cancellationToken)
        => Task.FromResult<IReadOnlyList<AITool>>(Tools);
}
