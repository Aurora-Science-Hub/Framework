# Changelog

All notable changes to this project are documented in this file.

The format is based on [Keep a Changelog](https://keepachangelog.com/en/1.1.0/).
Package versions follow the rules in [AGENTS.md](AGENTS.md#package-versioning).

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
