using CommunicationService.Domain.Entities;
using CommunicationService.Domain.Repositories;
using Microsoft.EntityFrameworkCore;

namespace CommunicationService.Infrastructure.Persistence.Repositories;

public sealed class NotificationRepository : INotificationRepository
{
    private readonly CommunicationDbContext _db;

    public NotificationRepository(CommunicationDbContext db)
    {
        _db = db;
    }

    public async Task<ComNotification?> GetByIdWithDetailsAsync(long id, CancellationToken cancellationToken = default)
    {
        return await _db.Notifications
            .Include(n => n.Recipients)
            .Include(n => n.Channels).ThenInclude(c => c.Template)
            .Include(n => n.Queues)
            .FirstOrDefaultAsync(n => n.Id == id, cancellationToken);
    }

    public async Task<ComNotification?> GetByIdForProcessingAsync(
        long tenantId,
        long facilityId,
        long id,
        CancellationToken cancellationToken = default)
    {
        return await _db.Notifications
            .IgnoreQueryFilters()
            .Where(n => n.TenantId == tenantId && n.FacilityId == facilityId && n.Id == id && !n.IsDeleted)
            .Include(n => n.Recipients)
            .Include(n => n.Channels).ThenInclude(c => c.Template)
            .Include(n => n.Queues)
            .FirstOrDefaultAsync(cancellationToken);
    }

    public async Task AddAsync(ComNotification entity, CancellationToken cancellationToken = default)
    {
        await _db.Notifications.AddAsync(entity, cancellationToken);
    }

    public async Task AddNotificationLogAsync(ComNotificationLog log, CancellationToken cancellationToken = default)
    {
        await _db.NotificationLogs.AddAsync(log, cancellationToken);
    }

    public Task SaveChangesAsync(CancellationToken cancellationToken = default) =>
        _db.SaveChangesAsync(cancellationToken);
}
