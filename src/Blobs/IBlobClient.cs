using AuroraScienceHub.Framework.ValueObjects.Blobs;

namespace AuroraScienceHub.Framework.Blobs;

/// <summary>
/// S3 Blob client interface
/// </summary>
public interface IBlobClient
{
    public Task<BlobId> AddFileAsync(
        string bucket,
        string fileName,
        Stream uploadStream,
        CancellationToken cancellationToken = default);

    public Task<BlobId> AddFileAsync(
        string fileName,
        Stream uploadStream,
        CancellationToken cancellationToken = default);

    public Task<(BlobMetadata, byte[])> GetAsync(
        BlobId blobId,
        CancellationToken
            cancellationToken = default);

    public Task<BlobMetadata> ReadToStreamAsync(
        BlobId blobId,
        Stream outputStream,
        CancellationToken cancellationToken = default);

    public Task<BlobMetadata> GetMetadataAsync(BlobId blobId, CancellationToken cancellationToken = default);

    public Task<bool> IsExistsAsync(
        BlobId blobId,
        CancellationToken cancellationToken = default);
}
