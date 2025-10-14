using System.Diagnostics.CodeAnalysis;
using System.Text;

namespace AuroraScienceHub.Framework.Http;

/// <summary>
/// A builder for creating URLs
/// </summary>
public sealed class UrlBuilder
{
    private bool _isBuilt;
    private readonly StringBuilder _builder;
    private readonly Dictionary<string, string?> _parameters = new();

    private UrlBuilder(string url)
    {
        _builder = new StringBuilder(url);
    }

    /// <summary>
    /// Create a URI from a <see cref="string"/>
    /// </summary>
    public static UrlBuilder From(string url)
    {
        return new UrlBuilder(url);
    }

    /// <summary>
    /// Create an uri from a <see cref="Uri"/>
    /// </summary>
    public static UrlBuilder From(Uri url)
    {
        return From(url.OriginalString);
    }

    /// <summary>
    /// Create an uri from a base and relative address
    /// </summary>
    public static UrlBuilder From(Uri baseUrl, string? relativeUrl)
    {
        return From(new Uri(baseUrl, relativeUrl));
    }

    /// <summary>
    /// Add a query parameter to the uri
    /// </summary>
    public UrlBuilder AddParameter(string name, string? value)
    {
        ThrowIfBuilt();

        if (string.IsNullOrWhiteSpace(name))
        {
            throw new ArgumentException("Parameter name cannot be null or empty", nameof(name));
        }
        if (!_parameters.TryAdd(name, value))
        {
            throw new ArgumentException($"Parameter with name '{name}' already exists", nameof(name));
        }

        return this;
    }

    /// <summary>
    /// Replace a segment in the uri
    /// </summary>
    public UrlBuilder ReplaceSegment(string segmentName, string value)
    {
        ThrowIfBuilt();

        _builder.Replace("{" + segmentName + "}", EscapeString(value));

        return this;
    }

    /// <summary>
    /// Build the uri
    /// </summary>
    public Uri Build()
    {
        ThrowIfBuilt();
        _isBuilt = true;

        ApplyParameters();

        var urlString = _builder.ToString();
        ValidateUrl(urlString);

        return new Uri(urlString, UriKind.RelativeOrAbsolute);
    }

    private void ApplyParameters()
    {
        const char querySeparator = '?';
        const char parameterSeparator = '&';

        var escapedParameters = _parameters
            .Where(parameter => string.IsNullOrEmpty(parameter.Value) == false)
            .Select(parameter => $"{EscapeString(parameter.Key)}={EscapeString(parameter.Value)}")
            .ToList();

        if (escapedParameters.Count == 0)
        {
            return;
        }

        var urlString = _builder.ToString();

        if (urlString.Contains(querySeparator))
        {
            if (!urlString.EndsWith(querySeparator) && !urlString.EndsWith(parameterSeparator))
            {
                _builder.Append(parameterSeparator);
            }
        }
        else
        {
            _builder.Append(querySeparator);
        }

        _builder.Append(string.Join(parameterSeparator, escapedParameters));
    }

    private void ThrowIfBuilt()
    {
        if (_isBuilt)
        {
            throw new InvalidOperationException("The URL has already been built.");
        }
    }

    [return: NotNullIfNotNull("value")]
    private static string? EscapeString(string? value)
    {
        return value == null ? null : Uri.EscapeDataString(value);
    }

    private static void ValidateUrl(string url)
    {
        if (url.Contains("{") || url.Contains("}"))
        {
            throw new InvalidOperationException("There are unreplaced placeholders in the URL");
        }
    }
}
