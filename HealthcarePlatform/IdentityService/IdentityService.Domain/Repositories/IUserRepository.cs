using IdentityService.Domain.Entities;

namespace IdentityService.Domain.Repositories;

public interface IUserRepository
{
    Task<AppUser?> GetByEmailAsync(string email, long tenantId, CancellationToken cancellationToken = default);

    Task<AppUser?> GetByIdAsync(long id, CancellationToken cancellationToken = default);

    Task<AppUser?> GetByIdForUpdateAsync(long id, CancellationToken cancellationToken = default);

    Task<bool> EmailExistsAsync(string email, long tenantId, long? excludeUserId, CancellationToken cancellationToken = default);

    void Add(AppUser user);

    void Update(AppUser user);

    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}
