namespace AuroraScienceHub.Framework.Blobs.S3;

/// <summary>
/// S3 storage options
/// </summary>
public sealed class S3Options
{
    /// <summary>
    /// Configuration section name
    /// </summary>
    public static string SectionKey => "S3";

    /// <summary>
    /// Default bucket for storage
    /// </summary>
    public string? Bucket { get; set; }

    /// <summary>
    /// S3 Endpoint URL
    /// </summary>
    public string? ServiceUrl { get; set; }

    /// <summary>
    /// Access key (login)
    /// </summary>
    public string? AccessKey { get; set; }

    /// <summary>
    /// Secret key (password)
    /// </summary>
    public string? SecretKey { get; set; }

    /// <summary>
    /// Use HTTPS for S3 connections (default: false)
    /// </summary>
    public bool UseHttps { get; set; } = false;

    /// <summary>
    /// Default bucket for storage
    /// </summary>
    public string RequiredBucket
        => Bucket ?? throw new InvalidOperationException($"Configuration value «{nameof(Bucket)}» is not set");

    /// <summary>
    /// S3 Endpoint URL
    /// </summary>
    public string RequiredServiceUrl
        => ServiceUrl ?? throw new InvalidOperationException($"Configuration value «{nameof(ServiceUrl)}» is not set");

    /// <summary>
    /// Access key (login)
    /// </summary>
    public string RequiredAccessKey
        => AccessKey ?? throw new InvalidOperationException($"Configuration value «{nameof(AccessKey)}» is not set");

    /// <summary>
    /// Secret key (password)
    /// </summary>
    public string RequiredSecretKey
        => SecretKey ?? throw new InvalidOperationException($"Configuration value «{nameof(SecretKey)}» is not set");
}
