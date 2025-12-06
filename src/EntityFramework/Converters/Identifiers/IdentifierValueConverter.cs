using AuroraScienceHub.Framework.Entities.Identifiers;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace AuroraScienceHub.Framework.EntityFramework.Converters.Identifiers;

/// <summary>
/// Value converter for identifiers
/// </summary>
public sealed class IdentifierValueConverter<T> : ValueConverter<T, string>
    where T : IIdentifier<T>
{
    public IdentifierValueConverter()
        : base(
            input => input.Value,
            output => Parse(output),
            new ConverterMappingHints(size: 255))
    {
    }

    private static T Parse(string text) => T.Parse(text);
}
