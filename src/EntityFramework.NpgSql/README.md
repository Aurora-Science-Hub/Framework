# AuroraScienceHub.Framework.EntityFramework.NpgSql

PostgreSQL-specific extensions for Entity Framework Core including DbContext factories and optimized configurations.

## Overview

Provides PostgreSQL-specific implementations for Entity Framework Core, including design-time DbContext factories for migrations and PostGIS spatial support.

## Key Features

- 🏭 **DbContext Factories** - Design-time factories for EF migrations
- 🌍 **PostGIS Support** - Spatial data types and operations
- 🎯 **PostgreSQL Conventions** - Database naming and configuration
- ⚡ **Performance Optimizations** - PostgreSQL-specific optimizations

## Installation

```bash
dotnet add package AuroraScienceHub.Framework.EntityFramework.NpgSql
```

## Usage

### DbContext Factory

```csharp
// 1. Define connection options
public class DatabaseOptions : IConnectionOptions
{
    public static string OptionKey => "Database";
    public string Host { get; set; } = "localhost";
    public int Port { get; set; } = 5432;
    public string Database { get; set; } = string.Empty;
    public string Username { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;

    public string GetConnectionString()
    {
        return $"Host={Host};Port={Port};Database={Database};Username={Username};Password={Password}";
    }
}

// 2. Create factory for migrations
public class ApplicationDbContextFactory
    : PostgreSqlDbContextFactoryBase<ApplicationDbContext, DatabaseOptions>
{
    protected override ApplicationDbContext CreateDbContext(
        DbContextOptions<ApplicationDbContext> options)
    {
        return new ApplicationDbContext(options);
    }
}
```

### Service Registration

```csharp
builder.Services.AddDbContext<ApplicationDbContext>((sp, options) =>
{
    var configuration = sp.GetRequiredService<IConfiguration>();
    var dbOptions = configuration.GetRequiredOptions<DatabaseOptions>();

    options.UseNpgsql(
        dbOptions.GetConnectionString(),
        npgsqlOptions =>
        {
            npgsqlOptions.MigrationsAssembly(typeof(ApplicationDbContext).Assembly.FullName);
            npgsqlOptions.EnableRetryOnFailure(maxRetryCount: 3);
        });
});
```

### PostgreSQL-Specific Features

```csharp
public class ProductConfiguration : IEntityTypeConfiguration<Product>
{
    public void Configure(EntityTypeBuilder<Product> builder)
    {
        // PostgreSQL array types
        builder.Property(p => p.Tags)
            .HasColumnType("text[]");

        // JSONB
        builder.Property(p => p.Metadata)
            .HasColumnType("jsonb");

        // Full-text search index
        builder.HasIndex(p => p.Description)
            .HasMethod("GIN")
            .IsTsVectorExpressionIndex("english");
    }
}
```

### Spatial Data (PostGIS)

```csharp
public class Location : IEntity<LocationId>
{
    public LocationId Id { get; private set; }
    public Point Coordinates { get; set; } = null!;
}

// Configure DbContext
options.UseNpgsql(connectionString, x => x.UseNetTopologySuite());

// Spatial queries
var nearbyLocations = await _context.Locations
    .Where(l => l.Coordinates.Distance(point) <= radiusMeters)
    .ToListAsync();
```

### Running Migrations

```bash
# Add migration
dotnet ef migrations add InitialCreate --project src/MyApp.Infrastructure

# Update database
dotnet ef database update --project src/MyApp.Infrastructure

# Generate SQL script
dotnet ef migrations script --output migration.sql
```

## Best Practices

- Use DbContext factories for design-time operations
- Enable connection pooling for better performance
- Use appropriate indexes (GIN for arrays/JSONB, GIST for spatial)
- Leverage PostgreSQL features (arrays, JSONB, full-text search)
- Use snake_case naming for PostgreSQL convention
- Enable retry on failure for transient errors

## Configuration Example

```json
{
  "Database": {
    "Host": "localhost",
    "Port": 5432,
    "Database": "myapp",
    "Username": "postgres",
    "Password": "secret",
    "MinPoolSize": 10,
    "MaxPoolSize": 100
  }
}
```

## License

See [LICENSE](../../LICENSE) file in the repository root.

## Related Packages

- `AuroraScienceHub.Framework.EntityFramework` - Base EF Core functionality
- `AuroraScienceHub.Framework.Configuration` - Configuration helpers
