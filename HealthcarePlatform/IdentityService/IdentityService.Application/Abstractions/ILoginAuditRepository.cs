using IdentityService.Domain.Entities.Rbac;

namespace IdentityService.Application.Abstractions;

public interface ILoginAuditRepository
{
    Task AppendAsync(IdentityLoginAttempt attempt, CancellationToken cancellationToken = default);

    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}
