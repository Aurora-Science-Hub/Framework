using System.Buffers.Text;
using System.Diagnostics.CodeAnalysis;
using System.Text.RegularExpressions;

namespace AuroraScienceHub.Framework.ValueObjects.Blobls;

/// <summary>
/// Идентификатор блоба
/// </summary>
public sealed class BlobId : IEquatable<BlobId>, ISpanParsable<BlobId>
{
    private static readonly Regex s_bucketNameRegex = new(@"^[a-z0-9][a-z0-9\.]*[a-z0-9]$", RegexOptions.Compiled);

    private const string Delimiter = "_";
    private const string Prefix = "blob";
    private const string PrefixWithDelimiter = Prefix + Delimiter;

    private BlobId(string bucketName, string objectId)
    {
        BucketName = bucketName;
        ObjectId = objectId;
        Value = $"{Prefix}{Delimiter}{BucketName}{Delimiter}{ObjectId}";
    }

    /// <summary>
    /// String representation of the BlobId
    /// </summary>
    public string Value { get; }

    /// <summary>
    /// Blob's bucket name
    /// </summary>
    /// <remarks>
    /// https://docs.aws.amazon.com/AmazonS3/latest/userguide/bucketnamingrules.html
    /// </remarks>
    public string BucketName { get; }

    /// <summary>
    /// Blob's object identifier
    /// </summary>
    /// <remarks>
    /// https://docs.aws.amazon.com/AmazonS3/latest/userguide/object-keys.html
    /// </remarks>
    public string ObjectId { get; }

    /// <summary>
    /// Create a new BlobId
    /// </summary>
    public static BlobId New(string bucketName)
    {
        if (!IsValidBucketName(bucketName))
        {
            throw new ArgumentException(
                $"Invalid bucket name. Must comply with S3 naming rules and not contain '{Delimiter}'",
                nameof(bucketName));
        }

        var uniqueId = Guid.CreateVersion7();
        return new BlobId(bucketName, Base64Url.EncodeToString(uniqueId.ToByteArray()));
    }

    /// <inheritdoc/>
    public override string ToString() => Value;

    #region Equatable Members

    /// <inheritdoc/>
    public bool Equals(BlobId? other)
    {
        if (ReferenceEquals(null, other))
        {
            return false;
        }

        if (ReferenceEquals(this, other))
        {
            return true;
        }

        return Value.AsSpan().SequenceEqual(other.Value.AsSpan());
    }

    /// <inheritdoc/>
    public override bool Equals(object? obj) => Equals(obj as BlobId);

    /// <inheritdoc/>
    public override int GetHashCode() => HashCode.Combine(BucketName, ObjectId);

    /// <summary>
    /// Equality operator for <see cref="BlobId"/>
    /// </summary>
    public static bool operator ==(BlobId? left, BlobId? right) => Equals(left, right);

    /// <summary>
    /// Inequality operator for <see cref="BlobId"/>
    /// </summary>
    public static bool operator !=(BlobId? left, BlobId? right) => !Equals(left, right);

    #endregion Equatable Members

    /// <summary>
    /// Parse <see cref="String"/> into <see cref="BlobId"/>
    /// </summary>
    public static BlobId Parse(string? text) => Parse(text, null);

    /// <summary>
    /// Parse <see cref="String"/> into <see cref="BlobId"/> with format provider
    /// </summary>
    public static BlobId Parse(string? text, IFormatProvider? provider)
        => TryParse(text, provider, out BlobId? blobId)
            ? blobId
            : throw new FormatException($"Wrong format of identifier for entity «{nameof(blobId)}»: {text}");

    /// <summary>
    /// Try to parse <see cref="String"/> into <see cref="BlobId"/>
    /// </summary>
    public static bool TryParse(
        [NotNullWhen(true)] string? text,
        [NotNullWhen(true)] out BlobId? result)
        => TryParse(text, null, out result);

    /// <summary>
    /// Try to parse <see cref="String"/> into <see cref="BlobId"/> with format provider
    /// </summary>
    public static bool TryParse(
        [NotNullWhen(true)] string? text,
        IFormatProvider? provider,
        [NotNullWhen(true)] out BlobId? result)
        => TryParse(text.AsSpan(), provider, out result);

    /// <summary>
    /// Parse <see cref="ReadOnlySpan{Char}"/> into <see cref="BlobId"/>
    /// </summary>
    public static BlobId Parse(ReadOnlySpan<char> text, IFormatProvider? provider)
        => TryParse(text, provider, out BlobId? blobId)
            ? blobId
            : throw new FormatException($"Wrong format of identifier for entity «{nameof(blobId)}»: {text.ToString()}");

    /// <summary>
    /// Try to parse <see cref="ReadOnlySpan{Char}"/> into <see cref="BlobId"/> using format provider
    /// </summary>
    public static bool TryParse(
        ReadOnlySpan<char> text,
        IFormatProvider? provider,
        [MaybeNullWhen(false)] out BlobId result)
    {
        result = null;

        if (text.IsEmpty)
        {
            return false;
        }

        // Prefix check
        if (!text.StartsWith(PrefixWithDelimiter.AsSpan(), StringComparison.Ordinal))
        {
            return false;
        }

        // Find first and second delimiters after prefix
        int firstDelimiterIndex = Prefix.Length + 1; // "blob_"
        int secondDelimiterIndex = text.Slice(firstDelimiterIndex).IndexOf(Delimiter);

        if (secondDelimiterIndex == -1)
        {
            return false;
        }

        secondDelimiterIndex += firstDelimiterIndex;

        int objectIdStart = secondDelimiterIndex + 1;
        if (objectIdStart >= text.Length)
        {
            return false;
        }

        ReadOnlySpan<char> bucketSpan = text.Slice(firstDelimiterIndex, secondDelimiterIndex - firstDelimiterIndex);
        ReadOnlySpan<char> objectSpan = text.Slice(objectIdStart);

        if (bucketSpan.IsEmpty || objectSpan.IsEmpty)
        {
            return false;
        }

        if (!IsValidBucketName(bucketSpan))
        {
            return false;
        }

        result = new BlobId(bucketSpan.ToString(), objectSpan.ToString());
        return true;
    }

    private static bool IsValidBucketName(ReadOnlySpan<char> bucketName)
    {
        // AWS S3 rules: 3-63 chars, lowercase, numbers, dots, hyphens
        // MinIO is less restrictive but let's follow S3 rules for compatibility.
        // https://docs.aws.amazon.com/AmazonS3/latest/userguide/bucketnamingrules.html
        if (bucketName.Length is < 3 or > 63)
        {
            return false;
        }

        if (bucketName.Contains(Delimiter.AsSpan(), StringComparison.Ordinal))
        {
            return false;
        }

        // Should respect S3 rules with only lowercase letters, numbers, dots. Excluding hyphens.
        return s_bucketNameRegex.IsMatch(bucketName);
    }
}
