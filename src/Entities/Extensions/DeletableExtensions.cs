namespace AuroraScienceHub.Framework.Entities.Extensions;

public static class DeletableExtensions
{
    public static bool IsDeleted(this ISoftDeletable entity)
    {
        return entity.DeletedAt is not null;
    }
}
