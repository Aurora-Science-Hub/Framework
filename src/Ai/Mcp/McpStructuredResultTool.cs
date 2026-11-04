using Microsoft.Extensions.AI;
using ModelContextProtocol.Client;
using ModelContextProtocol.Protocol;

namespace AuroraScienceHub.Framework.Ai.Mcp;

/// <summary>
/// MCP tool that gives the model only the result itself: the structured content, or the text when there is none
/// </summary>
/// <remarks>
/// <see cref="McpClientTool"/> hands the model the whole <see cref="CallToolResult"/>. A server with structured output
/// sends the same data twice, as a JSON string in <see cref="CallToolResult.Content"/> (with every quote escaped) and as
/// <see cref="CallToolResult.StructuredContent"/>, so the model would read it about 2.4 times over.
/// </remarks>
internal sealed class McpStructuredResultTool : DelegatingAIFunction
{
    private readonly McpClientTool _tool;

    public McpStructuredResultTool(McpClientTool tool)
        : base(tool)
    {
        _tool = tool;
    }

    protected override async ValueTask<object?> InvokeCoreAsync(AIFunctionArguments arguments, CancellationToken cancellationToken)
    {
        var result = await _tool.CallAsync(arguments, progress: null, options: null, cancellationToken);

        // An error has no structured content: its text tells the model what went wrong (e.g. a too long range)
        if (result.IsError is not true && result.StructuredContent is { } structured)
        {
            return structured;
        }

        var texts = result.Content.OfType<TextContentBlock>().Select(block => block.Text).ToList();
        return texts.Count > 0 ? string.Join('\n', texts) : result;
    }
}
