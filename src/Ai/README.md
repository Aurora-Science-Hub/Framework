# AuroraScienceHub.Framework.Ai

AI service integrations including OpenAI GPT and DeepL translation with proxy support.

## Overview

Provides unified interfaces for common AI services including GPT-based chat completions and DeepL translations.

## Key Features

- **GPT Integration** - OpenAI GPT chat completions
- **DeepL Translation** - High-quality machine translation
- **Proxy Support** - HTTP proxy configuration for AI services
- **Unified Interfaces** - Consistent API across providers

## Installation

```bash
dotnet add package AuroraScienceHub.Framework.Ai
```

## Usage

### GPT Client

```csharp
// Configuration
builder.Services.AddChatGpt(options =>
{
    options.ApiKey = builder.Configuration["Gpt:ApiKey"]!;
    options.DefaultModel = "gpt-4";
});

builder.Services.AddScoped<IGptClient, GptClient>();

// Usage
public class AiService
{
    private readonly IGptClient _gptClient;

    public async Task<string?> GenerateTextAsync(string prompt)
    {
        return await _gptClient.AskAsync(prompt);
    }
}
```

### DeepL Translation

```csharp
// Configuration
builder.Services.Configure<DeeplOptions>(
    builder.Configuration.GetSection("Deepl"));
builder.Services.AddScoped<IDeeplClient, DeeplClient>();

// Usage
public class TranslationService
{
    private readonly IDeeplClient _deeplClient;

    public async Task<string> TranslateAsync(
        string text,
        string targetLanguage)
    {
        return await _deeplClient.TranslateAsync(text, targetLanguage);
    }
}
```

### Configuration

```json
{
  "Gpt": {
    "ApiKey": "your-openai-api-key",
    "Model": "gpt-4"
  },
  "Deepl": {
    "ApiKey": "your-deepl-api-key"
  }
}
```


## License

See [LICENSE](../../LICENSE) file in the repository root.

## Related Packages

- `AuroraScienceHub.Framework.Http` - HTTP utilities
- `AuroraScienceHub.Framework.Caching` - Cache AI responses
