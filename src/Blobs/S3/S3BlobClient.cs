using Amazon.S3;
using Amazon.S3.Model;
using AuroraScienceHub.Framework.ValueObjects.Blobs;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace AuroraScienceHub.Framework.Blobs.S3;

internal sealed class S3BlobClient : IBlobClient
{
    // AWS SDK automatically adds "x-amz-meta-" prefix, so we use just the key name
    private const string OriginalFileNameMetadataKey = "original-filename";

    private readonly IAmazonS3 _s3Client;
    private readonly S3Options _options;
    private readonly ILogger<S3BlobClient> _logger;

    public S3BlobClient(IAmazonS3 s3Client, IOptions<S3Options> options, ILogger<S3BlobClient> logger)
    {
        _s3Client = s3Client;
        _options = options.Value;
        _logger = logger;
    }

    public Task<BlobId> AddFileAsync(
        string fileName,
        Stream uploadStream,
        string? contentType = null,
        IReadOnlyDictionary<string, string>? metadata = null,
        CancellationToken cancellationToken = default)
        => AddFileAsync(_options.RequiredBucket, fileName, uploadStream, contentType, metadata, cancellationToken);

    public async Task<BlobId> AddFileAsync(
        string bucket,
        string fileName,
        Stream uploadStream,
        string? contentType = null,
        IReadOnlyDictionary<string, string>? metadata = null,
        CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(bucket);
        ArgumentException.ThrowIfNullOrWhiteSpace(fileName);
        ArgumentNullException.ThrowIfNull(uploadStream);

        var extension = Path.GetExtension(fileName).TrimStart('.');
        var blobId = BlobId.New(bucket, string.IsNullOrEmpty(extension) ? null : extension);

        // Auto-detect content type if not provided
        var resolvedContentType = contentType ?? ContentTypeResolver.ResolveFromFileName(fileName);

        _logger.LogDebug(
            "Uploading file {FileName} to bucket {Bucket} with key {ObjectKey} and content type {ContentType}",
            fileName, blobId.BucketName, blobId.ObjectKey, resolvedContentType);

        var request = new PutObjectRequest
        {
            BucketName = blobId.BucketName,
            Key = blobId.ObjectKey,
            InputStream = uploadStream,
            ContentType = resolvedContentType
        };

        request.Metadata[OriginalFileNameMetadataKey] = fileName;

        if (metadata is not null)
        {
            foreach (var (key, value) in metadata)
            {
                request.Metadata[key] = value;
            }
        }

        try
        {
            await _s3Client.PutObjectAsync(request, cancellationToken);

            _logger.LogInformation(
                "Successfully uploaded file {FileName} to {BlobId}",
                fileName, blobId);
        }
        catch (AmazonS3Exception ex)
        {
            _logger.LogError(ex,
                "Failed to upload file {FileName} to bucket {Bucket}: {ErrorCode} - {ErrorMessage}",
                fileName, blobId.BucketName, ex.ErrorCode, ex.Message);
            throw;
        }

        return blobId;
    }

    public async Task<(BlobMetadata Metadata, byte[] Content)> GetAsync(
        BlobId blobId,
        CancellationToken cancellationToken = default)
    {
        _logger.LogDebug("Downloading blob {BlobId} into memory", blobId);

        try
        {
            using var response = await _s3Client.GetObjectAsync(
                blobId.BucketName,
                blobId.ObjectKey,
                cancellationToken);

            using var memoryStream = new MemoryStream();
            await response.ResponseStream.CopyToAsync(memoryStream, cancellationToken);

            var metadata = CreateBlobMetadata(blobId, response);

            _logger.LogInformation(
                "Successfully downloaded blob {BlobId}, size: {Size} bytes",
                blobId, metadata.Size);

            return (metadata, memoryStream.ToArray());
        }
        catch (AmazonS3Exception ex)
        {
            _logger.LogError(ex,
                "Failed to download blob {BlobId}: {ErrorCode} - {ErrorMessage}",
                blobId, ex.ErrorCode, ex.Message);
            throw;
        }
    }

    public async Task<BlobContent> GetStreamAsync(
        BlobId blobId,
        CancellationToken cancellationToken = default)
    {
        _logger.LogDebug("Opening stream for blob {BlobId}", blobId);

        try
        {
            var response = await _s3Client.GetObjectAsync(
                blobId.BucketName,
                blobId.ObjectKey,
                cancellationToken);

            var metadata = CreateBlobMetadata(blobId, response);

            _logger.LogInformation(
                "Successfully opened stream for blob {BlobId}, size: {Size} bytes",
                blobId, metadata.Size);

            return new BlobContent
            {
                Metadata = metadata,
                Stream = new S3ResponseStream(response)
            };
        }
        catch (AmazonS3Exception ex)
        {
            _logger.LogError(ex,
                "Failed to open stream for blob {BlobId}: {ErrorCode} - {ErrorMessage}",
                blobId, ex.ErrorCode, ex.Message);
            throw;
        }
    }

    public async Task<BlobMetadata> ReadToStreamAsync(
        BlobId blobId,
        Stream outputStream,
        CancellationToken cancellationToken = default)
    {
        _logger.LogDebug("Reading blob {BlobId} to output stream", blobId);

        try
        {
            using var response = await _s3Client.GetObjectAsync(
                blobId.BucketName,
                blobId.ObjectKey,
                cancellationToken);

            await response.ResponseStream.CopyToAsync(outputStream, cancellationToken);

            var metadata = CreateBlobMetadata(blobId, response);

            _logger.LogInformation(
                "Successfully read blob {BlobId} to output stream, size: {Size} bytes",
                blobId, metadata.Size);

            return metadata;
        }
        catch (AmazonS3Exception ex)
        {
            _logger.LogError(ex,
                "Failed to read blob {BlobId} to output stream: {ErrorCode} - {ErrorMessage}",
                blobId, ex.ErrorCode, ex.Message);
            throw;
        }
    }

    public async Task<BlobMetadata> GetMetadataAsync(
        BlobId blobId,
        CancellationToken cancellationToken = default)
    {
        _logger.LogDebug("Retrieving metadata for blob {BlobId}", blobId);

        try
        {
            var response = await _s3Client.GetObjectMetadataAsync(
                blobId.BucketName,
                blobId.ObjectKey,
                cancellationToken);

            var metadata = CreateBlobMetadata(blobId, response);

            _logger.LogDebug(
                "Successfully retrieved metadata for blob {BlobId}, size: {Size} bytes",
                blobId, metadata.Size);

            return metadata;
        }
        catch (AmazonS3Exception ex)
        {
            _logger.LogError(ex,
                "Failed to retrieve metadata for blob {BlobId}: {ErrorCode} - {ErrorMessage}",
                blobId, ex.ErrorCode, ex.Message);
            throw;
        }
    }

    public async Task<bool> ExistsAsync(
        BlobId blobId,
        CancellationToken cancellationToken = default)
    {
        _logger.LogDebug("Checking existence of blob {BlobId}", blobId);

        try
        {
            await _s3Client.GetObjectMetadataAsync(
                blobId.BucketName,
                blobId.ObjectKey,
                cancellationToken);

            _logger.LogDebug("Blob {BlobId} exists", blobId);
            return true;
        }
        catch (AmazonS3Exception ex) when (ex.StatusCode == System.Net.HttpStatusCode.NotFound)
        {
            _logger.LogDebug("Blob {BlobId} does not exist", blobId);
            return false;
        }
        catch (AmazonS3Exception ex) when (ex.StatusCode == System.Net.HttpStatusCode.Forbidden)
        {
            _logger.LogWarning(
                "Access denied when checking existence of blob {BlobId}: {ErrorMessage}",
                blobId, ex.Message);
            throw;
        }
        catch (AmazonS3Exception ex)
        {
            _logger.LogError(ex,
                "Error checking existence of blob {BlobId}: {ErrorCode} - {ErrorMessage}",
                blobId, ex.ErrorCode, ex.Message);
            throw;
        }
    }

    public async Task DeleteAsync(
        BlobId blobId,
        CancellationToken cancellationToken = default)
    {
        _logger.LogDebug("Deleting blob {BlobId}", blobId);

        try
        {
            // Check if blob exists before attempting deletion
            var exists = await ExistsAsync(blobId, cancellationToken);

            if (!exists)
            {
                _logger.LogWarning("Attempted to delete non-existent blob {BlobId}", blobId);
                return;
            }

            await _s3Client.DeleteObjectAsync(
                blobId.BucketName,
                blobId.ObjectKey,
                cancellationToken);

            _logger.LogInformation("Successfully deleted blob {BlobId}", blobId);
        }
        catch (AmazonS3Exception ex)
        {
            _logger.LogError(ex,
                "Failed to delete blob {BlobId}: {ErrorCode} - {ErrorMessage}",
                blobId, ex.ErrorCode, ex.Message);
            throw;
        }
    }

    private static BlobMetadata CreateBlobMetadata(BlobId blobId, GetObjectResponse response)
        => new()
        {
            Id = blobId,
            Size = response.ContentLength,
            ContentType = response.Headers.ContentType,
            LastModified = response.LastModified,
            ETag = response.ETag,
            OriginalFileName = ExtractOriginalFileName(response.Metadata),
            Metadata = ExtractMetadata(response.Metadata)
        };

    private static BlobMetadata CreateBlobMetadata(BlobId blobId, GetObjectMetadataResponse response)
        => new()
        {
            Id = blobId,
            Size = response.ContentLength,
            ContentType = response.Headers.ContentType,
            LastModified = response.LastModified,
            ETag = response.ETag,
            OriginalFileName = ExtractOriginalFileName(response.Metadata),
            Metadata = ExtractMetadata(response.Metadata)
        };

    private static string? ExtractOriginalFileName(MetadataCollection metadataCollection)
        => metadataCollection.Keys.Contains(OriginalFileNameMetadataKey)
            ? metadataCollection[OriginalFileNameMetadataKey]
            : null;

    private static IReadOnlyDictionary<string, string>? ExtractMetadata(MetadataCollection metadataCollection)
    {
        if (metadataCollection.Count == 0)
        {
            return null;
        }

        var result = new Dictionary<string, string>(metadataCollection.Count);
        foreach (var key in metadataCollection.Keys)
        {
            if (key.Equals(OriginalFileNameMetadataKey, StringComparison.OrdinalIgnoreCase))
            {
                continue;
            }

            result[key] = metadataCollection[key];
        }

        return result.Count > 0 ? result : null;
    }
}
