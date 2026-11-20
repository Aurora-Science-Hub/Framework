using System.Threading.Channels;
using AuroraScienceHub.Framework.Messaging.Rebus.PostgreSql;
using Microsoft.EntityFrameworkCore;
using Npgsql;
using Rebus.Activation;
using Rebus.Bus;
using Rebus.Config;
using Rebus.Config.Outbox;
using Rebus.Messages;
using Rebus.Pipeline;
using Rebus.Transport;
using Shouldly;
using Testcontainers.PostgreSql;
using Testcontainers.RabbitMq;
using Xunit;

namespace AuroraScienceHub.Framework.OutboxTests;

public sealed class PostgreSqlOutboxTests(OutboxInfrastructure infrastructure) : IClassFixture<OutboxInfrastructure>
{
    private static readonly TimeSpan DeliveryTimeout = TimeSpan.FromSeconds(45);

    [Fact(DisplayName = "An outbox scope requires an active PostgreSQL transaction")]
    public void UsePostgreSqlOutbox_WithoutTransaction_Throws()
    {
        // Arrange
        using var database = new TestDbContext(infrastructure.PostgreSql.GetConnectionString());
        using var scope = new RebusTransactionScope();

        // Act
        var exception = Should.Throw<InvalidOperationException>(() => scope.UsePostgreSqlOutbox(database.Database));

        // Assert
        exception.Message.ShouldContain("active Entity Framework Core PostgreSQL transaction");
    }

    [Fact(DisplayName = "Rolling back the EF Core transaction rolls back the outbox message")]
    public async Task Rollback_RemovesDomainChangeAndMessage()
    {
        // Arrange
        using var scenario = new OutboxScenario(infrastructure);
        var cancellationToken = TestContext.Current.CancellationToken;
        await scenario.InitializeAsync(cancellationToken);

        // Act
        await scenario.WriteAsync(Guid.NewGuid(), commit: false, cancellationToken);

        // Assert
        (await scenario.CountAsync(scenario.DomainTable, cancellationToken)).ShouldBe(0);
        (await scenario.CountAsync(scenario.OutboxTable, cancellationToken)).ShouldBe(0);
    }

    [Fact(DisplayName = "The outbox retains a message through a RabbitMQ outage and sends it after recovery")]
    public async Task RabbitMqOutage_RetainsCommittedMessage()
    {
        // Arrange
        using var scenario = new OutboxScenario(infrastructure);
        var cancellationToken = TestContext.Current.CancellationToken;
        await scenario.InitializeAsync(cancellationToken);
        var messageId = Guid.NewGuid();
        scenario.StopSubscriber();
        await infrastructure.SetBrokerRunningAsync(false, cancellationToken);

        try
        {
            // Act
            await scenario.WriteAsync(messageId, commit: true, cancellationToken);

            // Assert
            await scenario.WaitForDeliveryAttemptAsync(cancellationToken);
            (await scenario.CountAsync(scenario.DomainTable, cancellationToken)).ShouldBe(1);
            (await scenario.CountAsync(scenario.OutboxTable, cancellationToken)).ShouldBe(1);
            (await scenario.GetOldestMessageAgeAsync(cancellationToken)).ShouldBeGreaterThanOrEqualTo(TimeSpan.Zero);
        }
        finally
        {
            await infrastructure.SetBrokerRunningAsync(true, CancellationToken.None);
        }

        await scenario.StartSubscriberAsync();
        (await scenario.ReadReceiptAsync(cancellationToken)).ShouldBe(messageId.ToString());
        await scenario.WaitForEmptyOutboxAsync(cancellationToken);
    }

    [Fact(DisplayName = "A failed outbox commit after broker confirmation redelivers the same message ID")]
    public async Task CleanupCommitFails_RedeliversWithSameMessageId()
    {
        // Arrange
        using var scenario = new OutboxScenario(infrastructure);
        var cancellationToken = TestContext.Current.CancellationToken;
        await scenario.InitializeAsync(cancellationToken);
        await scenario.FailCleanupCommitAsync(cancellationToken);
        var messageId = Guid.NewGuid();

        // Act
        await scenario.WriteAsync(messageId, commit: true, cancellationToken);

        // Assert
        var firstReceipt = await scenario.ReadReceiptAsync(cancellationToken);
        var secondReceipt = await scenario.ReadReceiptAsync(cancellationToken);
        firstReceipt.ShouldBe(messageId.ToString());
        secondReceipt.ShouldBe(firstReceipt);
        (await scenario.CountAsync(scenario.OutboxTable, cancellationToken)).ShouldBe(1);

        await scenario.AllowCleanupCommitAsync(cancellationToken);
        await scenario.WaitForEmptyOutboxAsync(cancellationToken);
    }

    private sealed class TestDbContext(string connectionString) : DbContext
    {
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseNpgsql(connectionString);
        }
    }

    private sealed class OutboxScenario : IDisposable
    {
        private readonly string _connectionString;
        private BuiltinHandlerActivator? _receiver;
        private readonly BuiltinHandlerActivator _sender = new();
        private readonly Channel<string> _receipts = Channel.CreateUnbounded<string>();
        private readonly IBus _bus;
        private readonly string _function;
        private readonly string _brokerConnection;
        private readonly string _receiverQueue;

        public OutboxScenario(OutboxInfrastructure infrastructure)
        {
            var suffix = Guid.NewGuid().ToString("N");
            DomainTable = $"domain_{suffix}";
            OutboxTable = $"outbox_{suffix}";
            _function = $"fail_{suffix}";
            _connectionString = infrastructure.PostgreSql.GetConnectionString();
            _brokerConnection = infrastructure.RabbitMq.GetConnectionString();
            _receiverQueue = $"receiver-{suffix}";

            _bus = Configure.With(_sender)
                .Transport(transport => transport.UseRabbitMqAsOneWayClient(_brokerConnection)
                    .SetPublisherConfirms(true, TimeSpan.FromSeconds(2)))
                .Outbox(outbox => outbox.StoreInPostgreSql(_connectionString, OutboxTable))
                .Start();
        }

        public string DomainTable { get; }

        public string OutboxTable { get; }

        public async Task InitializeAsync(CancellationToken cancellationToken)
        {
            await ExecuteAsync($"CREATE TABLE {DomainTable} (id uuid PRIMARY KEY)", cancellationToken);
            await StartSubscriberAsync();
        }

        public async Task StartSubscriberAsync()
        {
            var receiver = new BuiltinHandlerActivator();
            _receiver = receiver;
            receiver.Handle<OutboxEvent>(_ =>
            {
                _receipts.Writer.TryWrite(MessageContext.Current.Headers[Headers.MessageId]);
                return Task.CompletedTask;
            });

            var receiverBus = Configure.With(receiver)
                .Transport(transport => transport.UseRabbitMq(_brokerConnection, _receiverQueue))
                .Start();

            await receiverBus.Subscribe<OutboxEvent>();
        }

        public void StopSubscriber()
        {
            _receiver?.Dispose();
            _receiver = null;
        }

        public async Task WriteAsync(Guid id, bool commit, CancellationToken cancellationToken)
        {
            await using var database = new TestDbContext(_connectionString);
            await using var transaction = await database.Database.BeginTransactionAsync(cancellationToken);
            using var scope = new RebusTransactionScope();
            scope.UsePostgreSqlOutbox(database.Database);

            var insert = $"INSERT INTO {DomainTable} (id) VALUES ({{0}})";
            await database.Database.ExecuteSqlRawAsync(insert, [id], cancellationToken);
            await _bus.Publish(new OutboxEvent(id),
                new Dictionary<string, string> { [Headers.MessageId] = id.ToString() });
            await scope.CompleteAsync();

            if (commit)
            {
                await transaction.CommitAsync(cancellationToken);
            }
            else
            {
                await transaction.RollbackAsync(cancellationToken);
            }
        }

        public Task FailCleanupCommitAsync(CancellationToken cancellationToken)
        {
            return ExecuteAsync($"""
                CREATE FUNCTION {_function}() RETURNS trigger LANGUAGE plpgsql AS $$
                BEGIN
                    RAISE EXCEPTION 'Simulated outbox commit failure';
                END;
                $$;
                CREATE CONSTRAINT TRIGGER fail_cleanup
                AFTER DELETE ON {OutboxTable}
                DEFERRABLE INITIALLY DEFERRED
                FOR EACH ROW EXECUTE FUNCTION {_function}();
                """, cancellationToken);
        }

        public Task AllowCleanupCommitAsync(CancellationToken cancellationToken)
        {
            return ExecuteAsync($"DROP TRIGGER fail_cleanup ON {OutboxTable}", cancellationToken);
        }

        public async Task<long> CountAsync(string table, CancellationToken cancellationToken)
        {
            await using var connection = new NpgsqlConnection(_connectionString);
            await connection.OpenAsync(cancellationToken);
            await using var command = new NpgsqlCommand($"SELECT count(*) FROM {table}", connection);
            return (long)(await command.ExecuteScalarAsync(cancellationToken))!;
        }

        public async Task<TimeSpan> GetOldestMessageAgeAsync(CancellationToken cancellationToken)
        {
            await using var connection = new NpgsqlConnection(_connectionString);
            await connection.OpenAsync(cancellationToken);
            await using var command = new NpgsqlCommand($"""
                SELECT now() - min(("Headers"::jsonb ->> 'rbs2-senttime')::timestamptz)
                FROM {OutboxTable}
                """, connection);
            return (TimeSpan)(await command.ExecuteScalarAsync(cancellationToken))!;
        }

        public async Task<string> ReadReceiptAsync(CancellationToken cancellationToken)
        {
            using var timeout = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);
            timeout.CancelAfter(DeliveryTimeout);
            return await _receipts.Reader.ReadAsync(timeout.Token);
        }

        public Task WaitForDeliveryAttemptAsync(CancellationToken cancellationToken)
        {
            return WaitUntilAsync(async token =>
            {
                await using var connection = new NpgsqlConnection(_connectionString);
                await connection.OpenAsync(token);
                // Rebus locks the table for deletion until the transport send completes.
                await using var command = new NpgsqlCommand("""
                    SELECT EXISTS (
                        SELECT FROM pg_locks
                        WHERE relation = to_regclass(@table) AND mode = 'RowExclusiveLock' AND granted
                    )
                    """, connection);
                command.Parameters.AddWithValue("table", OutboxTable);
                return (bool)(await command.ExecuteScalarAsync(token))!;
            }, cancellationToken);
        }

        public Task WaitForEmptyOutboxAsync(CancellationToken cancellationToken)
        {
            return WaitUntilAsync(async token => await CountAsync(OutboxTable, token) == 0, cancellationToken);
        }

        private static async Task WaitUntilAsync(
            Func<CancellationToken, Task<bool>> condition,
            CancellationToken cancellationToken)
        {
            using var timeout = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);
            timeout.CancelAfter(DeliveryTimeout);

            while (!await condition(timeout.Token))
            {
                await Task.Delay(TimeSpan.FromMilliseconds(100), timeout.Token);
            }
        }

        public void Dispose()
        {
            _sender.Dispose();
            StopSubscriber();
        }

        private async Task ExecuteAsync(string sql, CancellationToken cancellationToken)
        {
            await using var connection = new NpgsqlConnection(_connectionString);
            await connection.OpenAsync(cancellationToken);
            await using var command = new NpgsqlCommand(sql, connection);
            await command.ExecuteNonQueryAsync(cancellationToken);
        }
    }

    public sealed record OutboxEvent(Guid Id);
}

public sealed class OutboxInfrastructure : IAsyncLifetime
{
    public PostgreSqlContainer PostgreSql { get; } = new PostgreSqlBuilder("postgres:17-alpine").Build();

    public RabbitMqContainer RabbitMq { get; } = new RabbitMqBuilder("rabbitmq:4.4.0-management-alpine").Build();

    public async ValueTask InitializeAsync()
    {
        var cancellationToken = TestContext.Current.CancellationToken;
        await Task.WhenAll(PostgreSql.StartAsync(cancellationToken), RabbitMq.StartAsync(cancellationToken));
    }

    public async ValueTask DisposeAsync()
    {
        await Task.WhenAll(PostgreSql.DisposeAsync().AsTask(), RabbitMq.DisposeAsync().AsTask());
    }

    public async Task SetBrokerRunningAsync(bool running, CancellationToken cancellationToken)
    {
        var command = running ? "start_app" : "stop_app";
        var result = await RabbitMq.ExecAsync(["rabbitmqctl", command], cancellationToken);
        result.ExitCode.ShouldBe(0, result.Stderr);
    }
}
