# AuroraScienceHub.Framework.Ai

AI integrations built on Microsoft Agent Framework and `Microsoft.Extensions.AI`: a stateless assistant with optional
MCP tools, conversation compaction, and DeepL translation.

## Overview

`AddAi` registers a chat client for an OpenAI-compatible provider, a stateless `IAiAssistant` on top of it, and an
`IConversationCompactor` that folds older messages of a conversation into a summary. The assistant calls the tools of
the configured MCP servers and of any tool provider (`IAiToolProvider`) a service registers itself.

The package does not try to cover or replace Microsoft Agent Framework. It configures its `AIAgent` and `IChatClient`
from application settings and adds validation, retries, time budgets, MCP tool discovery, conversation compaction,
telemetry, and DeepL translation. The API exposes Agent Framework types such as `AIAgent` and `ChatOptions` directly.

## Key Features

- **Assistant** — `IAiAssistant.AskAsync` sends the instructions, the summary, the recent history and the question, and
  calls the tools the model asks for
- **Failures as outcomes** — a provider failure is reported in `AiResult.Outcome` and never thrown. Application errors
  and cancellation by the caller propagate
- **Time budget** — `Ai:RequestTimeout` limits the whole request, retries and tool rounds included
- **MCP tools** — Streamable HTTP servers from the `Ai:Mcp` section; an unreachable server is left out and retried
- **Per-call options** — a caller overrides the model, the answer budget, the reasoning effort, the temperature and
  top-p for one request with `ChatOptions`
- **DeepL translation** — `AddDeepl` registers the DeepL client with the shared proxy settings
- **Telemetry** — `gen_ai.*` spans and metrics through OpenTelemetry

## Installation

```bash
dotnet add package AuroraScienceHub.Framework.Ai
```

The types are grouped by concern: `AuroraScienceHub.Framework.Ai.Assistant` (`IAiAssistant`, `AiRequest`, `AiResult`,
`AiChatMessage`), `...Ai.Conversation` (`IConversationCompactor`), `...Ai.Options` (`AiOptions`), `...Ai.Tools`
(`IAiToolProvider`), `...Ai.Mcp` (the built-in MCP tool provider) and `...Ai.Deepl`. The root namespace holds
`AddAi` and `AiTelemetry`.

## Versioning

11.1.0 replaces the `IGptClient` of the 11.0.x line with the Agent Framework based assistant. Removing the GPT client
is a breaking change, and it ships in a minor release on purpose: the core and the bot are the only consumers of the
package, and both move to the new client in the same release train, so there is no version of the package that has to
serve both. The removal is called out in the [changelog](../../CHANGELOG.md). A consumer that needs the GPT client stays
on 11.0.x.

## Usage

### Registration

```csharp
builder.Services.AddAi(builder.Configuration);
```

`AddAi` binds the `Ai` section, validates it on start, and registers the assistant, the compactor and the MCP tool
provider. The services are registered even while `Ai:Enabled` is `false`, and the graph is resolvable without a key and
a model: the assistant then works with a client that fails only if it is called. Callers check `AiOptions.Enabled`
before they ask.

The options are read once, when the services are created: every change of the AI settings applies on a restart.

The optional `configure` delegate changes the options after the configuration is bound:

```csharp
builder.Services.AddAi(builder.Configuration, options => options.MaxRetries = 3);
```

### Configuration

```json
{
  "Ai": {
    "Enabled": true,
    "Provider": "OpenAICompatible",
    "Endpoint": "https://api.deepseek.com",
    "ApiKey": "",
    "Model": "deepseek-chat",
    "RequestTimeout": "00:01:00",
    "MaxRetries": 1,
    "MaxOutputTokens": null,
    "SendLegacyMaxTokens": false,
    "ReasoningEffort": null,
    "Temperature": null,
    "TopP": null,
    "MaxToolCallRounds": 5,
    "McpConnectionTimeout": "00:00:05",
    "McpRetryInterval": "00:01:00",
    "UseProxy": false,
    "Mcp": {
      "core": { "Endpoint": "https://core.example.org/mcp" }
    }
  }
}
```

- `Endpoint` must be an absolute HTTPS address; plain HTTP is accepted only for a loopback address, so the API key
  never leaves the host unencrypted
- `MaxOutputTokens` is an upper bound of the answer, not a target size; `null` means the limit is not sent and the
  provider default applies
- `SendLegacyMaxTokens` also sends the budget as `max_tokens`, for providers that ignore `max_completion_tokens`
  (DeepSeek); OpenAI rejects `max_tokens` for its reasoning models, so keep it `false` there
- `ReasoningEffort` is `null` unless the provider should use it; `null` leaves `ChatOptions.Reasoning` unset
- `Temperature` and `TopP` are `null` unless the provider should use them
- `UseProxy` sends the provider and the MCP requests through the proxy of the `Proxy` section; the proxied requests use
  clients of their own, so the primary handler the host configures for every client stays in place for the direct ones
- An MCP server without `Endpoint` is not used; plain HTTP is accepted for it, as no secret is sent there

### Asking a question

```csharp
var request = new AiRequest(
    Instructions: "You answer questions about space weather.",
    Summary: summary,
    History: messages,
    Question: "What is the Kp index now?");

var result = await assistant.AskAsync(request, cancellationToken);
if (result.IsSuccess)
{
    return result.Text;
}

logger.LogWarning("The assistant did not answer: {Outcome}", result.Outcome);
```

`AiRequest.Options` overrides the configured defaults for this call only:

```csharp
var request = new AiRequest(instructions, summary, messages, question)
{
    Options = new ChatOptions { MaxOutputTokens = 400, Temperature = 0.2f },
};
```

Tools given in `AiRequest.Options.Tools` are used together with the tools of the `IAiToolProvider` services, the tools
of the request first. A name used more than once — by the request and a provider, or by two providers — fails the
request with `AiToolNameConflictException`, so one tool never silently shadows another.

The assistant is deliberately not streaming: the answer, its usage and its outcome arrive together, and the caller
needs the whole text to store it.

### Console sample

The [console sample](../../samples/Ai/) registers the package, sends one question, and prints the answer and token
usage. Its `appsettings.json` contains the DeepSeek connection settings without an API key.

1. Set the API key:

   ```bash
   export Ai__ApiKey="your-api-key"
   ```

2. Run the sample from the repository root:

   ```bash
   dotnet run --project samples/Ai/Ai.Sample.csproj
   ```

3. Pass a custom question after `--` if needed:

   ```bash
   dotnet run --project samples/Ai/Ai.Sample.csproj -- "What is the Kp index?"
   ```

The host loads environment variables by default, so other settings can be overridden with variables such as
`Ai__Model` and `Ai__Endpoint`.

### Summarizing a conversation

```csharp
var summary = await compactor.CompactAsync(
    previousSummary,
    olderMessages,
    new ChatOptions { MaxOutputTokens = 800 },
    cancellationToken);
```

### MCP tools

Tools of the configured servers are loaded on the first request, and the list is kept until the application restarts.
A server that cannot be reached is left out and tried again after `McpRetryInterval`, so the assistant answers without
its tools meanwhile. A tool of an MCP server returns its structured result as JSON when the server provides one, so the
model sees the fields and not only a formatted text.

### Own tools

Register any number of `IAiToolProvider` implementations; all of them are used:

```csharp
public sealed class AuthenticatedMcpToolProvider : IAiToolProvider
{
    public Task<IReadOnlyList<AITool>> GetToolsAsync(CancellationToken cancellationToken)
        => /* connect to the server and return its tools */;
}

builder.Services.AddSingleton<IAiToolProvider, AuthenticatedMcpToolProvider>();
```

Two providers that return a tool with the same name — or a tool of the request that shares a name with a provider
tool — fail the request with `AiToolNameConflictException`, which lists the conflicting names: such a configuration
would otherwise silently shadow one of the tools.

### DeepL translation

```csharp
builder.Services.AddDeepl();

var text = await deeplClient.TranslateAsync("Aurora", DeeplLanguage.English, DeeplLanguage.Russian, cancellationToken);
```

```json
{
  "DeepL": {
    "ApiKey": "",
    "UseProxy": false
  },
  "Proxy": {
    "Address": "http://proxy.example.org:3128",
    "UserName": "",
    "Password": ""
  }
}
```

### Telemetry

`AiTelemetry.SourceName` is the activity source and the meter of the model calls (`gen_ai.*` spans and metrics);
`AiTelemetry.McpSourceName` is the one of the MCP SDK. Subscribe both in the OpenTelemetry setup of the host.

## License

See [LICENSE](../../LICENSE) file in the repository root.

## Related Packages

- `AuroraScienceHub.Framework.Http` — HTTP utilities, including the proxy handler
- `AuroraScienceHub.Framework.Json` — JSON serialization
