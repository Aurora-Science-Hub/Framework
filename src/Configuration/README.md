# AuroraScienceHub.Framework.Configuration

Enhanced configuration utilities for .NET applications with type-safe configuration loading and validation.

## Overview

Provides extension methods to simplify .NET's configuration system, including required configuration validation and strongly-typed options.

## Key Features

- ✅ **Required Configuration** - Enforce presence of configuration sections
- 🔒 **Type-Safe Options** - Strongly typed configuration with validation
- 🏗️ **Minimal Loading** - Lightweight configuration for design-time tools
- 🎯 **Option Descriptions** - Self-documenting configuration classes

## Installation

```bash
dotnet add package AuroraScienceHub.Framework.Configuration
```

## Usage

### Define Options

```csharp
public class DatabaseOptions : IOptionDescription
{
    public static string OptionKey => "Database";

    public string ConnectionString { get; set; } = string.Empty;
    public int MaxRetries { get; set; }
    public TimeSpan Timeout { get; set; }
}

// appsettings.json
{
  "Database": {
    "ConnectionString": "Server=localhost;Database=mydb",
    "MaxRetries": 3,
    "Timeout": "00:00:30"
  }
}
```

### Get Required Options

```csharp
public class MyService
{
    private readonly DatabaseOptions _options;

    public MyService(IConfiguration configuration)
    {
        // Throws if section missing or invalid
        _options = configuration.GetRequiredOptions<DatabaseOptions>();
    }
}
```

### Design-Time Loading

```csharp
public class DesignTimeDbContextFactory : IDesignTimeDbContextFactory<MyDbContext>
{
    public MyDbContext CreateDbContext(string[] args)
    {
        var configuration = MinimalConfigurationLoader.Load(args);
        var options = configuration.GetRequiredOptions<DatabaseOptions>();
        return new MyDbContext(options);
    }
}
```

### Service Registration Pattern

```csharp
public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddEmailService(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        var options = configuration.GetRequiredOptions<EmailOptions>();
        services.Configure<EmailOptions>(
            configuration.GetSection(EmailOptions.OptionKey));
        services.AddScoped<IEmailSender, EmailSender>();
        return services;
    }
}
```

## Best Practices

- Implement IOptionDescription for self-documenting configuration
- Use GetRequiredOptions early to fail fast if configuration is missing
- Use strongly typed options to avoid magic strings
- Don't store secrets in appsettings.json (use user secrets or Key Vault)

## License

See [LICENSE](../../LICENSE) file in the repository root.

## Related Packages

- `AuroraScienceHub.Framework.Utilities` - Core utilities and helpers
- `AuroraScienceHub.Framework.Composition` - Module-based configuration
