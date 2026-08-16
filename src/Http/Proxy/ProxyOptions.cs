namespace AuroraScienceHub.Framework.Http.Proxy;

/// <summary>
/// Proxy options
/// </summary>
public sealed class ProxyOptions
{
    public static readonly string OptionKey = "Proxy";

    /// <summary>
    /// Proxy address
    /// </summary>
    public Uri? Address { get; set; }

    /// <summary>
    /// User name
    /// </summary>
    public string? UserName { get; set; }

    /// <summary>
    /// Password
    /// </summary>
    public string? Password { get; set; }
}
