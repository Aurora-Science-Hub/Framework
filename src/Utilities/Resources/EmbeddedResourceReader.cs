using System.Reflection;
using System.Text.Json;

namespace AuroraScienceHub.Framework.Utilities.Resources;

/// <summary>
/// Helper class for reading embedded resources
/// </summary>
public static class EmbeddedResourceReader
{
    /// <summary>
    /// Reads the text content of the embedded resource
    /// </summary>
    public static async Task<string> ReadTextAsync(
        Assembly assembly,
        string resourceFilePath,
        CancellationToken cancellationToken = default)
    {
        await using var stream = assembly.GetManifestResourceStream(resourceFilePath)
                                 ?? throw new FileNotFoundException($"Embedded resource '{resourceFilePath}' not found.");

        using var reader = new StreamReader(stream);
        return await reader.ReadToEndAsync(cancellationToken).ConfigureAwait(false);
    }

    /// <summary>
    /// Reads the binary content of the embedded resource
    /// </summary>
    public static async Task<byte[]> ReadBinaryAsync(
        Assembly assembly,
        string resourceFilePath,
        CancellationToken cancellationToken = default)
    {
        await using var stream = assembly.GetManifestResourceStream(resourceFilePath)
                                 ?? throw new FileNotFoundException($"Embedded resource '{resourceFilePath}' not found.");

        using var memoryStream = new MemoryStream();
        await stream.CopyToAsync(memoryStream, cancellationToken).ConfigureAwait(false);
        return memoryStream.ToArray();
    }

    /// <summary>
    /// Reads the json content of the embedded resource
    /// </summary>
    public static async Task<T> ReadJsonAsync<T>(
        Assembly assembly,
        string resourceFilePath,
        JsonSerializerOptions? options = null,
        CancellationToken cancellationToken = default)
    {
        await using var stream = assembly.GetManifestResourceStream(resourceFilePath)
                                 ?? throw new FileNotFoundException($"Embedded resource '{resourceFilePath}' not found.");

        using var reader = new StreamReader(stream);
        var json = await reader.ReadToEndAsync(cancellationToken).ConfigureAwait(false);
        return JsonSerializer.Deserialize<T>(json, options)
               ?? throw new InvalidOperationException("Could not deserialize JSON.");
    }
}
