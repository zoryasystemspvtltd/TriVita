using IdentityService.Domain.Entities.Rbac;

namespace IdentityService.Application.Abstractions;

public interface IAccountLockoutRepository
{
    Task<IdentityAccountLockoutState?> GetAsync(long userId, CancellationToken cancellationToken = default);

    Task RecordFailedAttemptAsync(
        long userId,
        long tenantId,
        int maxAttempts,
        int lockoutMinutes,
        CancellationToken cancellationToken = default);

    Task ClearFailuresAsync(long userId, CancellationToken cancellationToken = default);

    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}
