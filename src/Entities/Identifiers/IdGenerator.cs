using System.Buffers.Text;

namespace AuroraScienceHub.Framework.Entities.Identifiers;

/// <summary>
/// Identifier generator
/// </summary>
public static class IdGenerator
{
    private const char PrefixSeparator = '-';

    /// <summary>
    /// Generate a new unique identifier based on the prefix
    /// </summary>
    public static string NewId(string prefix)
    {
        if (string.IsNullOrWhiteSpace(prefix))
        {
            throw new ArgumentException("Prefix cannot be null, empty or white space", nameof(prefix));
        }

        if (!prefix.EndsWith(PrefixSeparator))
        {
            prefix += PrefixSeparator;
        }

        return prefix + NewId();
    }

    private static string NewId()
    {
        var uniqueId = Guid.CreateVersion7();
        var encoded = Base64Url.EncodeToString(uniqueId.ToByteArray());
        return encoded;
    }
}
