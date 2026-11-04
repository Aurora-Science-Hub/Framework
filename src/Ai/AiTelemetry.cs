namespace AuroraScienceHub.Framework.Ai;

/// <summary>
/// Names of the telemetry the AI features emit
/// </summary>
public static class AiTelemetry
{
    /// <summary>
    /// Activity source and meter of the calls to the language model (OpenTelemetry <c>gen_ai.*</c> spans and metrics)
    /// </summary>
    public const string SourceName = "AuroraScienceHub.Framework.Ai";

    /// <summary>
    /// Activity source and meter of the MCP SDK (the calls to the tools of MCP servers)
    /// </summary>
    public const string McpSourceName = "Experimental.ModelContextProtocol";
}
