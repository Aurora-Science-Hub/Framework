using AuroraScienceHub.Framework.Ai;
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
