using AuroraScienceHub.Framework.Entities.Identifiers;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace AuroraScienceHub.Framework.EntityFramework.Converters.Identifiers;

/// <summary>
/// Value converter for identifiers
/// </summary>
public sealed class NullableIdentifierValueConverter<T> : ValueConverter<T?, string?>
    where T : IIdentifier<T>
{
    public NullableIdentifierValueConverter()
        : base(input => input != null ? input.Value : null,
            output => ParseOrDefault(output))
    {
    }

    private static T? ParseOrDefault(string? text)
        => T.TryParse(text, out var result)
            ? result
            : default;
}
