using System.Data;
using AuroraScienceHub.Framework.Entities.Storage;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;

namespace AuroraScienceHub.Framework.EntityFramework.Storage;

public abstract class DefaultUnitOfWork<TContext> :
    IUnitOfWork,
    IAsyncDisposable,
    IDisposable
    where TContext : DbContext
{
    private readonly TContext _context;
    private IDbContextTransaction? _transaction;

    protected DefaultUnitOfWork(TContext context)
    {
        _context = context;
    }

    public async Task BeginAsync(IsolationLevel isolationLevel, CancellationToken cancellationToken)
    {
        if (_transaction != null)
        {
            throw new InvalidOperationException("Transaction already started!");
        }

        _transaction = await _context.Database.BeginTransactionAsync(isolationLevel, cancellationToken)
            .ConfigureAwait(false);
    }

    public async Task CommitAsync(CancellationToken cancellationToken)
    {
        if (_transaction == null)
        {
            throw new InvalidOperationException("Transaction not started!");
        }

        try
        {
            await _context.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
            await _transaction.CommitAsync(cancellationToken).ConfigureAwait(false);
        }
        finally
        {
            await _transaction.DisposeAsync().ConfigureAwait(false);
            _transaction = null;
        }
    }

    public async Task RollbackAsync(CancellationToken cancellationToken)
    {
        if (_transaction == null)
        {
            return;
        }

        await _transaction.RollbackAsync(cancellationToken).ConfigureAwait(false);
        await _transaction.DisposeAsync().ConfigureAwait(false);
        _transaction = null;
    }

    public async ValueTask DisposeAsync()
    {
        if (_transaction != null)
        {
            await _transaction.DisposeAsync().ConfigureAwait(false);
        }
    }

    public void Dispose()
    {
        _transaction?.Dispose();
    }
}
