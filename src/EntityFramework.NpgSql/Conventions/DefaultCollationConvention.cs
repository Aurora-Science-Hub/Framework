using AuroraScienceHub.Framework.Entities.Identifiers;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.Extensions.DependencyInjection;

namespace AuroraScienceHub.Framework.EntityFramework.NpgSql.Conventions;

/// <summary>
/// Extensions for <see cref="ModelConfigurationBuilder"/> to set a default collation for string properties.
/// </summary>
public static class ModelConfigurationBuilderExtensions
{
    public static string DefaultCollation => "C";

    /// <summary>
    /// Applies the default collation C to string and identifier properties in the model configuration.
    /// </summary>
    public static ModelConfigurationBuilder HasDefaultCollation(this ModelConfigurationBuilder configurationBuilder)
    {
        configurationBuilder.Properties<string>().UseCollation(DefaultCollation);
        configurationBuilder.Properties<IIdentifier>().UseCollation(DefaultCollation);

        return configurationBuilder;
    }
}
