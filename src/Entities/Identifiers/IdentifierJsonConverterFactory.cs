using System.Collections.Concurrent;
using System.Text.Json;
using System.Text.Json.Serialization;
using AuroraScienceHub.Framework.Utilities.System;

namespace AuroraScienceHub.Framework.Entities.Identifiers;

/// <summary>
/// JSON converter factory for <see cref="IIdentifier"/>
/// </summary>
public sealed class IdentifierJsonConverterFactory : JsonConverterFactory
{
    private static readonly ConcurrentDictionary<Type, Func<JsonConverter>> s_factoryCache = new();

    /// <inheritdoc/>
    public override bool CanConvert(Type typeToConvert)
    {
        return typeof(IIdentifier).IsAssignableFrom(typeToConvert);
    }

    /// <inheritdoc/>
    public override JsonConverter CreateConverter(Type typeToConvert, JsonSerializerOptions options)
    {
        var factory = s_factoryCache.GetOrAdd(typeToConvert, BuildFactory);
        return factory();
    }

    private static Func<JsonConverter> BuildFactory(Type typeToConvert)
    {
        var converterType = typeof(IdentifierJsonConverter<>).MakeGenericType(typeToConvert);
        return GenericActivator.BuildFactory<JsonConverter>(converterType);
    }
}
