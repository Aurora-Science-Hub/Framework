namespace AuroraScienceHub.Framework.AspNetCore.Diagnostics;

/// <summary>
/// API resource paths
/// </summary>
internal static class ApiResource
{
    private const string Prefix = "api";

    /// <summary>
    /// Diagnostics endpoints
    /// </summary>
    public static class Diagnostics
    {
        public const string About = Prefix + "/about";
    }
}
