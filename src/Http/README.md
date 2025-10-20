# AuroraScienceHub.Framework.Http

HTTP client extensions and utilities for building robust API clients in .NET applications.

## Overview

Provides extension methods for `HttpClient` to simplify JSON-based API communication, handle errors gracefully, and build URLs safely.

## Key Features

- 🚀 **Simplified HTTP Calls** - Extension methods for common operations
- 📦 **Automatic JSON Handling** - Built-in serialization/deserialization
- 🛡️ **Error Handling** - Structured exception handling with `ApiClientException`
- 🔗 **URL Builder** - Safe and fluent URL construction
- 📋 **Problem Details Support** - RFC 7807 compliant error responses

## Installation

```bash
dotnet add package AuroraScienceHub.Framework.Http
```

## Usage

### Basic API Client

```csharp
public class UserApiClient
{
    private readonly HttpClient _httpClient;

    public UserApiClient(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<User?> GetUserAsync(int userId)
    {
        return await _httpClient.GetFromJsonOrDefaultAsync<User>(
            new Uri($"/api/users/{userId}", UriKind.Relative));
    }

    public async Task<User?> CreateUserAsync(CreateUserRequest request)
    {
        return await _httpClient.PostAsJsonAsync<CreateUserRequest, User>(
            new Uri("/api/users", UriKind.Relative),
            request);
    }
}
```

### URL Builder

```csharp
var url = UrlBuilder.Create("/api/products/search")
    .AddQuery("q", searchTerm)
    .AddQuery("page", page)
    .AddQueryIfNotNull("category", category)
    .Build();

var products = await _httpClient.GetFromJsonOrDefaultAsync<List<Product>>(url);
```

### Service Registration

```csharp
builder.Services.AddHttpClient<OrderApiClient>(client =>
{
    client.BaseAddress = new Uri("https://api.example.com");
    client.Timeout = TimeSpan.FromSeconds(30);
});
```

### Error Handling

```csharp
try
{
    return await _httpClient.GetFromJsonOrDefaultAsync<Product>(uri);
}
catch (ApiClientException ex)
{
    _logger.LogError(ex, "API call failed. Status: {StatusCode}", ex.StatusCode);
    throw;
}
```

## Extension Methods

```csharp
Task<T?> GetFromJsonOrDefaultAsync<T>(Uri uri, CancellationToken ct = default)
Task<TResponse?> PostAsJsonAsync<TRequest, TResponse>(Uri uri, TRequest body, CancellationToken ct = default)
Task<TResponse?> PutAsJsonAsync<TRequest, TResponse>(Uri uri, TRequest body, CancellationToken ct = default)
```

## Best Practices

- Use typed HTTP clients with `AddHttpClient<T>`
- Set appropriate timeouts
- Implement retry policies with Polly
- Use cancellation tokens
- Log HTTP errors for monitoring

## License

See [LICENSE](../../LICENSE) file in the repository root.

## Related Packages

- `AuroraScienceHub.Framework.Json` - JSON serialization utilities
- `Polly` - Resilience and transient-fault-handling (recommended)
