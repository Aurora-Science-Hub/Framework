using AuroraScienceHub.Framework.EntityFramework.Configuration;

namespace AuroraScienceHub.Framework.EntityFramework;

public interface IDataContext
{
    static virtual string Schema => "public";

    static virtual string MigrationHistoryTable => MigrationConstants.MigrationHistoryTable;
}
