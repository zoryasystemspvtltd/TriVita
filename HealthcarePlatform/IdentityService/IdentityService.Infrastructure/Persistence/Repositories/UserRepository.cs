using IdentityService.Domain.Entities;
using IdentityService.Domain.Repositories;
using Microsoft.EntityFrameworkCore;

namespace IdentityService.Infrastructure.Persistence.Repositories;

public sealed class UserRepository : IUserRepository
{
    private readonly IdentityDbContext _db;

    public UserRepository(IdentityDbContext db)
    {
        _db = db;
    }

    public async Task<AppUser?> GetByEmailAsync(string email, long tenantId, CancellationToken cancellationToken = default)
    {
        var normalized = email.Trim().ToLowerInvariant();
        return await _db.Users.AsNoTracking()
            .FirstOrDefaultAsync(u => u.Email == normalized && u.TenantId == tenantId, cancellationToken);
    }

    public async Task<AppUser?> GetByIdAsync(long id, CancellationToken cancellationToken = default) =>
        await _db.Users.AsNoTracking().FirstOrDefaultAsync(u => u.Id == id, cancellationToken);

    public async Task<AppUser?> GetByIdForUpdateAsync(long id, CancellationToken cancellationToken = default) =>
        await _db.Users.AsTracking().FirstOrDefaultAsync(u => u.Id == id, cancellationToken);

    public async Task<bool> EmailExistsAsync(
        string email,
        long tenantId,
        long? excludeUserId,
        CancellationToken cancellationToken = default)
    {
        var normalized = email.Trim().ToLowerInvariant();
        var q = _db.Users.Where(u => u.Email == normalized && u.TenantId == tenantId);
        if (excludeUserId is not null)
            q = q.Where(u => u.Id != excludeUserId.Value);
        return await q.AnyAsync(cancellationToken);
    }

    public void Add(AppUser user) => _db.Users.Add(user);

    public void Update(AppUser user) => _db.Users.Update(user);

    public Task<int> SaveChangesAsync(CancellationToken cancellationToken = default) =>
        _db.SaveChangesAsync(cancellationToken);
}
