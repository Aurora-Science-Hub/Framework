# AuroraScienceHub.Framework.Json

Standardized JSON serialization utilities with optimized settings for .NET applications.

## Overview

Provides consistent JSON serialization configuration with sensible defaults for web APIs, including camelCase naming, enum string conversion, and proper handling of complex types.

## Key Features

- **Consistent Serialization** - Uniform JSON settings across the application
- **camelCase by Default** - Web-friendly property naming
- **Enum String Conversion** - Human-readable enum values
- **Performance Optimized** - Configured for best performance

## Installation

```bash
dotnet add package AuroraScienceHub.Framework.Json
```

## Usage

### Basic Serialization

```csharp
// Serialize
var json = DefaultJsonSerializer.Serialize(myObject);

// Deserialize
var myObject = DefaultJsonSerializer.Deserialize<MyType>(json);
```

### Custom Configuration

```csharp
var options = DefaultJsonSerializerOptions.Create();
options.WriteIndented = true; // Pretty print
var json = JsonSerializer.Serialize(myObject, options);
```

### ASP.NET Core Integration

```csharp
builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        var defaultOptions = DefaultJsonSerializerOptions.Create();
        options.JsonSerializerOptions.PropertyNamingPolicy = defaultOptions.PropertyNamingPolicy;
        options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
    });
```

## Default Configuration

```csharp
PropertyNamingPolicy = JsonNamingPolicy.CamelCase
PropertyNameCaseInsensitive = true
DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull
Converters = { new JsonStringEnumConverter() }
```


## License

See [LICENSE](../../LICENSE) file in the repository root.

## Related Packages

- `AuroraScienceHub.Framework.Http` - HTTP client extensions
- `AuroraScienceHub.Framework.Caching` - Caching with JSON serialization
