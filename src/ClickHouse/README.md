# AuroraScienceHub.Framework.ClickHouse

ClickHouse database integration with connection management and migration support for OLAP workloads.

## Overview

Provides integration with ClickHouse, a high-performance columnar database for analytical workloads, including connection management and migration runners.

## Key Features

- 🚀 **ClickHouse Integration** - Connection and query support
- 🔄 **Migration Support** - Database schema migration management
- ⚙️ **Configuration** - Type-safe ClickHouse configuration
- 📊 **OLAP Optimized** - Designed for analytical workloads

## Installation

```bash
dotnet add package AuroraScienceHub.Framework.ClickHouse
```

## Usage

### Setup

```csharp
// Configuration
builder.Services.AddClickHouse();

// appsettings.json
{
  "ClickHouse": {
    "ClickHouse": "Host=localhost;Port=9000;Database=analytics;Username=default"
  }
}
```

### Query Example

```csharp
public class AnalyticsService
{
    private readonly IClickHouseConnection _connection;

    public async Task<List<EventData>> GetEventsAsync(DateTime from, DateTime to)
    {
        var query = @"
            SELECT event_date, event_type, user_id, COUNT(*) as count
            FROM events
            WHERE event_date BETWEEN @from AND @to
            GROUP BY event_date, event_type, user_id
            ORDER BY event_date DESC
        ";

        using var command = _connection.CreateCommand();
        command.CommandText = query;
        command.Parameters.AddWithValue("from", from);
        command.Parameters.AddWithValue("to", to);

        var events = new List<EventData>();
        using var reader = await command.ExecuteReaderAsync();
        while (await reader.ReadAsync())
        {
            events.Add(new EventData
            {
                EventDate = reader.GetDateTime(0),
                EventType = reader.GetString(1),
                UserId = reader.GetInt32(2),
                Count = reader.GetInt64(3)
            });
        }
        return events;
    }
}
```

### Migrations

```csharp
// Create migration
public class CreateEventsTableMigration
{
    public static string Up => @"
        CREATE TABLE IF NOT EXISTS events
        (
            event_date Date,
            event_type String,
            user_id UInt32
        )
        ENGINE = MergeTree()
        PARTITION BY toYYYYMM(event_date)
        ORDER BY (event_date, event_type)
    ";
}

// Run migrations
using var scope = app.Services.CreateScope();
var migrationRunner = scope.ServiceProvider.GetRequiredService<IClickHouseMigrationRunner>();
await migrationRunner.RunMigrationsAsync();
```

## Best Practices

- Use MergeTree engine for most use cases
- Partition data by date for time-series workloads
- Batch inserts for better performance
- Use materialized views for pre-aggregation
- Set appropriate ORDER BY based on query patterns

## License

See [LICENSE](../../LICENSE) file in the repository root.

## Related Packages

- `AuroraScienceHub.Framework.Diagnostics` - Health checks for ClickHouse
