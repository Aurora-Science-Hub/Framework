# AuroraScienceHub.Framework.Caching

Extension methods for .NET's HybridCache to simplify caching operations in distributed applications.

## Overview

Provides extension methods for Microsoft's `HybridCache` to simplify common caching operations with automatic JSON serialization.

## Key Features

- **Simplified API** - Easy-to-use extension methods for HybridCache
- **Type-Safe Operations** - Generic methods with strong typing
- **JSON Serialization** - Automatic serialization/deserialization
- **Async/Await** - Fully asynchronous operations

## Installation

```bash
dotnet add package AuroraScienceHub.Framework.Caching
```

## Usage

### Service Registration

```csharp
builder.Services.AddHybridCache(options =>
{
    options.MaximumPayloadBytes = 1024 * 1024; // 1 MB
    options.DefaultEntryOptions = new HybridCacheEntryOptions
    {
        Expiration = TimeSpan.FromMinutes(5)
    };
});
```

### Basic Operations

```csharp
// String operations
var value = await cache.GetStringAsync("my-key");
await cache.SetStringAsync("my-key", "my-value", TimeSpan.FromMinutes(5));

// Byte operations
var bytes = await cache.GetBytesAsync("image-key");
await cache.SetBytesAsync("image-key", imageData, TimeSpan.FromHours(1));

// JSON object caching
var user = await cache.GetOrCreateJsonAsync(
    $"user:{userId}",
    async ct => await _userService.GetUserAsync(userId, ct),
    options: new HybridCacheEntryOptions { Expiration = TimeSpan.FromMinutes(10) });
```

### Service Example

```csharp
public class ProductService
{
    private readonly HybridCache _cache;
    private readonly IProductRepository _repository;

    public async Task<Product> GetProductAsync(int id, CancellationToken ct = default)
    {
        return await _cache.GetOrCreateJsonAsync(
            $"product:{id}",
            async token => await _repository.GetByIdAsync(id, token),
            options: new HybridCacheEntryOptions { Expiration = TimeSpan.FromMinutes(15) },
            cancellationToken: ct);
    }
}
```


## License

See [LICENSE](../../LICENSE) file in the repository root.

## Related Packages

- `AuroraScienceHub.Framework.Json` - JSON serialization with optimized settings
