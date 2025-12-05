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

        request.Metadata[OriginalFileNameMetadataKey] = fileName;

        if (metadata is not null)
        {
            foreach (var (key, value) in metadata)
            {
                request.Metadata[key] = value;
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

        return (metadata, new S3ResponseStream(response));
    }

    public async Task<BlobMetadata> ReadToStreamAsync(
        BlobId blobId,
        Stream outputStream,
        CancellationToken cancellationToken = default)
    {
        using var response = await _s3Client.GetObjectAsync(
            blobId.BucketName,
            blobId.ObjectKey,
            cancellationToken);

        await response.ResponseStream.CopyToAsync(outputStream, cancellationToken);

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

    private sealed class S3ResponseStream : Stream
    {
        private readonly GetObjectResponse _response;
        private readonly Stream _innerStream;
        private bool _disposed;

        public S3ResponseStream(GetObjectResponse response)
        {
            _response = response ?? throw new ArgumentNullException(nameof(response));
            _innerStream = response.ResponseStream ?? throw new ArgumentException("Response stream is null", nameof(response));
        }

        public override bool CanRead => _innerStream.CanRead;
        public override bool CanSeek => _innerStream.CanSeek;
        public override bool CanWrite => _innerStream.CanWrite;
        public override long Length => _innerStream.Length;

        public override long Position
        {
            get => _innerStream.Position;
            set => _innerStream.Position = value;
        }

        public override void Flush() => _innerStream.Flush();
        public override int Read(byte[] buffer, int offset, int count) => _innerStream.Read(buffer, offset, count);
        public override long Seek(long offset, SeekOrigin origin) => _innerStream.Seek(offset, origin);
        public override void SetLength(long value) => _innerStream.SetLength(value);
        public override void Write(byte[] buffer, int offset, int count) => _innerStream.Write(buffer, offset, count);

        public override Task FlushAsync(CancellationToken cancellationToken) => _innerStream.FlushAsync(cancellationToken);
        public override Task<int> ReadAsync(byte[] buffer, int offset, int count, CancellationToken cancellationToken)
            => _innerStream.ReadAsync(buffer, offset, count, cancellationToken);
        public override ValueTask<int> ReadAsync(Memory<byte> buffer, CancellationToken cancellationToken = default)
            => _innerStream.ReadAsync(buffer, cancellationToken);
        public override Task WriteAsync(byte[] buffer, int offset, int count, CancellationToken cancellationToken)
            => _innerStream.WriteAsync(buffer, offset, count, cancellationToken);
        public override ValueTask WriteAsync(ReadOnlyMemory<byte> buffer, CancellationToken cancellationToken = default)
            => _innerStream.WriteAsync(buffer, cancellationToken);

        protected override void Dispose(bool disposing)
        {
            if (_disposed)
            {
                base.Dispose(disposing);
                return;
            }

            if (disposing)
            {
                _innerStream.Dispose();
                _response.Dispose();
            }

            _disposed = true;
            base.Dispose(disposing);
        }

        public override async ValueTask DisposeAsync()
        {
            if (_disposed)
            {
                await base.DisposeAsync();
                return;
            }

            await _innerStream.DisposeAsync().ConfigureAwait(false);
            _response.Dispose();
            _disposed = true;
            await base.DisposeAsync().ConfigureAwait(false);
        }
    }
}
