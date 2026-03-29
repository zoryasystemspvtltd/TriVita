using IdentityService.Application.Abstractions;
using IdentityService.Domain.Entities.Rbac;
using IdentityService.Infrastructure.Persistence;

namespace IdentityService.Infrastructure.Persistence.Repositories;

public sealed class LoginAuditRepository : ILoginAuditRepository
{
    private readonly IdentityDbContext _db;

    public LoginAuditRepository(IdentityDbContext db)
    {
        _db = db;
    }

    public async Task AppendAsync(IdentityLoginAttempt attempt, CancellationToken cancellationToken = default)
    {
        await _db.IdentityLoginAttempts.AddAsync(attempt, cancellationToken);
    }

    public Task<int> SaveChangesAsync(CancellationToken cancellationToken = default) =>
        _db.SaveChangesAsync(cancellationToken);
}
