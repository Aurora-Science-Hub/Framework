using System.Runtime.InteropServices;

namespace AuroraScienceHub.Framework.Utilities.Regional;

/// <summary>
/// Utilities for <see cref="TimeZoneInfo"/>
/// </summary>
public class TimeZoneInfoUtils
{
    private static readonly string s_mskTimeZoneId = RuntimeInformation.IsOSPlatform(OSPlatform.Windows)
        ? "Russian Standard Time"
        : "Europe/Moscow";

    /// <summary>
    /// <see cref="TimeZoneInfo"/> for the Russian Standard Time
    /// </summary>
    public static TimeZoneInfo RussianStandardTime { get; } = TimeZoneInfo.FindSystemTimeZoneById(s_mskTimeZoneId);
}
