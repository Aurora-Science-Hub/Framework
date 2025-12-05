using Amazon.S3;
using Amazon.S3.Model;
using AuroraScienceHub.Framework.ValueObjects.Blobs;
using Microsoft.Extensions.Options;

namespace AuroraScienceHub.Framework.Blobs.S3;

internal sealed class S3BlobClient : IBlobClient
{
    private const string OriginalFileNameMetadataKey = "x-amz-meta-original-filename";

    private readonly IAmazonS3 _s3Client;
    private readonly S3Options _options;

    public S3BlobClient(IAmazonS3 s3Client, IOptions<S3Options> options)
    {
        _s3Client = s3Client;
        _options = options.Value;
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

        var request = new PutObjectRequest
        {
            BucketName = blobId.BucketName,
            Key = blobId.ObjectKey,
            InputStream = uploadStream,
            ContentType = contentType ?? ContentTypes.Application.OctetStream
        };

        request.Metadata.Add(OriginalFileNameMetadataKey, fileName);

        if (metadata is not null)
        {
            foreach (var (key, value) in metadata)
            {
                request.Metadata.Add(key, value);
            }
        }

        await _s3Client.PutObjectAsync(request, cancellationToken);

        return blobId;
    }

    public async Task<(BlobMetadata Metadata, byte[] Content)> GetAsync(
        BlobId blobId,
        CancellationToken cancellationToken = default)
    {
        using var response = await _s3Client.GetObjectAsync(
            blobId.BucketName,
            blobId.ObjectKey,
            cancellationToken);

        using var memoryStream = new MemoryStream();
        await response.ResponseStream.CopyToAsync(memoryStream, cancellationToken);

        var metadata = CreateBlobMetadata(blobId, response);

        return (metadata, memoryStream.ToArray());
    }

    public async Task<(BlobMetadata Metadata, Stream Content)> GetStreamAsync(
        BlobId blobId,
        CancellationToken cancellationToken = default)
    {
        var response = await _s3Client.GetObjectAsync(
            blobId.BucketName,
            blobId.ObjectKey,
            cancellationToken);

        var metadata = CreateBlobMetadata(blobId, response);

        return (metadata, response.ResponseStream);
    }

    public async Task<BlobMetadata> ReadToStreamAsync(
        BlobId blobId,
        Stream outputStream,
        CancellationToken cancellationToken = default)
    {
        var response = await _s3Client.GetObjectAsync(
            blobId.BucketName,
            blobId.ObjectKey,
            cancellationToken);

        await using (response.ResponseStream)
        {
            await response.ResponseStream.CopyToAsync(outputStream, cancellationToken);
        }

        return CreateBlobMetadata(blobId, response);
    }

    public async Task<BlobMetadata> GetMetadataAsync(
        BlobId blobId,
        CancellationToken cancellationToken = default)
    {
        var response = await _s3Client.GetObjectMetadataAsync(
            blobId.BucketName,
            blobId.ObjectKey,
            cancellationToken);

        return CreateBlobMetadata(blobId, response);
    }

    public async Task<bool> ExistsAsync(
        BlobId blobId,
        CancellationToken cancellationToken = default)
    {
        try
        {
            await _s3Client.GetObjectMetadataAsync(
                blobId.BucketName,
                blobId.ObjectKey,
                cancellationToken);
            return true;
        }
        catch (AmazonS3Exception ex) when (ex.StatusCode == System.Net.HttpStatusCode.NotFound)
        {
            return false;
        }
    }

    public async Task DeleteAsync(
        BlobId blobId,
        CancellationToken cancellationToken = default)
    {
        await _s3Client.DeleteObjectAsync(
            blobId.BucketName,
            blobId.ObjectKey,
            cancellationToken);
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
            {continue;}

            result[key] = metadataCollection[key];
        }

        return result.Count > 0 ? result : null;
    }
}
