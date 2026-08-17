using System.Diagnostics.CodeAnalysis;
using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using System.Text.Json.Serialization.Metadata;
using AuroraScienceHub.Framework.Json;

namespace AuroraScienceHub.Framework.Http;

/// <summary>
/// Extension methods for <see cref="HttpClient"/> to simplify working with JSON serialization
/// </summary>
/// <remarks>
/// <see cref="DefaultJsonSerializer"/> is used for serialization and deserialization.
/// </remarks>
public static class HttpClientExtensions
{
    private const string RequiresUnreferencedCodeMessage =
        "JSON serialization and deserialization might require types that cannot be statically analyzed. " +
        "Use the overload that accepts a JsonTypeInfo, or ensure all required types are preserved.";

    private const string RequiresDynamicCodeMessage =
        "JSON serialization and deserialization might require types that cannot be statically analyzed and might need runtime code generation. " +
        "Use System.Text.Json source generation for native AOT applications.";

    private static readonly JsonSerializerOptions s_options = DefaultJsonSerializerOptions.Create();

    // ---------------------------------------------------------------------------------
    // AOT-safe overloads backed by source-generated JsonTypeInfo<T> metadata.
    // ---------------------------------------------------------------------------------

    public static async Task<TResponse?> GetFromJsonOrDefaultAsync<TResponse>(
        this HttpClient client,
        Uri requestUri,
        JsonTypeInfo<TResponse> jsonTypeInfo,
        CancellationToken cancellationToken = default)
    {
        using var response = await client.GetAsync(requestUri, cancellationToken).ConfigureAwait(false);

        return await response.ReadFromJsonOrDefaultAsync(jsonTypeInfo, cancellationToken).ConfigureAwait(false);
    }

    public static async Task<TResponse?> PostAsJsonAsync<TRequest, TResponse>(
        this HttpClient client,
        Uri requestUri,
        TRequest body,
        JsonTypeInfo<TRequest> requestJsonTypeInfo,
        JsonTypeInfo<TResponse> responseJsonTypeInfo,
        CancellationToken cancellationToken = default)
    {
        using var response = await client
            .PostAsJsonAsync(requestUri, body, requestJsonTypeInfo, cancellationToken)
            .ConfigureAwait(false);

        return await response.ReadFromJsonOrDefaultAsync(responseJsonTypeInfo, cancellationToken).ConfigureAwait(false);
    }

    public static async Task<T?> PostAsync<T>(
        this HttpClient client,
        Uri requestUri,
        JsonTypeInfo<T> jsonTypeInfo,
        CancellationToken cancellationToken = default)
    {
        using var response = await client.PostAsync(requestUri, null, cancellationToken).ConfigureAwait(false);

        return await response.ReadFromJsonOrDefaultAsync(jsonTypeInfo, cancellationToken).ConfigureAwait(false);
    }

    public static async Task<TResponse?> ReadFromJsonOrDefaultAsync<TResponse>(
        this HttpResponseMessage response,
        JsonTypeInfo<TResponse> jsonTypeInfo,
        CancellationToken cancellationToken = default)
    {
        await response.EnsureSuccess();

        // Standard `ReadFromJsonAsync` method throws an exception if it encounters an empty Content
        if (response.StatusCode == HttpStatusCode.NoContent || response.Content.Headers.ContentLength == 0)
        {
            // No content
            return default;
        }

        return await response.Content.ReadFromJsonAsync(jsonTypeInfo, cancellationToken).ConfigureAwait(false);
    }

    public static async Task<TResponse?> PutAsJsonAsync<TRequest, TResponse>(
        this HttpClient client,
        Uri requestUri,
        TRequest body,
        JsonTypeInfo<TRequest> requestJsonTypeInfo,
        JsonTypeInfo<TResponse> responseJsonTypeInfo,
        CancellationToken cancellationToken = default)
    {
        using var response = await client
            .PutAsJsonAsync(requestUri, body, requestJsonTypeInfo, cancellationToken)
            .ConfigureAwait(false);

        return await response.ReadFromJsonOrDefaultAsync(responseJsonTypeInfo, cancellationToken).ConfigureAwait(false);
    }

    // ---------------------------------------------------------------------------------
    // Reflection-based overloads. They require runtime reflection and are therefore
    // marked so that trimming/AOT consumers opt in explicitly.
    // ---------------------------------------------------------------------------------

    [RequiresUnreferencedCode(RequiresUnreferencedCodeMessage)]
    [RequiresDynamicCode(RequiresDynamicCodeMessage)]
    public static async Task<TResponse?> GetFromJsonOrDefaultAsync<TResponse>(
        this HttpClient client,
        Uri requestUri,
        CancellationToken cancellationToken = default)
    {
        using var response = await client.GetAsync(requestUri, cancellationToken).ConfigureAwait(false);

        return await response.ReadFromJsonOrDefaultAsync<TResponse?>(cancellationToken).ConfigureAwait(false);
    }

    [RequiresUnreferencedCode(RequiresUnreferencedCodeMessage)]
    [RequiresDynamicCode(RequiresDynamicCodeMessage)]
    public static async Task<TResponse?> PostAsJsonAsync<TRequest, TResponse>(
        this HttpClient client,
        Uri requestUri,
        TRequest body,
        CancellationToken cancellationToken = default)
    {
        using var response = await client.PostAsJsonAsync(requestUri, body, cancellationToken).ConfigureAwait(false);

        return await response.ReadFromJsonOrDefaultAsync<TResponse?>(cancellationToken).ConfigureAwait(false);
    }

    [RequiresUnreferencedCode(RequiresUnreferencedCodeMessage)]
    [RequiresDynamicCode(RequiresDynamicCodeMessage)]
    public static async Task<T?> PostAsync<T>(
        this HttpClient client,
        Uri requestUri,
        CancellationToken cancellationToken = default)
    {
        using var response = await client.PostAsync(requestUri, null, cancellationToken).ConfigureAwait(false);

        return await response.ReadFromJsonOrDefaultAsync<T?>(cancellationToken).ConfigureAwait(false);
    }

    [RequiresUnreferencedCode(RequiresUnreferencedCodeMessage)]
    [RequiresDynamicCode(RequiresDynamicCodeMessage)]
    public static async Task<TResponse?> ReadFromJsonOrDefaultAsync<TResponse>(
        this HttpResponseMessage response,
        CancellationToken cancellationToken = default)
    {
        await response.EnsureSuccess();

        // Standard `ReadFromJsonAsync` method throws an exception if it encounters an empty Content
        if (response.StatusCode == HttpStatusCode.NoContent || response.Content.Headers.ContentLength == 0)
        {
            // No content
            return default;
        }

        return await response.Content.ReadFromJsonAsync<TResponse>(s_options, cancellationToken: cancellationToken);
    }

    [RequiresUnreferencedCode(RequiresUnreferencedCodeMessage)]
    [RequiresDynamicCode(RequiresDynamicCodeMessage)]
    public static async Task<TResponse?> PutAsJsonAsync<TRequest, TResponse>(
        this HttpClient client,
        Uri requestUri,
        TRequest body,
        CancellationToken cancellationToken = default)
    {
        using var response = await client.PutAsJsonAsync(requestUri, body, cancellationToken).ConfigureAwait(false);

        return await response.ReadFromJsonOrDefaultAsync<TResponse?>(cancellationToken).ConfigureAwait(false);
    }
}
