using Amazon.S3.Model;

namespace AuroraScienceHub.Framework.Blobs.S3;

/// <summary>
/// A wrapper stream that manages the lifecycle of an AWS S3 GetObjectResponse and its underlying response stream.
/// </summary>
/// <remarks>
/// <para>
/// This stream ensures that both the response stream and the GetObjectResponse object are properly disposed
/// when the stream is disposed. This is necessary because the GetObjectResponse must remain alive
/// for the duration of the stream reading operation.
/// </para>
/// <para>
/// The stream delegates all read operations to the underlying response stream while managing
/// the lifecycle of the parent GetObjectResponse object.
/// </para>
/// </remarks>
internal sealed class S3ResponseStream : Stream
{
    private readonly GetObjectResponse _response;
    private readonly Stream _innerStream;
    private bool _disposed;

    /// <summary>
    /// Initializes a new instance of the <see cref="S3ResponseStream"/> class.
    /// </summary>
    /// <param name="response">The AWS S3 GetObjectResponse containing the response stream.</param>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="response"/> is null.</exception>
    /// <exception cref="ArgumentException">Thrown when the response stream is null.</exception>
    public S3ResponseStream(GetObjectResponse response)
    {
        _response = response ?? throw new ArgumentNullException(nameof(response));
        _innerStream = response.ResponseStream ?? throw new ArgumentException("Response stream is null", nameof(response));
    }

    /// <inheritdoc/>
    public override bool CanRead => _innerStream.CanRead;

    /// <inheritdoc/>
    public override bool CanSeek => _innerStream.CanSeek;

    /// <inheritdoc/>
    public override bool CanWrite => _innerStream.CanWrite;

    /// <inheritdoc/>
    public override long Length => _innerStream.Length;

    /// <inheritdoc/>
    public override long Position
    {
        get => _innerStream.Position;
        set => _innerStream.Position = value;
    }

    /// <inheritdoc/>
    public override void Flush() => _innerStream.Flush();

    /// <inheritdoc/>
    public override Task FlushAsync(CancellationToken cancellationToken) => _innerStream.FlushAsync(cancellationToken);

    /// <inheritdoc/>
    public override int Read(byte[] buffer, int offset, int count) => _innerStream.Read(buffer, offset, count);

    /// <inheritdoc/>
    public override Task<int> ReadAsync(byte[] buffer, int offset, int count, CancellationToken cancellationToken)
        => _innerStream.ReadAsync(buffer, offset, count, cancellationToken);

    /// <inheritdoc/>
    public override ValueTask<int> ReadAsync(Memory<byte> buffer, CancellationToken cancellationToken = default)
        => _innerStream.ReadAsync(buffer, cancellationToken);

    /// <inheritdoc/>
    public override long Seek(long offset, SeekOrigin origin) => _innerStream.Seek(offset, origin);

    /// <inheritdoc/>
    public override void SetLength(long value) => _innerStream.SetLength(value);

    /// <inheritdoc/>
    public override void Write(byte[] buffer, int offset, int count) => _innerStream.Write(buffer, offset, count);

    /// <inheritdoc/>
    public override Task WriteAsync(byte[] buffer, int offset, int count, CancellationToken cancellationToken)
        => _innerStream.WriteAsync(buffer, offset, count, cancellationToken);

    /// <inheritdoc/>
    public override ValueTask WriteAsync(ReadOnlyMemory<byte> buffer, CancellationToken cancellationToken = default)
        => _innerStream.WriteAsync(buffer, cancellationToken);

    /// <summary>
    /// Disposes the stream and the underlying GetObjectResponse.
    /// </summary>
    /// <param name="disposing">True if called from Dispose; false if called from finalizer.</param>
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

    /// <summary>
    /// Asynchronously disposes the stream and the underlying GetObjectResponse.
    /// </summary>
    /// <returns>A ValueTask representing the asynchronous dispose operation.</returns>
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

