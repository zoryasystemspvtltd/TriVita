using CommunicationService.Domain.Entities;
using Healthcare.Common.Entities;

namespace CommunicationService.Infrastructure.Persistence;

internal static class WorkerAuditHelper
{
    public static void ApplyCreate(BaseEntity entity, ComNotificationQueue queue)
    {
        var now = DateTime.UtcNow;
        entity.TenantId = queue.TenantId;
        entity.FacilityId = queue.FacilityId;
        entity.CreatedOn = now;
        entity.ModifiedOn = now;
        entity.CreatedBy = 0;
        entity.ModifiedBy = 0;
        entity.IsActive = true;
        entity.IsDeleted = false;
    }
}
