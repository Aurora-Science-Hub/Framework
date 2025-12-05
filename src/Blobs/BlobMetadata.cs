using AuroraScienceHub.Framework.ValueObjects.Blobs;

namespace AuroraScienceHub.Framework.Blobs;

/// <summary>
/// Metadata for a blob stored in S3
/// </summary>
public sealed record BlobMetadata
{
    /// <summary>
    /// The unique identifier of the blob
    /// </summary>
    public required BlobId Id { get; init; }

    /// <summary>
    /// The size of the blob in bytes
    /// </summary>
    public required long Size { get; init; }

    /// <summary>
    /// The content type (MIME type) of the blob
    /// </summary>
    public string? ContentType { get; init; }

    /// <summary>
    /// The date and time when the blob was last modified
    /// </summary>
    public DateTimeOffset? LastModified { get; init; }

    /// <summary>
    /// The ETag of the blob (usually MD5 hash)
    /// </summary>
    public string? ETag { get; init; }

    /// <summary>
    /// Original file name provided during upload
    /// </summary>
    public string? OriginalFileName { get; init; }

    /// <summary>
    /// Custom user-defined metadata associated with the blob
    /// </summary>
    public IReadOnlyDictionary<string, string>? Metadata { get; init; }
}

