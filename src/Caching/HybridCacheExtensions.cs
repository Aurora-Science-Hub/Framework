using System.Text.Json;
using AuroraScienceHub.Framework.Json;
using Microsoft.Extensions.Caching.Hybrid;
using Microsoft.Extensions.Logging;

namespace AuroraScienceHub.Framework.Caching;

/// <summary>
/// Extension methods for <see cref="HybridCache"/>
/// </summary>
public static class HybridCacheExtensions
{
    private static readonly HybridCacheEntryOptions s_fastExpireCacheOptions = new()
    {
        Expiration = TimeSpan.FromTicks(1),
    };

    /// <summary>
    /// Get the value associated with the specified key.
    /// </summary>
    /// <remarks>
    /// If the key does not exist, the default value is returned.
    /// </remarks>
    public static async Task<string?> GetStringAsync(
        this HybridCache cache,
        string key,
        CancellationToken cancellationToken = default)
    {
        return await cache.GetOrCreateAsync<string?>(
                key,
                options: s_fastExpireCacheOptions,
                factory: _ => ValueTask.FromResult<string?>(null),
                // There is no Get method in HybridCache for now. See more details:
                // https://github.com/dotnet/aspnetcore/issues/54647#issuecomment-2515652588
                // https://github.com/dotnet/aspnetcore/discussions/57191
                cancellationToken: cancellationToken)
            .ConfigureAwait(false);
    }

    /// <summary>
    /// Get the bytes array associated with the specified key.
    /// </summary>
    /// <remarks>
    /// If the key does not exist, the default value is returned.
    /// </remarks>
    public static async Task<byte[]?> GetBytesAsync(
        this HybridCache cache,
        string key,
        CancellationToken cancellationToken = default)
    {
        return await cache.GetOrCreateAsync<byte[]?>(
                key,
                options: s_fastExpireCacheOptions,
                factory: _ => ValueTask.FromResult<byte[]?>(null),
                // There is no Get method in HybridCache for now. https://github.com/dotnet/aspnetcore/issues/54647#issuecomment-2515652588
                cancellationToken: cancellationToken)
            .ConfigureAwait(false);
    }

    /// <summary>
    /// Get the bytes array associated with the specified key.
    /// </summary>
    /// <remarks>
    /// If the key does not exist, the default value is returned.
    /// </remarks>
    public static async Task<byte[]?> SafeGetBytesAsync(
        this HybridCache cache,
        string key,
        ILogger logger,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var cachedData = await cache.GetBytesAsync(key, cancellationToken).ConfigureAwait(false);
            if (cachedData is null)
            {
                logger.LogDebug("Cache miss for {CacheKey}", key);
                return null;
            }

            logger.LogDebug("Cache hit for {CacheKey}", key);

            return cachedData;
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Failed to get cache for {CacheKey}", key);
            return null;
        }
    }

    /// <summary>
    /// Get the value associated with the specified key.
    /// </summary>
    /// <remarks>
    /// If the key does not exist, the default value is returned.
    /// </remarks>
    public static async Task<T?> SafeGetAsync<T>(
        this HybridCache cache,
        string key,
        ILogger logger,
        JsonSerializerOptions? jsonSerializerOptions = null,
        CancellationToken cancellationToken = default)
        where T : class?
    {
        try
        {
            var cachedData = await cache.GetStringAsync(key, cancellationToken).ConfigureAwait(false);
            if (cachedData is null)
            {
                logger.LogDebug("Cache miss for {CacheKey}", key);
                return null;
            }

            logger.LogDebug("Cache hit for {CacheKey}", key);

            var result = jsonSerializerOptions is null
                 ? DefaultJsonSerializer.Deserialize<T?>(cachedData)
                 : JsonSerializer.Deserialize<T?>(cachedData, jsonSerializerOptions);

            return result;
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Failed to get cache for {CacheKey}", key);
            return null;
        }
    }

    /// <summary>
    /// Set the bytes array associated with the specified key.
    /// </summary>
    public static async Task SafeSetBytesAsync(
        this HybridCache cache,
        string key,
        byte[] data,
        ILogger logger,
        HybridCacheEntryOptions? cacheEntryOptions = null,
        IEnumerable<string>? tags = null,
        CancellationToken cancellationToken = default)
    {
        try
        {
            await cache.SetAsync(
                    key,
                    data,
                    options: cacheEntryOptions,
                    tags: tags,
                    cancellationToken: cancellationToken)
                .ConfigureAwait(false);

            logger.LogDebug("Cache set for {CacheKey}", key);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Failed to set cache for {CacheKey}", key);
        }
    }

    /// <summary>
    /// Set the serializable value associated with the specified key.
    /// </summary>
    public static async Task SafeSetAsync<TData>(
        this HybridCache cache,
        string key,
        TData data,
        ILogger logger,
        HybridCacheEntryOptions? cacheEntryOptions = null,
        IEnumerable<string>? tags = null,
        JsonSerializerOptions? jsonSerializerOptions = null,
        CancellationToken cancellationToken = default)
        where TData : class
    {
        try
        {
            var serializedData = jsonSerializerOptions is null
                ? DefaultJsonSerializer.Serialize(data)
                : JsonSerializer.Serialize(data, jsonSerializerOptions);

            await cache.SetAsync(
                    key,
                    serializedData,
                    options: cacheEntryOptions,
                    tags: tags,
                    cancellationToken: cancellationToken)
                .ConfigureAwait(false);

            logger.LogDebug("Cache set for {CacheKey}", key);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Failed to set cache for {CacheKey}", key);
        }
    }
}
