using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage;
using Npgsql;
using Rebus.Config.Outbox;
using Rebus.Transport;

namespace AuroraScienceHub.Framework.Messaging.Rebus.PostgreSql;

/// <summary>
/// Binds a Rebus outbox scope to an Entity Framework Core transaction.
/// </summary>
public static class RebusTransactionScopeExtensions
{
    /// <summary>
    /// Enlists outgoing messages in the active PostgreSQL transaction.
    /// Configure the publishing bus with the PostgreSQL outbox before using this method.
    /// Complete the scope before committing the database transaction.
    /// </summary>
    /// <param name="scope">The Rebus scope used for publishing.</param>
    /// <param name="database">The database facade with an active PostgreSQL transaction.</param>
    /// <exception cref="InvalidOperationException">The database has no active PostgreSQL transaction.</exception>
    public static void UsePostgreSqlOutbox(this RebusTransactionScope scope, DatabaseFacade database)
    {
        ArgumentNullException.ThrowIfNull(scope);
        ArgumentNullException.ThrowIfNull(database);

        if (database.CurrentTransaction?.GetDbTransaction() is not NpgsqlTransaction { Connection: { } connection } transaction)
        {
            throw new InvalidOperationException("An active Entity Framework Core PostgreSQL transaction is required.");
        }

        scope.UseOutbox(connection, transaction);
    }
}
