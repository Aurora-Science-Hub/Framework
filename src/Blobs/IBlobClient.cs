using AuroraScienceHub.Framework.ValueObjects.Blobs;

namespace AuroraScienceHub.Framework.Blobs;

/// <summary>
/// Provides an abstraction for interacting with blob storage systems (e.g., AWS S3, Azure Blob Storage).
/// </summary>
/// <remarks>
/// This interface defines operations for uploading, downloading, and managing binary large objects (BLOBs).
/// For AWS S3 documentation, see <see href="https://docs.aws.amazon.com/s3/"/>.
/// </remarks>
public interface IBlobClient
{
    /// <summary>
    /// Uploads a file to the specified bucket.
    /// </summary>
    /// <param name="bucket">The name of the bucket where the file will be stored.</param>
    /// <param name="fileName">
    /// The original file name. The file extension will be extracted and used in the blob identifier.
    /// This name is also stored in the blob metadata for future reference.
    /// </param>
    /// <param name="uploadStream">The stream containing the file data to upload.</param>
    /// <param name="contentType">
    /// The MIME type of the file (e.g., "image/jpeg", "application/pdf").
    /// If not specified, defaults to "application/octet-stream".
    /// See <see href="https://docs.aws.amazon.com/AmazonS3/latest/API/API_PutObject.html#API_PutObject_RequestSyntax"/> for details.
    /// </param>
    /// <param name="metadata">
    /// Optional user-defined metadata to associate with the blob as key-value pairs.
    /// These will be stored as S3 object metadata and can be retrieved later.
    /// See <see href="https://docs.aws.amazon.com/AmazonS3/latest/userguide/UsingMetadata.html"/> for more information.
    /// </param>
    /// <param name="cancellationToken">A cancellation token to cancel the operation.</param>
    /// <returns>A <see cref="BlobId"/> that uniquely identifies the uploaded blob.</returns>
    /// <exception cref="ArgumentException">Thrown when <paramref name="bucket"/> or <paramref name="fileName"/> is null or whitespace.</exception>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="uploadStream"/> is null.</exception>
    Task<BlobId> AddFileAsync(
        string bucket,
        string fileName,
        Stream uploadStream,
        string? contentType = null,
        IReadOnlyDictionary<string, string>? metadata = null,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Uploads a file to the default bucket configured in the storage options.
    /// </summary>
    /// <param name="fileName">
    /// The original file name. The file extension will be extracted and used in the blob identifier.
    /// This name is also stored in the blob metadata for future reference.
    /// </param>
    /// <param name="uploadStream">The stream containing the file data to upload.</param>
    /// <param name="contentType">
    /// The MIME type of the file (e.g., "image/jpeg", "application/pdf").
    /// If not specified, defaults to "application/octet-stream".
    /// </param>
    /// <param name="metadata">
    /// Optional user-defined metadata to associate with the blob as key-value pairs.
    /// These will be stored as S3 object metadata and can be retrieved later.
    /// </param>
    /// <param name="cancellationToken">A cancellation token to cancel the operation.</param>
    /// <returns>A <see cref="BlobId"/> that uniquely identifies the uploaded blob.</returns>
    /// <exception cref="ArgumentException">Thrown when <paramref name="fileName"/> is null or whitespace.</exception>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="uploadStream"/> is null.</exception>
    /// <exception cref="InvalidOperationException">Thrown when the default bucket is not configured.</exception>
    Task<BlobId> AddFileAsync(
        string fileName,
        Stream uploadStream,
        string? contentType = null,
        IReadOnlyDictionary<string, string>? metadata = null,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Downloads a blob and loads its entire content into memory as a byte array.
    /// </summary>
    /// <param name="blobId">The unique identifier of the blob to retrieve.</param>
    /// <param name="cancellationToken">A cancellation token to cancel the operation.</param>
    /// <returns>
    /// A tuple containing the blob's metadata and its content as a byte array.
    /// </returns>
    /// <remarks>
    /// <para>
    /// ⚠️ <b>Warning:</b> This method loads the entire file content into memory.
    /// For large files, this may cause high memory consumption or <see cref="OutOfMemoryException"/>.
    /// Consider using <see cref="GetStreamAsync"/> or <see cref="ReadToStreamAsync"/> for large files.
    /// </para>
    /// <para>
    /// See <see href="https://docs.aws.amazon.com/AmazonS3/latest/API/API_GetObject.html"/> for S3 GetObject documentation.
    /// </para>
    /// </remarks>
    /// <exception cref="Amazon.S3.AmazonS3Exception">Thrown when the blob does not exist or access is denied.</exception>
    Task<(BlobMetadata Metadata, byte[] Content)> GetAsync(
        BlobId blobId,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Downloads a blob and returns its content as a stream for efficient processing of large files.
    /// </summary>
    /// <param name="blobId">The unique identifier of the blob to retrieve.</param>
    /// <param name="cancellationToken">A cancellation token to cancel the operation.</param>
    /// <returns>
    /// A tuple containing the blob's metadata and a <see cref="Stream"/> for reading the content.
    /// </returns>
    /// <remarks>
    /// <para>
    /// The returned stream must be disposed by the caller to release resources properly.
    /// The stream wraps the underlying S3 response and will dispose it when the stream is disposed.
    /// </para>
    /// <para>
    /// This method is recommended for large files as it allows streaming the content without loading it entirely into memory.
    /// </para>
    /// <para>
    /// See <see href="https://docs.aws.amazon.com/AmazonS3/latest/API/API_GetObject.html"/> for S3 GetObject documentation.
    /// </para>
    /// </remarks>
    /// <exception cref="Amazon.S3.AmazonS3Exception">Thrown when the blob does not exist or access is denied.</exception>
    Task<(BlobMetadata Metadata, Stream Content)> GetStreamAsync(
        BlobId blobId,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Downloads a blob and writes its content directly to the provided output stream.
    /// </summary>
    /// <param name="blobId">The unique identifier of the blob to retrieve.</param>
    /// <param name="outputStream">The destination stream where the blob content will be written.</param>
    /// <param name="cancellationToken">A cancellation token to cancel the operation.</param>
    /// <returns>The metadata of the downloaded blob.</returns>
    /// <remarks>
    /// <para>
    /// This method is useful when you already have a target stream (e.g., file stream, network stream)
    /// and want to copy the blob content directly without additional buffering.
    /// </para>
    /// <para>
    /// The <paramref name="outputStream"/> is not disposed by this method - the caller remains responsible for its lifecycle.
    /// </para>
    /// <para>
    /// See <see href="https://docs.aws.amazon.com/AmazonS3/latest/API/API_GetObject.html"/> for S3 GetObject documentation.
    /// </para>
    /// </remarks>
    /// <exception cref="Amazon.S3.AmazonS3Exception">Thrown when the blob does not exist or access is denied.</exception>
    Task<BlobMetadata> ReadToStreamAsync(
        BlobId blobId,
        Stream outputStream,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Retrieves the metadata of a blob without downloading its content.
    /// </summary>
    /// <param name="blobId">The unique identifier of the blob.</param>
    /// <param name="cancellationToken">A cancellation token to cancel the operation.</param>
    /// <returns>The metadata of the blob, including size, content type, last modified date, ETag, and custom metadata.</returns>
    /// <remarks>
    /// <para>
    /// This is a lightweight operation that only retrieves the blob's metadata (headers) without downloading the actual content.
    /// Use this method when you only need information about the blob, such as its size or modification date.
    /// </para>
    /// <para>
    /// See <see href="https://docs.aws.amazon.com/AmazonS3/latest/API/API_HeadObject.html"/> for S3 HeadObject documentation.
    /// </para>
    /// </remarks>
    /// <exception cref="Amazon.S3.AmazonS3Exception">Thrown when the blob does not exist or access is denied.</exception>
    Task<BlobMetadata> GetMetadataAsync(
        BlobId blobId,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Checks whether a blob exists in the storage.
    /// </summary>
    /// <param name="blobId">The unique identifier of the blob to check.</param>
    /// <param name="cancellationToken">A cancellation token to cancel the operation.</param>
    /// <returns>
    /// <c>true</c> if the blob exists; otherwise, <c>false</c>.
    /// </returns>
    /// <remarks>
    /// <para>
    /// This method performs a HEAD request to check for the blob's existence without downloading its content.
    /// It returns <c>false</c> if the blob is not found (HTTP 404).
    /// </para>
    /// <para>
    /// Note that other errors (e.g., access denied, network issues) will throw exceptions.
    /// </para>
    /// <para>
    /// See <see href="https://docs.aws.amazon.com/AmazonS3/latest/API/API_HeadObject.html"/> for S3 HeadObject documentation.
    /// </para>
    /// </remarks>
    /// <exception cref="Amazon.S3.AmazonS3Exception">Thrown when an error occurs other than "not found" (e.g., access denied).</exception>
    Task<bool> ExistsAsync(
        BlobId blobId,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Deletes a blob from the storage.
    /// </summary>
    /// <param name="blobId">The unique identifier of the blob to delete.</param>
    /// <param name="cancellationToken">A cancellation token to cancel the operation.</param>
    /// <returns>A task representing the asynchronous delete operation.</returns>
    /// <remarks>
    /// <para>
    /// This operation is idempotent - deleting a non-existent blob will not throw an error.
    /// S3 returns success even if the object did not exist.
    /// </para>
    /// <para>
    /// See <see href="https://docs.aws.amazon.com/AmazonS3/latest/API/API_DeleteObject.html"/> for S3 DeleteObject documentation.
    /// </para>
    /// </remarks>
    /// <exception cref="Amazon.S3.AmazonS3Exception">Thrown when an error occurs (e.g., access denied).</exception>
    Task DeleteAsync(
        BlobId blobId,
        CancellationToken cancellationToken = default);
}
