using System.Data;
using Healthcare.Common.Responses;
using PharmacyService.Domain.Repositories;
using Microsoft.EntityFrameworkCore;

namespace PharmacyService.Infrastructure.Persistence;

public sealed class PharmacyUnitOfWork : IPharmacyUnitOfWork
{
    private readonly PharmacyDbContext _db;

    public PharmacyUnitOfWork(PharmacyDbContext db)
    {
        _db = db;
    }

    public async Task ExecuteInTransactionAsync(
        Func<CancellationToken, Task> action,
        CancellationToken cancellationToken = default,
        IsolationLevel isolationLevel = IsolationLevel.ReadCommitted)
    {
        await using var tx = await _db.Database.BeginTransactionAsync(isolationLevel, cancellationToken);
        try
        {
            await action(cancellationToken);
            await _db.SaveChangesAsync(cancellationToken);
            await tx.CommitAsync(cancellationToken);
        }
        catch
        {
            await tx.RollbackAsync(cancellationToken);
            throw;
        }
    }

    public async Task<T> ExecuteInTransactionAsync<T>(
        Func<CancellationToken, Task<T>> action,
        CancellationToken cancellationToken = default,
        IsolationLevel isolationLevel = IsolationLevel.ReadCommitted)
    {
        await using var tx = await _db.Database.BeginTransactionAsync(isolationLevel, cancellationToken);
        try
        {
            var result = await action(cancellationToken);
            if (result is IBaseResponse { Success: false })
            {
                await tx.RollbackAsync(cancellationToken);
                _db.ChangeTracker.Clear();
                return result;
            }

            await _db.SaveChangesAsync(cancellationToken);
            await tx.CommitAsync(cancellationToken);
            return result;
        }
        catch
        {
            await tx.RollbackAsync(cancellationToken);
            throw;
        }
    }
}

