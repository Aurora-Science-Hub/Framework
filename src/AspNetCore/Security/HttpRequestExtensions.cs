using Microsoft.AspNetCore.Http;

namespace AuroraScienceHub.Framework.AspNetCore.Security;

public static class HttpRequestExtensions
{
    private static readonly IReadOnlySet<string> s_browserIdentifiers = new HashSet<string>(
        ["Mozilla", "Chrome", "Safari", "Opera", "Edge", "Firefox"],
        StringComparer.OrdinalIgnoreCase);

    public static bool IsWebBrowser(this HttpRequest request)
    {
        if (!request.Headers.TryGetValue("User-Agent", out var userAgent))
        {
            return false;
        }

        var userAgentString = userAgent.ToString();
        return s_browserIdentifiers.Any(identifier =>
            userAgentString.Contains(identifier, StringComparison.OrdinalIgnoreCase));
    }
}
