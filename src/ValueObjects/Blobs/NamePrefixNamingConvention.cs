namespace AuroraScienceHub.Framework.ValueObjects.Blobs;

/// <summary>
/// Validates and normalizes name prefix according to S3 object key rules
/// </summary>
/// <remarks>
/// https://docs.aws.amazon.com/AmazonS3/latest/userguide/object-keys.html
/// </remarks>
internal static class NamePrefixNamingConvention
{
    private const char PathSeparator = '/';

    /// <summary>
    /// Maximum length of name prefix
    /// </summary>
    /// <remarks>
    /// Calculated as: MaxLength (255) - "blb_" (4) - MaxBucketLength (63) - "_" (1) - "/" (1) - ObjectIdLength (22) - "." (1) - MaxExtensionLength (10) = 153
    /// </remarks>
    public const int MaxLength = 153;

    /// <summary>
    /// Validates name prefix according to naming convention:
    /// - Cannot exceed MaxLength characters
    /// - Cannot contain specified delimiter
    /// </summary>
    public static bool IsValid(ReadOnlySpan<char> prefix, ReadOnlySpan<char> delimiter)
    {
        if (prefix.IsEmpty || prefix.IsWhiteSpace())
        {
            return true; // Empty prefix is valid
        }

        if (prefix.Length > MaxLength)
        {
            return false;
        }

        if (prefix.Contains(delimiter, StringComparison.Ordinal))
        {
            return false;
        }

        return true;
    }

    /// <summary>
    /// Normalizes name prefix by trimming whitespace and path separators
    /// </summary>
    public static string? Normalize(string? prefix)
    {
        if (string.IsNullOrWhiteSpace(prefix))
        {
            return null;
        }

        return prefix.Trim().Trim(PathSeparator);
    }
}

