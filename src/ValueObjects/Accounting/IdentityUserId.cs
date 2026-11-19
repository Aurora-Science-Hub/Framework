using System.Text.Json.Serialization;


namespace AuroraScienceHub.Framework.ValueObjects.Accounting;

/// <summary>
/// User identifier assigned by an identity provider
/// </summary>
[JsonConverter(typeof(IdentityUserIdJsonConverter))]
public sealed record IdentityUserId
{
    /// <summary>Creates an identifier from a user subject</summary>
    /// <param name="value">Nonblank subject from the identity provider</param>
    public IdentityUserId(string value)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(value);
        Value = value;
    }

    /// <summary>Original subject value</summary>
    public string Value { get; }

    /// <inheritdoc />
    public override string ToString() => Value;
}
