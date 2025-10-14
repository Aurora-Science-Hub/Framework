namespace Ai.Proxy;

/// <summary>
/// Proxy options
/// </summary>
public sealed class ProxyOptions
{
    public static readonly string OptionKey = "Proxy";

    /// <summary>
    /// Proxy address
    /// </summary>
    public Uri? Address { get; init; }

    /// <summary>
    /// User name
    /// </summary>
    public string? UserName { get; init; }

    /// <summary>
    /// Password
    /// </summary>
    public string? Password { get; init; }
}
