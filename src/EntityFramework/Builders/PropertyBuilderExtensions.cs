using AuroraScienceHub.Framework.Entities;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AuroraScienceHub.Framework.EntityFramework.Builders;

public static class PropertyBuilderExtensions
{
    public static PropertyBuilder<TEntity> HasShortString<TEntity>(this PropertyBuilder<TEntity> propertyBuilder)
    {
        return propertyBuilder.HasMaxLength(EntityConfiguration.ShortString.MaxLength);
    }

    public static PropertyBuilder HasShortString(this PropertyBuilder propertyBuilder)
    {
        return propertyBuilder.HasMaxLength(EntityConfiguration.ShortString.MaxLength);
    }
}
