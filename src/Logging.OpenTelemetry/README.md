# AuroraScienceHub.Framework.Logging.OpenTelemetry

OpenTelemetry logging integration for distributed tracing and observability in .NET applications.

## Overview

Provides simplified OpenTelemetry logging configuration with automatic resource detection and application naming for distributed systems monitoring.

## Key Features

- **OpenTelemetry Integration** - Structured logging with OpenTelemetry
- **Automatic Resource Tagging** - Application name and namespace detection
- **Distributed Tracing** - Correlation across microservices
- **Observability Ready** - Works with Grafana, Jaeger, etc.

## Installation

```bash
dotnet add package AuroraScienceHub.Framework.Logging.OpenTelemetry
```

## Usage

### Basic Setup

```csharp
// Program.cs
var builder = WebApplication.CreateBuilder(args);

builder.Logging.AddOpenTelemetryLogging(builder.Configuration);

var app = builder.Build();
```

### Configuration

```json
{
  "Host": {
    "Name": "user-service",
    "Namespace": "production"
  }
}
```

### Complete OpenTelemetry Setup

```csharp
builder.Services.AddOpenTelemetry()
    .WithTracing(tracing =>
    {
        tracing
            .AddAspNetCoreInstrumentation()
            .AddHttpClientInstrumentation()
            .AddEntityFrameworkCoreInstrumentation();
    })
    .WithMetrics(metrics =>
    {
        metrics
            .AddAspNetCoreInstrumentation()
            .AddRuntimeInstrumentation();
    });

builder.Logging.AddOpenTelemetryLogging(builder.Configuration);

// Configure exporter
builder.Services.ConfigureOpenTelemetryTracerProvider(tracing =>
{
    tracing.AddOtlpExporter(options =>
    {
        options.Endpoint = new Uri("http://localhost:4317");
    });
});
```

### Structured Logging

```csharp
public class OrderService
{
    private readonly ILogger<OrderService> _logger;

    public async Task<Order> CreateOrderAsync(CreateOrderCommand command)
    {
        using var scope = _logger.BeginScope(new Dictionary<string, object>
        {
            ["OrderId"] = command.OrderId,
            ["CustomerId"] = command.CustomerId
        });

        _logger.LogInformation("Creating order for customer {CustomerId}", command.CustomerId);

        // ... create order

        _logger.LogInformation("Order {OrderId} created successfully", order.Id);
        return order;
    }
}
```


## License

See [LICENSE](../../LICENSE) file in the repository root.

## Related Packages

- `AuroraScienceHub.Framework.Diagnostics` - Health checks and diagnostics
- `AuroraScienceHub.Framework.AspNetCore` - ASP.NET Core integration
