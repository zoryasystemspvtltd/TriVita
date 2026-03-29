using CommunicationService.Domain.Entities;
using CommunicationService.Domain.Repositories;
using Microsoft.EntityFrameworkCore;

namespace CommunicationService.Infrastructure.Persistence.Repositories;

public sealed class NotificationLogRepository : INotificationLogRepository
{
    private readonly CommunicationDbContext _db;

    public NotificationLogRepository(CommunicationDbContext db)
    {
        _db = db;
    }

    public async Task<(IReadOnlyList<ComNotificationLog> Items, int TotalCount)> GetPagedAsync(
        long? notificationChannelId,
        int page,
        int pageSize,
        CancellationToken cancellationToken = default)
    {
        var query = _db.NotificationLogs.AsQueryable();

        if (notificationChannelId is { } cid)
            query = query.Where(l => l.NotificationChannelId == cid);

        var total = await query.CountAsync(cancellationToken);
        var items = await query
            .OrderByDescending(l => l.Id)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);

        return (items, total);
    }
}
