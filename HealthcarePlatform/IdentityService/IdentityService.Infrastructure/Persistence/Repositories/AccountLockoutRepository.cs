using IdentityService.Application.Abstractions;
using IdentityService.Domain.Entities.Rbac;
using IdentityService.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace IdentityService.Infrastructure.Persistence.Repositories;

public sealed class AccountLockoutRepository : IAccountLockoutRepository
{
    private readonly IdentityDbContext _db;

    public AccountLockoutRepository(IdentityDbContext db)
    {
        _db = db;
    }

    public Task<IdentityAccountLockoutState?> GetAsync(long userId, CancellationToken cancellationToken = default) =>
        _db.IdentityAccountLockoutStates.AsNoTracking()
            .FirstOrDefaultAsync(s => s.UserId == userId && !s.IsDeleted, cancellationToken);

    public async Task RecordFailedAttemptAsync(
        long userId,
        long tenantId,
        int maxAttempts,
        int lockoutMinutes,
        CancellationToken cancellationToken = default)
    {
        var row = await _db.IdentityAccountLockoutStates.AsTracking()
            .FirstOrDefaultAsync(s => s.UserId == userId, cancellationToken);
        var now = DateTime.UtcNow;
        if (row is null)
        {
            row = new IdentityAccountLockoutState
            {
                UserId = userId,
                TenantId = tenantId,
                FailedAttemptCount = 1,
                LastFailedAttemptOn = now,
                CreatedOn = now,
                ModifiedOn = now,
            };
            await _db.IdentityAccountLockoutStates.AddAsync(row, cancellationToken);
        }
        else
        {
            row.FailedAttemptCount++;
            row.LastFailedAttemptOn = now;
            row.ModifiedOn = now;
            if (row.FailedAttemptCount >= maxAttempts)
                row.LockoutEndOn = now.AddMinutes(lockoutMinutes);
        }
    }

    public async Task ClearFailuresAsync(long userId, CancellationToken cancellationToken = default)
    {
        var row = await _db.IdentityAccountLockoutStates.AsTracking()
            .FirstOrDefaultAsync(s => s.UserId == userId, cancellationToken);
        if (row is null)
            return;
        row.FailedAttemptCount = 0;
        row.LockoutEndOn = null;
        row.ModifiedOn = DateTime.UtcNow;
    }

    public Task<int> SaveChangesAsync(CancellationToken cancellationToken = default) =>
        _db.SaveChangesAsync(cancellationToken);
}
