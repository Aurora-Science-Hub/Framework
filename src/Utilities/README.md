# AuroraScienceHub.Framework.Utilities

Core utilities and helper methods for common programming tasks in .NET applications.

## Overview

Collection of utility classes and extension methods for date/time handling, regional utilities, configuration helpers, and system-level abstractions.

## Key Features

- **Date/Time Utilities** - Date ranges, time zone handling
- **Regional Support** - Culture and localization helpers
- **System Extensions** - Null checking, string operations
- **Configuration Helpers** - Option description interfaces

## Installation

```bash
dotnet add package AuroraScienceHub.Framework.Utilities
```

## Usage

### DateTimeRange

```csharp
var range = new DateTimeRange(
    DateTime.Parse("2024-01-01"),
    DateTime.Parse("2024-12-31"));

if (range.Contains(DateTime.Now)) { }
var duration = range.Duration;
```

### Required() - Null Checking

```csharp
// Throws ArgumentNullException if null
var user = userRepository.GetById(id).Required();
var config = configuration.GetSection("Database").Required("Database config missing");
```

### String Extensions

```csharp
if (input.IsNullOrWhiteSpace()) { }
var result = longString.SafeSubstring(0, 100);
var preview = longText.Truncate(50); // Adds "..."
```

### Configuration with IOptionDescription

```csharp
public class DatabaseOptions : IOptionDescription
{
    public static string OptionKey => "Database";
    public string ConnectionString { get; set; } = string.Empty;
}
```

### Host Options

```csharp
public class HostOptions : IOptionDescription
{
    public static string OptionKey => "Host";
    public string Name { get; set; } = string.Empty;
    public string Namespace { get; set; } = string.Empty;
}

// appsettings.json
{
  "Host": {
    "Name": "user-service",
    "Namespace": "production"
  }
}
```


## License

See [LICENSE](../../LICENSE) file in the repository root.

## Related Packages

- `AuroraScienceHub.Framework.Configuration` - Enhanced configuration utilities
- `AuroraScienceHub.Framework.Exceptions` - Framework exception types
