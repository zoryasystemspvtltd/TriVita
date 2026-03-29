using IdentityService.Application.Abstractions;
using IdentityService.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace IdentityService.Infrastructure.Persistence.Repositories;

public sealed class UserProfileReadRepository : IUserProfileReadRepository
{
    private readonly IdentityDbContext _db;

    public UserProfileReadRepository(IdentityDbContext db)
    {
        _db = db;
    }

    public async Task<bool> IsMfaEnabledAsync(long userId, CancellationToken cancellationToken = default)
    {
        var profile = await _db.IdentityUserProfiles.AsNoTracking()
            .FirstOrDefaultAsync(p => p.UserId == userId && !p.IsDeleted, cancellationToken);
        return profile?.MfaEnabled == true;
    }
}
