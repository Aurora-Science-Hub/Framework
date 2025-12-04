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
    private const string Delimiter = "_";
    private const string Prefix = "blb";
    private const string PrefixWithDelimiter = Prefix + Delimiter;
    private const char PathSeparator = '/';
    private const char ExtensionSeparator = '.';

    /// <summary>
    /// Maximum length of the BlobId string representation
    /// </summary>
    public const int MaxLength = 255;

    /// <summary>
    /// Length of Base64Url encoded GUID (22 characters)
    /// </summary>
    public const int ObjectIdLength = 22;

    /// <summary>
    /// Maximum length of file extension (without dot)
    /// </summary>
    public const int MaxExtensionLength = 10;

    /// <summary>
    /// Maximum length of name prefix
    /// </summary>
    /// <remarks>
    /// Calculated as: MaxLength - "blb_" (4) - MaxBucketLength (63) - "_" (1) - "/" (1) - ObjectIdLength (22) - "." (1) - MaxExtensionLength (10) = 153
    /// </remarks>
    public const int MaxNamePrefixLength = 153;

    private BlobId(
        string bucketName,
        string objectId,
        string? namePrefix,
        string? extension)
    {
        BucketName = bucketName;
        ObjectId = objectId;
        NamePrefix = namePrefix;
        Extension = extension;
        ObjectKey = BuildObjectKey(objectId, namePrefix, extension);
        Value = $"{Prefix}{Delimiter}{BucketName}{Delimiter}{ObjectKey}";
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
    /// Blob's unique object identifier (base64url encoded GUID)
    /// </summary>
    /// <remarks>
    /// https://docs.aws.amazon.com/AmazonS3/latest/userguide/object-keys.html
    /// </remarks>
    public string ObjectId { get; }

    /// <summary>
    /// Optional path prefix (folder structure)
    /// </summary>
    public string? NamePrefix { get; }

    /// <summary>
    /// Optional file extension (without dot)
    /// </summary>
    public string? Extension { get; }

    /// <summary>
    /// Full object key for S3: [prefix/]objectId[.extension]
    /// </summary>
    public string ObjectKey { get; }

    private static string BuildObjectKey(string objectId, string? namePrefix, string? extension)
    {
        var result = objectId;

        if (!string.IsNullOrEmpty(extension))
        {
            result = $"{result}{ExtensionSeparator}{extension}";
        }

        if (!string.IsNullOrEmpty(namePrefix))
        {
            result = $"{namePrefix}{PathSeparator}{result}";
        }

        return result;
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

        var uniqueId = Guid.CreateVersion7();
        var objectId = Base64Url.EncodeToString(uniqueId.ToByteArray());

        return new BlobId(
            bucketName,
            objectId,
            NormalizePrefix(namePrefix),
            NormalizeExtension(extension));
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

        if (prefix.Contains(Delimiter))
        {
            throw new ArgumentException(
                $"Prefix cannot contain '{Delimiter}'",
                nameof(prefix));
        }

        if (prefix.Length > MaxNamePrefixLength)
        {
            throw new ArgumentException(
                $"Prefix length cannot exceed {MaxNamePrefixLength} characters",
                nameof(prefix));
        }
    }

    private static void ValidateExtension(string? extension)
    {
        if (string.IsNullOrWhiteSpace(extension))
            return;

        if (extension.Contains(Delimiter) || extension.Contains(PathSeparator))
        {
            throw new ArgumentException(
                "Extension cannot contain special characters",
                nameof(extension));
        }

        // Normalize for length check (remove leading dot if present)
        var normalized = extension.TrimStart(ExtensionSeparator);
        if (normalized.Length > MaxExtensionLength)
        {
            throw new ArgumentException(
                $"Extension length cannot exceed {MaxExtensionLength} characters",
                nameof(extension));
        }
    }

    private static string? NormalizePrefix(string? prefix)
    {
        if (string.IsNullOrWhiteSpace(prefix))
        {
            return null;
        }

        return prefix.Trim().Trim(PathSeparator);
    }

    private static string? NormalizeExtension(string? extension)
    {
        if (string.IsNullOrWhiteSpace(extension))
        {
            return null;
        }

        return extension.TrimStart(ExtensionSeparator).ToLowerInvariant();
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

        var objectIdStart = secondDelimiterIndex + 1;
        if (objectIdStart >= text.Length)
        {
            return false;
        }

        ReadOnlySpan<char> bucketSpan = text.Slice(firstDelimiterIndex, secondDelimiterIndex - firstDelimiterIndex);
        ReadOnlySpan<char> objectKeySpan = text.Slice(objectIdStart);

        if (bucketSpan.IsEmpty || objectKeySpan.IsEmpty)
        {
            return false;
        }

        if (!BucketNamingConvention.IsValid(bucketSpan, Delimiter))
        {
            return false;
        }

        // Parse object key: [prefix/]objectId[.extension]
        string? namePrefix = null;
        string objectId;
        string? extension = null;

        var lastSlash = objectKeySpan.LastIndexOf(PathSeparator);
        ReadOnlySpan<char> filePartSpan;

        if (lastSlash >= 0)
        {
            namePrefix = objectKeySpan.Slice(0, lastSlash).ToString();
            filePartSpan = objectKeySpan.Slice(lastSlash + 1);
        }
        else
        {
            filePartSpan = objectKeySpan;
        }

        var dotIndex = filePartSpan.LastIndexOf(ExtensionSeparator);
        if (dotIndex >= 0)
        {
            objectId = filePartSpan.Slice(0, dotIndex).ToString();
            extension = filePartSpan.Slice(dotIndex + 1).ToString();
        }
        else
        {
            objectId = filePartSpan.ToString();
        }

        if (string.IsNullOrEmpty(objectId))
        {
            return false;
        }

        result = new BlobId(bucketSpan.ToString(), objectId, namePrefix, extension);
        return true;
    }

    #endregion
}
