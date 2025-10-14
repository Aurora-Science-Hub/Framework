namespace AuroraScienceHub.Framework.Utilities.System;

/// <summary>
/// Utilities for <see cref="DateTime"/>
/// </summary>
public static class DateTimeUtils
{
    /// <summary>
    /// Returns the minimum of two nullable <see cref="DateTime"/> values
    /// </summary>
    public static DateTime? Min(DateTime? value1, DateTime? value2)
    {
        if (value1 is null && !value2.HasValue)
        {
            return null;
        }

        if (value1 is null)
        {
            return value2;
        }

        if (!value2.HasValue)
        {
            return value1;
        }

        return value1.Value < value2.Value ? value1 : value2;
    }

    /// <summary>
    /// Returns the maximum of two nullable <see cref="DateTime"/> values
    /// </summary>
    public static DateTime? Max(DateTime? value1, DateTime? value2)
    {
        if (value1 is null && !value2.HasValue)
        {
            return null;
        }

        if (value1 is null)
        {
            return value2;
        }

        if (!value2.HasValue)
        {
            return value1;
        }

        return value1.Value > value2.Value ? value1 : value2;
    }
}
