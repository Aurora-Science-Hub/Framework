using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
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
    private static readonly JsonSerializerOptions s_options = DefaultJsonSerializerOptions.Create();

    public static async Task<TResponse?> GetFromJsonOrDefaultAsync<TResponse>(
        this HttpClient client,
        Uri requestUri,
        CancellationToken cancellationToken = default)
    {
        using var response = await client.GetAsync(requestUri, cancellationToken).ConfigureAwait(false);

        return await response.ReadFromJsonOrDefaultAsync<TResponse?>(cancellationToken).ConfigureAwait(false);
    }

    public static async Task<TResponse?> PostAsJsonAsync<TRequest, TResponse>(
        this HttpClient client,
        Uri requestUri,
        TRequest body,
        CancellationToken cancellationToken = default)
    {
        using var response = await client.PostAsJsonAsync(requestUri, body, cancellationToken).ConfigureAwait(false);

        return await response.ReadFromJsonOrDefaultAsync<TResponse?>(cancellationToken).ConfigureAwait(false);
    }

    public static async Task<T?> PostAsync<T>(
        this HttpClient client,
        Uri requestUri,
        CancellationToken cancellationToken = default)
    {
        using var response = await client.PostAsync(requestUri, null, cancellationToken).ConfigureAwait(false);

        return await response.ReadFromJsonOrDefaultAsync<T?>(cancellationToken).ConfigureAwait(false);
    }

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
