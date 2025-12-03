using System.Text.RegularExpressions;

namespace AuroraScienceHub.Framework.ValueObjects.Blobs;

/// <summary>
/// Validates bucket names according to AWS S3 naming rules
/// </summary>
/// <remarks>
/// https://docs.aws.amazon.com/AmazonS3/latest/userguide/bucketnamingrules.html
/// </remarks>
internal sealed partial class BucketNamingConvention
{
    /// <summary>
    /// Validates bucket name according to S3 naming convention:
    /// - 3-63 characters
    /// - Only lowercase letters, numbers, dots and hyphens
    /// - Must start and end with lowercase letter or number
    /// - Cannot contain specified delimiter
    /// </summary>
    public static bool IsValid(ReadOnlySpan<char> bucketName, ReadOnlySpan<char> delimiter)
    {
        // AWS S3 rules: 3-63 chars
        if (bucketName.Length is < 3 or > 63)
        {
            return false;
        }

        // Cannot contain delimiter
        if (bucketName.Contains(delimiter, StringComparison.Ordinal))
        {
            return false;
        }

        // Should respect S3 rules: only lowercase letters, numbers, dots and hyphens
        // Must start and end with lowercase letter or number
        return BucketNameRegex().IsMatch(bucketName);
    }

    [GeneratedRegex(@"^[a-z0-9][a-z0-9\-\.]*[a-z0-9]$", RegexOptions.Compiled)]
    private static partial Regex BucketNameRegex();
}
