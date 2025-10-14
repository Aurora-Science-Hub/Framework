using System.Globalization;

namespace AuroraScienceHub.Framework.Utilities.System;

/// <summary>
/// Extensions for <see cref="ReadOnlySpan{T}"/>
/// </summary>
public static partial class ReadOnlySpanExtensions
{
    /// <summary>
    /// Determines whether the specified value is parsable as an integer
    /// </summary>
    public static bool IsParsableAsInt(this ReadOnlySpan<char> value)
        => int.TryParse(value, NumberFormatInfo.InvariantInfo, out _);

    /// <summary>
    /// Parses the specified value as an integer
    /// </summary>
    public static int ParseIntInvariant(this ReadOnlySpan<char> value)
        => int.Parse(value, NumberFormatInfo.InvariantInfo);

    /// <summary>
    /// Parses the specified value as a float
    /// </summary>
    public static float ParseFloatInvariant(this ReadOnlySpan<char> value)
        => float.Parse(value, NumberFormatInfo.InvariantInfo);
}
