# AuroraScienceHub.Framework.ValueObjects

Domain value objects for building type-safe, immutable domain models in .NET applications.

## Overview

Provides strongly-typed value objects that encapsulate domain concepts with built-in validation, parsing, and equality semantics.

## Key Features

- **Type Safety** - Strongly typed value objects prevent primitive obsession
- **Immutability** - Thread-safe, immutable by design
- **Validation** - Built-in validation at creation time
- **Parsing Support** - `ISpanParsable<T>` implementation for efficient parsing
- **EF Core Integration** - Seamless Entity Framework Core support via value converters

## Installation

```bash
dotnet add package AuroraScienceHub.Framework.ValueObjects
```

## Value Objects

### BlobId

Type-safe identifier for MinIO (S3-compatible) blob storage with S3 bucket naming validation, Guid v7-based object IDs (time-ordered, sortable), and compact Base64Url encoding.

Format: `blb_{BucketName}_{ObjectKey}` (e.g., `blb_avatars_auSaAYDuWXG9JXl7SxAlww`)

**Usage:**

```csharp
// Create new BlobId
var blobId = BlobId.New("avatars");

// Parse from string
var parsed = BlobId.Parse("blb_documents_bX9KlP2mN4qR8tVwYzA1Bc");

// Access components
Console.WriteLine(blobId.BucketName); // "avatars"
Console.WriteLine(blobId.ObjectKey);   // "auSaAYDuWXG9JXl7SxAlww"
Console.WriteLine(blobId.Value);      // "blb_avatars_auSaAYDuWXG9JXl7SxAlww"

// Try parse
if (BlobId.TryParse(input, out var result))
{
    // Use result
}
```

**EF Core Integration:**

```csharp
public class Document
{
    public int Id { get; set; }
    public BlobId FileId { get; set; }
    public BlobId? ThumbnailId { get; set; }
}

protected override void OnModelCreating(ModelBuilder modelBuilder)
{
    // Automatic conversion for all BlobId properties (nullable and non-nullable)
    modelBuilder.UseValueObjectConversions();
}
```

## License

See [LICENSE](../../LICENSE) file in the repository root.

## Related Packages

- `AuroraScienceHub.Framework.Entities` - Entity interfaces and patterns
- `AuroraScienceHub.Framework.EntityFramework` - EF Core extensions and value converters

