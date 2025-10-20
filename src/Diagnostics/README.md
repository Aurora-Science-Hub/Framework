# AuroraScienceHub.Framework.Diagnostics

Diagnostic utilities for .NET applications including health checks, connection string parsing, and application diagnostics.

## Overview

Provides essential diagnostic capabilities for monitoring application health, parsing connection strings, tracking application state, and exposing application information.

## Key Features

- 🏥 **Health Check Extensions** - Simplified health check registration
- 🔗 **Connection String Parser** - Safe parsing of database connection strings
- 📊 **Application Descriptor** - Automatic application information tracking (version, instance ID, uptime)
- 🎯 **Custom Health Checks** - Easy creation of custom health checks

## Installation

```bash
dotnet add package AuroraScienceHub.Framework.Diagnostics
```

## Usage

### Application Descriptor

Track application information including version, instance ID, environment, and uptime.

```csharp
// Register (typically in Program.cs)
builder.Services.AddApplicationDescriptor<Program>();

// Configure Host options in appsettings.json
{
  "Host": {
    "Name": "user-service",
    "Namespace": "production"
  }
}

// Use in your services
public class InfoController : ControllerBase
{
    private readonly IApplicationDescriptor _descriptor;

    public InfoController(IApplicationDescriptor descriptor)
    {
        _descriptor = descriptor;
    }

    [HttpGet("/info")]
    public ApplicationInformation GetInfo()
    {
        return _descriptor.Describe();
        // Returns: InstanceId, ApplicationName, Environment,
        //          Namespace, StartedAt, Version, CommitHash
    }
}
```

### Health Check Setup

```csharp
builder.Services.AddHealthChecks()
    .AddCheck("self", () => HealthCheckResult.Healthy(), tags: new[] { "live" })
    .AddNpgSql(connectionString, name: "database", tags: new[] { "ready" })
    .AddRedis(redisConnection, name: "cache", tags: new[] { "ready" });

var app = builder.Build();

app.MapHealthChecks("/health/live", new HealthCheckOptions
{
    Predicate = check => check.Tags.Contains("live")
});

app.MapHealthChecks("/health/ready", new HealthCheckOptions
{
    Predicate = check => check.Tags.Contains("ready")
});
```

### Custom Health Check

```csharp
public class DatabaseHealthCheck : IHealthCheck
{
    private readonly IDbConnection _connection;

    public async Task<HealthCheckResult> CheckHealthAsync(
        HealthCheckContext context,
        CancellationToken ct = default)
    {
        try
        {
            await _connection.OpenAsync(ct);
            return HealthCheckResult.Healthy("Database is accessible");
        }
        catch (Exception ex)
        {
            return HealthCheckResult.Unhealthy("Cannot connect to database", ex);
        }
    }
}

// Register
builder.Services.AddHealthChecks()
    .AddCheck<DatabaseHealthCheck>("database");
```

### Connection String Parser

```csharp
var parsed = ConnectionStringParser.ParsePostgreSql(connectionString);
Console.WriteLine($"Host: {parsed.Host}");
Console.WriteLine($"Database: {parsed.Database}");
// Password is not exposed for security
```

## ApplicationInformation

The `ApplicationInformation` record provides comprehensive application metadata:

```csharp
public record ApplicationInformation(
    string InstanceId,          // Unique instance ID (e.g., "user-service-01234567")
    string ApplicationName,     // Application name from Host config
    string EnvironmentName,     // Environment (Development, Production, etc.)
    string HostNamespace,       // Namespace/cluster from Host config
    DateTimeOffset StartedAt,   // When the application started
    string Version,             // Assembly version
    string? CommitHash          // Git commit hash (if available)
);
```

## Best Practices

- Use `AddApplicationDescriptor<Program>()` to enable application tracking
- Use separate endpoints: `/health/live` for liveness, `/health/ready` for readiness
- Expose `/info` endpoint for application information
- Add meaningful tags to group related checks
- Don't include passwords in diagnostics
- Set appropriate timeouts for health checks
- Monitor health check failures

## Kubernetes Integration

```yaml
livenessProbe:
  httpGet:
    path: /health/live
    port: 80
readinessProbe:
  httpGet:
    path: /health/ready
    port: 80
```

## License

See [LICENSE](../../LICENSE) file in the repository root.

## Related Packages

- `AuroraScienceHub.Framework.AspNetCore` - ASP.NET Core diagnostic extensions
- `AuroraScienceHub.Framework.Utilities` - Host options and configuration
- `AspNetCore.HealthChecks.*` - Additional health check providers
