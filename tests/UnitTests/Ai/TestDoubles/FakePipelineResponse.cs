using System.ClientModel.Primitives;
using AuroraScienceHub.Framework.Ai;
using AuroraScienceHub.Framework.Ai.Assistant;
using AuroraScienceHub.Framework.Ai.Conversation;
using AuroraScienceHub.Framework.Ai.Mcp;
using AuroraScienceHub.Framework.Ai.Options;
using AuroraScienceHub.Framework.Ai.Tools;

namespace AuroraScienceHub.Framework.UnitTests.Ai.TestDoubles;

/// <summary>
/// HTTP response of the given status, to build a <see cref="System.ClientModel.ClientResultException"/>
/// </summary>
internal sealed class FakePipelineResponse(int status) : PipelineResponse
{
    public override int Status => status;

    public override string ReasonPhrase => string.Empty;

    public override Stream? ContentStream { get; set; }

    public override BinaryData Content => BinaryData.Empty;

    protected override PipelineResponseHeaders HeadersCore => throw new NotSupportedException();

    public override BinaryData BufferContent(CancellationToken cancellationToken = default) => BinaryData.Empty;

    public override ValueTask<BinaryData> BufferContentAsync(CancellationToken cancellationToken = default)
        => ValueTask.FromResult(BinaryData.Empty);

    public override void Dispose()
    {
    }
}
