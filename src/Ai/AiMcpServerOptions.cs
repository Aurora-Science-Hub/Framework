namespace AuroraScienceHub.Framework.Ai;

/// <summary>
/// An MCP server whose tools the assistant may call
/// </summary>
public sealed class AiMcpServerOptions
{
    /// <summary>
    /// Address of the server's Streamable HTTP endpoint, e.g. https://core.example.org/mcp; the server is not used
    /// while it is not set
    /// </summary>
    public Uri? Endpoint { get; set; }
}
