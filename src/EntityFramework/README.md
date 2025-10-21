# AuroraScienceHub.Framework.EntityFramework

Entity Framework Core extensions and utilities for building data access layers with best practices built-in.

## Overview

Provides base classes, interceptors, converters, and utilities for Entity Framework Core applications with automatic auditing, soft delete filters, and strong-typed ID support.

## Key Features

- **Automatic Auditing** - Auto-populate CreatedAt/UpdatedAt timestamps
- **Soft Delete Filters** - Global query filters for soft-deleted entities
- **Strong-Typed ID Converters** - Value converters for EntityId types
- **Migration Helpers** - Simplified migration management
- **Query Extensions** - Enhanced LINQ operations

## Installation

```bash
dotnet add package AuroraScienceHub.Framework.EntityFramework
```

## Usage

### DbContext Setup

```csharp
public class ApplicationDbContext : DbContext, IDataContext
{
    public static string Schema => "app";

    public DbSet<User> Users => Set<User>();

    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.HasDefaultSchema(Schema);
        modelBuilder.ApplyConfigurationsFromAssembly(GetType().Assembly);
    }
}
```

### Automatic Auditing

```csharp
// Configure with interceptor
builder.Services.AddDbContext<ApplicationDbContext>((sp, options) =>
{
    options.UseNpgsql(connectionString)
        .AddInterceptors(new AuditingInterceptor());
});

// Entity with auditing
public class User : IEntity<UserId>, IAuditable
{
    public UserId Id { get; private set; }
    public DateTime CreatedAt { get; set; }  // Auto-populated
    public DateTime UpdatedAt { get; set; }  // Auto-updated
}
```

### Soft Delete

```csharp
public class Product : IEntity<ProductId>, ISoftDeletable
{
    public ProductId Id { get; private set; }
    public bool IsDeleted { get; set; }
    public DateTime? DeletedAt { get; set; }
}

// Configure filter
protected override void OnModelCreating(ModelBuilder modelBuilder)
{
    modelBuilder.Entity<Product>()
        .HasQueryFilter(p => !p.IsDeleted);
}

// Deleted products are automatically excluded from queries
var products = await context.Products.ToListAsync();
```

### Strong-Typed IDs

```csharp
public sealed record UserId(Guid Value) : EntityId<Guid>(Value);

// Configure converter
builder.Property(u => u.Id)
    .HasConversion(
        id => id.Value,
        value => new UserId(value));
```

### Repository Pattern

```csharp
public class UserRepository
{
    private readonly ApplicationDbContext _context;

    public async Task<User?> GetByIdAsync(UserId id, CancellationToken ct = default)
    {
        return await _context.Users.FirstOrDefaultAsync(u => u.Id == id, ct);
    }

    public async Task AddAsync(User entity, CancellationToken ct = default)
    {
        await _context.Users.AddAsync(entity, ct);
        await _context.SaveChangesAsync(ct);
    }
}
```


## License

See [LICENSE](../../LICENSE) file in the repository root.

## Related Packages

- `AuroraScienceHub.Framework.Entities` - Domain entity interfaces
- `AuroraScienceHub.Framework.EntityFramework.NpgSql` - PostgreSQL-specific features
