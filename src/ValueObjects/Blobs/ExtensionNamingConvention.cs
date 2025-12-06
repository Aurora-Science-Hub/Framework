// filepath: /Users/Alex/Source/SWeather/SWeather-framework/src/ValueObjects/Blobs/ExtensionNamingConvention.cs
namespace AuroraScienceHub.Framework.ValueObjects.Blobs;

/// <summary>
/// Validates and normalizes file extension
/// </summary>
internal static class ExtensionNamingConvention
{
    private const char ExtensionSeparator = '.';
    private const char PathSeparator = '/';

    /// <summary>
    /// Maximum length of file extension (without dot)
    /// </summary>
    public const int MaxLength = 10;

    /// <summary>
    /// Validates file extension:
    /// - Cannot exceed MaxLength characters
    /// - Cannot contain delimiter or path separator
    /// </summary>
    public static bool IsValid(ReadOnlySpan<char> extension, ReadOnlySpan<char> delimiter)
    {
        if (extension.IsEmpty || extension.IsWhiteSpace())
        {
            return true; // Empty extension is valid
        }

        // Normalize for validation (remove leading dot if present)
        var normalized = extension.TrimStart(ExtensionSeparator);

        if (normalized.Length > MaxLength)
        {
            return false;
        }

        if (normalized.Contains(delimiter, StringComparison.Ordinal))
        {
            return false;
        }

        if (normalized.Contains(PathSeparator))
        {
            return false;
        }

        return true;
    }

    /// <summary>
    /// Normalizes file extension by removing leading dot and converting to lowercase
    /// </summary>
    public static string? Normalize(string? extension)
    {
        if (string.IsNullOrWhiteSpace(extension))
        {
            return null;
        }

        return extension.TrimStart(ExtensionSeparator).ToLowerInvariant();
    }
}

