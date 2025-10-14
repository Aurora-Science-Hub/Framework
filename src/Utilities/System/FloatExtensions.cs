namespace AuroraScienceHub.Framework.Utilities.System;

/// <summary>
/// Extensions for <see cref="float"/>
/// </summary>
public static class FloatExtensions
{
    private const float DefaultEpsilon = 0.00001f;

    /// <summary>
    /// Determines whether the specified value is close to another value within a certain epsilon
    /// </summary>
    public static bool IsCloseTo(this float a, float b, float epsilon = DefaultEpsilon)
    {
        return Math.Abs(a - b) < epsilon;
    }

    /// <summary>
    /// Determines whether the specified value is close to another value within a certain epsilon
    /// </summary>
    public static bool IsCloseTo(this float? a, float b, float epsilon = DefaultEpsilon)
    {
        return a is not null && Math.Abs(a.Value - b) < epsilon;
    }

    /// <summary>
    /// Determines whether the specified value is close to another value within a certain epsilon
    /// </summary>
    public static bool IsCloseTo(this float? a, float? b, float epsilon = DefaultEpsilon)
    {
        return (a is null && b is null)
               || a is not null && b is not null && Math.Abs(a.Value - b.Value) < epsilon;
    }
}
