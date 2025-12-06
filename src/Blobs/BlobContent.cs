namespace AuroraScienceHub.Framework.Blobs;

/// <summary>
/// Represents blob content with its metadata and a stream for reading the data.
/// </summary>
/// <remarks>
/// <para>
/// This type implements <see cref="IAsyncDisposable"/> and <see cref="IDisposable"/>.
/// Always dispose this object after use to release underlying resources (network connections, streams, etc.).
/// </para>
/// <para>
/// Recommended usage with <c>await using</c>:
/// <code>
/// await using var content = await blobClient.GetStreamAsync(blobId);
/// // Use content.Stream to read data
/// // Use content.Metadata to access blob information
/// </code>
/// </para>
/// </remarks>
public sealed class BlobContent : IAsyncDisposable, IDisposable
{
    /// <summary>
    /// Gets the metadata of the blob.
    /// </summary>
    public required BlobMetadata Metadata { get; init; }

    /// <summary>
    /// Gets the stream for reading the blob content.
    /// </summary>
    /// <remarks>
    /// The stream is disposed when this <see cref="BlobContent"/> instance is disposed.
    /// </remarks>
    public required Stream Stream { get; init; }

    /// <inheritdoc />
    public async ValueTask DisposeAsync()
    {
        await Stream.DisposeAsync();
    }

    /// <inheritdoc />
    public void Dispose()
    {
        Stream.Dispose();
    }
}

