using IdentityService.Domain.Entities.Rbac;

namespace IdentityService.Application.Abstractions;

public interface IPasswordResetRepository
{
    Task AddAsync(IdentityPasswordResetToken entity, CancellationToken cancellationToken = default);

    Task<IdentityPasswordResetToken?> FindActiveByUserAndHashAsync(
        long userId,
        string tokenHash,
        CancellationToken cancellationToken = default);

    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}
