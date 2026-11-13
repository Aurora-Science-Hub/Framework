# Changelog

All notable changes to this project are documented in this file.

The format is based on [Keep a Changelog](https://keepachangelog.com/en/1.1.0/).
Package versions follow the rules in [AGENTS.md](AGENTS.md#package-versioning).

## [11.2.0]

### Added

- `AuroraScienceHub.Framework.AspNetCore`: `AuthenticatedOutputCachePolicy` and the `AllowAuthenticated()` output
  cache builder extension allow caching authenticated machine-to-machine endpoints. The policy re-enables lookup and
  storage for an authenticated GET/HEAD after the default ASP.NET Core policy, and repeats the opt-in in the response
  phase for `11.0.0-rc.2`.

### Changed

- Dependencies: `System.Text.Json` is pinned to the stable `11.0.0`; the transitive `11.0.0-rc.2` package is flagged
  by NU1902.

### Fixed

- `AuroraScienceHub.Framework.Ai`: `McpToolProvider.GetToolsAsync` observes the request cancellation even when the
  shared connection attempt has already completed. A cancelled request no longer returns tools as if the server
  were reachable.

## [11.1.1]

### Fixed

- `AuroraScienceHub.Framework.AspNetCore`: `EntityNotFoundException` now returns `404 Not Found` and
  `AccessDeniedException` returns `403 Forbidden`. Previously the validation problem enricher overwrote their status
  with `400 Bad Request`. `ValidationException` and its other subclasses still return `400` with the validation errors.

## [11.1.0]

### Added

- `AuroraScienceHub.Framework.Ai`: `AddAi` configures an Agent Framework assistant from the `Ai` section. The package
  includes conversation compaction, MCP and application tools, request time budgets, retries, OpenTelemetry telemetry,
  and DeepL translation.
- `AuroraScienceHub.Framework.Ai`: a console sample demonstrates a request to an OpenAI-compatible provider.

### Changed

- `AuroraScienceHub.Framework.Ai`: the package line moves to 11.1.0. The major version still follows the highest
  supported .NET version, as defined in `Directory.Build.props`.

### Removed

- **Breaking:** `AuroraScienceHub.Framework.Ai` removes `IGptClient`, `GptClient`, `GptOptions`, and `AddGptClient`,
  together with the `ChatGptNet` dependency. Replace `AddGptClient` with `AddAi` and `IGptClient` with `IAiAssistant`.
  The `Ai` section replaces the previous GPT configuration. A consumer that cannot migrate stays on 11.0.x.
