using AuroraScienceHub.Framework.ValueObjects.Blobs;

namespace AuroraScienceHub.Framework.Blobs;

/// <summary>
/// S3 Blob client interface
/// </summary>
public interface IBlobClient
{
    Task<BlobId> AddFileAsync(
        string bucket,
        string fileName,
        Stream uploadStream,
        string? contentType = null,
        CancellationToken cancellationToken = default);

    Task<BlobId> AddFileAsync(
        string fileName,
        Stream uploadStream,
        string? contentType = null,
        CancellationToken cancellationToken = default);

    Task<(BlobMetadata Metadata, byte[] Content)> GetAsync(
        BlobId blobId,
        CancellationToken cancellationToken = default);

    Task<(BlobMetadata Metadata, Stream Content)> GetStreamAsync(
        BlobId blobId,
        CancellationToken cancellationToken = default);

    Task<BlobMetadata> ReadToStreamAsync(
        BlobId blobId,
        Stream outputStream,
        CancellationToken cancellationToken = default);

    Task<BlobMetadata> GetMetadataAsync(
        BlobId blobId,
        CancellationToken cancellationToken = default);

    Task<bool> ExistsAsync(
        BlobId blobId,
        CancellationToken cancellationToken = default);

    Task DeleteAsync(
        BlobId blobId,
        CancellationToken cancellationToken = default);
}
