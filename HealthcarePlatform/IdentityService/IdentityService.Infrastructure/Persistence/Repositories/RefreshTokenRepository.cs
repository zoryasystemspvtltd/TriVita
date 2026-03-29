using IdentityService.Application.Abstractions;
using IdentityService.Domain.Entities.Rbac;
using IdentityService.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace IdentityService.Infrastructure.Persistence.Repositories;

public sealed class RefreshTokenRepository : IRefreshTokenRepository
{
    private readonly IdentityDbContext _db;

    public RefreshTokenRepository(IdentityDbContext db)
    {
        _db = db;
    }

    public async Task AddAsync(IdentityRefreshToken entity, CancellationToken cancellationToken = default)
    {
        await _db.IdentityRefreshTokens.AddAsync(entity, cancellationToken);
    }

    public Task<IdentityRefreshToken?> FindActiveByHashAsync(string tokenHash, CancellationToken cancellationToken = default) =>
        _db.IdentityRefreshTokens.AsTracking()
            .FirstOrDefaultAsync(
                t => t.TokenHash == tokenHash && !t.IsDeleted && t.RevokedOn == null && t.ExpiresOn > DateTime.UtcNow,
                cancellationToken);

    public async Task RevokeAsync(long tokenId, DateTime revokedOnUtc, long? replacedByTokenId, CancellationToken cancellationToken = default)
    {
        var row = await _db.IdentityRefreshTokens.AsTracking()
            .FirstOrDefaultAsync(t => t.Id == tokenId, cancellationToken);
        if (row is null)
            return;
        row.RevokedOn = revokedOnUtc;
        row.ReplacedByTokenId = replacedByTokenId;
        row.ModifiedOn = revokedOnUtc;
    }

    public Task<int> SaveChangesAsync(CancellationToken cancellationToken = default) =>
        _db.SaveChangesAsync(cancellationToken);
}
