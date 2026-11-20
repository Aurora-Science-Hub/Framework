# PostgreSQL outbox for Rebus

Use this package to publish from a background job or application service that owns an EF Core
PostgreSQL transaction. Rebus stores the serialized message in that transaction and forwards
it after commit.

Configure the same Rebus bus that publishes the event:

```csharp
using Rebus.Config;
using Rebus.Config.Outbox;

// Use the connection string for the database behind dbContext.
services.AddRebus(configure => configure
    .Transport(transport => transport.UseRabbitMq(rabbitMqConnectionString, queueName)
        .SetPublisherConfirms(true, TimeSpan.FromSeconds(5)))
    .Outbox(outbox => outbox.StoreInPostgreSql(postgresConnectionString, "identity_outbox")));
```

Rebus creates the table at startup. The database role needs permission to create it on initial
startup and to read, insert and delete rows during operation. Use the same table for replicas
of the publisher.

Publish from the job using the configured bus:

```csharp
using AuroraScienceHub.Framework.Messaging.Rebus.PostgreSql;
using Microsoft.EntityFrameworkCore;
using Rebus.Messages;
using Rebus.Transport;

await using var transaction = await dbContext.Database.BeginTransactionAsync(cancellationToken);
using var scope = new RebusTransactionScope();
scope.UsePostgreSqlOutbox(dbContext.Database);

await dbContext.SaveChangesAsync(cancellationToken);
await bus.Publish(change, new Dictionary<string, string>
{
    [Headers.MessageId] = eventId.ToString()
});
await scope.CompleteAsync();
await transaction.CommitAsync(cancellationToken);
```

Use an event ID that stays the same across retries and make consumers idempotent. Rebus retries
failed delivery with delays and removes messages after a successful transport send. A failure
between the RabbitMQ confirmation and the PostgreSQL commit can redeliver the same message ID.
Keep publisher confirmations enabled and ensure the subscriber queue and binding exist before
publishing. A broker confirmation does not acknowledge processing by the consumer.

The extension requires an active PostgreSQL transaction. Always use the outbox-configured bus
and call it outside a Rebus message handler. This integration is tested for explicitly owned
transactions. If the app enables EF Core execution retries, run the entire transaction through
the EF Core execution strategy.

Inspect pending deliveries using a read-only query. The age uses the UTC timestamp in the
original Rebus message headers.

```sql
SELECT count(*) AS pending,
       now() - min(("Headers"::jsonb ->> 'rbs2-senttime')::timestamptz) AS oldest_message_age
FROM identity_outbox;
```

Run the integration tests with Docker available:

```sh
dotnet test --project tests/OutboxTests/OutboxTests.csproj
```
