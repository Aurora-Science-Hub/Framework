using System.Buffers.Text;
using System.Diagnostics.CodeAnalysis;

namespace AuroraScienceHub.Framework.ValueObjects.Blobs;

/// <summary>
/// MinIO (S3) Blob Identifier with optional prefix and extension support
/// </summary>
/// <remarks>
/// Format: blb_{bucket}_{objectKey} where objectKey = [prefix/]uniqueId[.extension]
/// </remarks>
public sealed class BlobId : IEquatable<BlobId>, ISpanParsable<BlobId>
{
    private const string Prefix = "blb";
    private const string Delimiter = "_";
    private const string PrefixWithDelimiter = Prefix + Delimiter;
    private const char PathSeparator = '/';
    private const char ExtensionSeparator = '.';

    /// <summary>
    /// Maximum length of the BlobId string representation
    /// </summary>
    public const int MaxLength = 255;

    private BlobId(string bucketName, string objectKey)
    {
        BucketName = bucketName;
        ObjectKey = objectKey;
        Value = $"{Prefix}{Delimiter}{BucketName}{Delimiter}{ObjectKey}";
    }

    /// <summary>
    /// String representation of the BlobId
    /// </summary>
    public string Value { get; }

    /// <summary>
    /// S3 bucket name
    /// </summary>
    /// <remarks>
    /// https://docs.aws.amazon.com/AmazonS3/latest/userguide/bucketnamingrules.html
    /// </remarks>
    public string BucketName { get; }

    /// <summary>
    /// S3 object key (full path including prefix and extension)
    /// </summary>
    /// <remarks>
    /// https://docs.aws.amazon.com/AmazonS3/latest/userguide/object-keys.html
    /// </remarks>
    public string ObjectKey { get; }

    /// <summary>
    /// File name extracted from ObjectKey (last segment after '/')
    /// </summary>
    /// <example>
    /// For ObjectKey "users/photos/abc123.jpg" returns "abc123.jpg"
    /// For ObjectKey "abc123.jpg" returns "abc123.jpg"
    /// </example>
    public string FileName
    {
        get
        {
            var lastSlashIndex = ObjectKey.LastIndexOf(PathSeparator);
            return lastSlashIndex >= 0
                ? ObjectKey[(lastSlashIndex + 1)..]
                : ObjectKey;
        }
    }

    private static string BuildObjectKey(string uniqueId, string? namePrefix, string? extension)
    {
        var normalizedPrefix = NamePrefixNamingConvention.Normalize(namePrefix);
        var normalizedExtension = ExtensionNamingConvention.Normalize(extension);

        return (normalizedPrefix, normalizedExtension) switch
        {
            (null, null) => uniqueId,
            (null, _) => $"{uniqueId}{ExtensionSeparator}{normalizedExtension}",
            (_, null) => $"{normalizedPrefix}{PathSeparator}{uniqueId}",
            _ => $"{normalizedPrefix}{PathSeparator}{uniqueId}{ExtensionSeparator}{normalizedExtension}"
        };
    }

    /// <summary>
    /// Creates a new BlobId with just bucket name
    /// </summary>
    public static BlobId New(string bucketName)
        => New(bucketName, namePrefix: null, extension: null);

    /// <summary>
    /// Creates a new BlobId with bucket name and extension
    /// </summary>
    public static BlobId New(string bucketName, string? extension)
        => New(bucketName, namePrefix: null, extension);

    /// <summary>
    /// Creates a new BlobId with all parameters
    /// </summary>
    public static BlobId New(string bucketName, string? namePrefix, string? extension)
    {
        ValidateBucketName(bucketName);
        ValidateNamePrefix(namePrefix);
        ValidateExtension(extension);

        var uniqueId = Base64Url.EncodeToString(Guid.CreateVersion7().ToByteArray());
        var objectKey = BuildObjectKey(uniqueId, namePrefix, extension);

        return new BlobId(bucketName, objectKey);
    }

    private static void ValidateBucketName(string bucketName)
    {
        if (!BucketNamingConvention.IsValid(bucketName, Delimiter))
        {
            throw new ArgumentException(
                $"Invalid bucket name. Must comply with S3 naming rules and not contain '{Delimiter}'",
                nameof(bucketName));
        }
    }

    private static void ValidateNamePrefix(string? prefix)
    {
        if (string.IsNullOrWhiteSpace(prefix))
            return;

        if (!NamePrefixNamingConvention.IsValid(prefix, Delimiter))
        {
            throw new ArgumentException(
                $"Invalid prefix. Cannot contain '{Delimiter}' and must not exceed {NamePrefixNamingConvention.MaxLength} characters",
                nameof(prefix));
        }
    }

    private static void ValidateExtension(string? extension)
    {
        if (string.IsNullOrWhiteSpace(extension))
            return;

        if (!ExtensionNamingConvention.IsValid(extension, Delimiter))
        {
            throw new ArgumentException(
                $"Invalid extension. Cannot contain special characters and must not exceed {ExtensionNamingConvention.MaxLength} characters",
                nameof(extension));
        }
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

        return Value == other.Value;
    }

    /// <inheritdoc/>
    public override bool Equals(object? obj) => Equals(obj as BlobId);

    /// <inheritdoc/>
    public override int GetHashCode() => Value.GetHashCode();

    /// <summary>
    /// Equality operator for <see cref="BlobId"/>
    /// </summary>
    public static bool operator ==(BlobId? left, BlobId? right) => Equals(left, right);

    /// <summary>
    /// Inequality operator for <see cref="BlobId"/>
    /// </summary>
    public static bool operator !=(BlobId? left, BlobId? right) => !Equals(left, right);

    #endregion Equatable Members

    #region Parsable Members

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
            : throw new FormatException(
                $"Wrong format of identifier for entity `{nameof(BlobId)}`: {text}");

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
            : throw new FormatException(
                $"Wrong format of identifier for entity `{nameof(BlobId)}`: {text}");

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
        var firstDelimiterIndex = PrefixWithDelimiter.Length; // 4
        var secondDelimiterIndex = text.Slice(firstDelimiterIndex).IndexOf(Delimiter);
        if (secondDelimiterIndex < 0)
        {
            return false;
        }

        secondDelimiterIndex += firstDelimiterIndex;

        var objectKeyStart = secondDelimiterIndex + 1;
        if (objectKeyStart >= text.Length)
        {
            return false;
        }

        ReadOnlySpan<char> bucketSpan = text.Slice(firstDelimiterIndex, secondDelimiterIndex - firstDelimiterIndex);
        ReadOnlySpan<char> objectKeySpan = text.Slice(objectKeyStart);

        if (bucketSpan.IsEmpty || objectKeySpan.IsEmpty)
        {
            return false;
        }

        if (!BucketNamingConvention.IsValid(bucketSpan, Delimiter))
        {
            return false;
        }

        result = new BlobId(bucketSpan.ToString(), objectKeySpan.ToString());
        return true;
    }

    #endregion
}
