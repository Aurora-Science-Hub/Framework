# AuroraScienceHub.Framework.Entities

Core domain entity interfaces and patterns for building clean domain models in .NET applications.

## Overview

Provides essential interfaces and abstractions for domain-driven design (DDD), including entity identification, auditing, soft deletion, and specification patterns.

## Key Features

- 🆔 **Strong Typed Identifiers** - Type-safe entity IDs with value object semantics
- 📝 **Auditing Support** - Built-in created/updated timestamp tracking
- 🗑️ **Soft Deletion** - Mark entities as deleted without physical removal
- 🎯 **Specification Pattern** - Composable query specifications
- 🔒 **Type Safety** - Strongly typed interfaces prevent common errors

## Installation

```bash
dotnet add package AuroraScienceHub.Framework.Entities
```

## Core Interfaces

### IEntity<TEntityId>

```csharp
public class User : IEntity<UserId>
{
    public UserId Id { get; private set; }
    public string Name { get; set; }
}
```

### IAuditable

```csharp
public class Order : IEntity<OrderId>, IAuditable
{
    public OrderId Id { get; private set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}
```

### ISoftDeletable

```csharp
public class Product : IEntity<ProductId>, ISoftDeletable
{
    public ProductId Id { get; private set; }
    public bool IsDeleted { get; set; }
    public DateTime? DeletedAt { get; set; }
}
```

## Strong Typed Identifiers

```csharp
// Define specific ID types
public sealed record UserId(Guid Value) : EntityId<Guid>(Value);
public sealed record OrderId(Guid Value) : EntityId<Guid>(Value);

// Use in entities
public class User : IEntity<UserId>
{
    public UserId Id { get; private set; }

    public User()
    {
        Id = new UserId(Guid.NewGuid());
    }
}

// Type safety prevents mistakes
void ProcessOrder(OrderId orderId)
{
    // ProcessOrder(userId); // Compiler error - type safety!
}
```

## Specification Pattern

```csharp
// Define specifications
public class ActiveProductsSpec : Specification<Product>
{
    public override Expression<Func<Product, bool>> ToExpression()
    {
        return product => !product.IsDeleted && product.IsActive;
    }
}

// Use with LINQ
var query = dbContext.Products.Where(activeSpec.ToExpression());
```

## Complete Example

```csharp
public sealed record UserId(Guid Value) : EntityId<Guid>(Value);

public class User : IEntity<UserId>, IAuditable, ISoftDeletable
{
    public UserId Id { get; private set; }
    public string Email { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
    public bool IsDeleted { get; set; }
    public DateTime? DeletedAt { get; set; }

    public User(string email)
    {
        Id = new UserId(Guid.NewGuid());
        Email = email;
        CreatedAt = DateTime.UtcNow;
        UpdatedAt = DateTime.UtcNow;
    }
}
```

## Best Practices

- Always use strong typed IDs to prevent accidental ID mix-ups
- Implement IAuditable to track entity changes automatically
- Use ISoftDeletable to preserve data integrity
- Create reusable specifications for composable query logic

## License

See [LICENSE](../../LICENSE) file in the repository root.
