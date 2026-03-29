using IdentityService.Domain.Entities.Rbac;

namespace IdentityService.Application.Abstractions;

public interface IRefreshTokenRepository
{
    Task AddAsync(IdentityRefreshToken entity, CancellationToken cancellationToken = default);

    Task<IdentityRefreshToken?> FindActiveByHashAsync(string tokenHash, CancellationToken cancellationToken = default);

    Task RevokeAsync(long tokenId, DateTime revokedOnUtc, long? replacedByTokenId, CancellationToken cancellationToken = default);

    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}
