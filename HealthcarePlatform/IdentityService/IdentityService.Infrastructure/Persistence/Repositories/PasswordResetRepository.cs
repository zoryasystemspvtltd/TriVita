using IdentityService.Application.Abstractions;
using IdentityService.Domain.Entities.Rbac;
using IdentityService.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace IdentityService.Infrastructure.Persistence.Repositories;

public sealed class PasswordResetRepository : IPasswordResetRepository
{
    private readonly IdentityDbContext _db;

    public PasswordResetRepository(IdentityDbContext db)
    {
        _db = db;
    }

    public async Task AddAsync(IdentityPasswordResetToken entity, CancellationToken cancellationToken = default)
    {
        await _db.IdentityPasswordResetTokens.AddAsync(entity, cancellationToken);
    }

    public Task<IdentityPasswordResetToken?> FindActiveByUserAndHashAsync(
        long userId,
        string tokenHash,
        CancellationToken cancellationToken = default) =>
        _db.IdentityPasswordResetTokens.AsTracking()
            .FirstOrDefaultAsync(
                t => t.UserId == userId
                     && t.TokenHash == tokenHash
                     && !t.IsDeleted
                     && t.ConsumedOn == null
                     && t.ExpiresOn > DateTime.UtcNow,
                cancellationToken);

    public Task<int> SaveChangesAsync(CancellationToken cancellationToken = default) =>
        _db.SaveChangesAsync(cancellationToken);
}
