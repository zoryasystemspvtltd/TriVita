using Healthcare.Common.Entities;
using Healthcare.Common.MultiTenancy;

namespace CommunicationService.Application.Services;

internal static class AuditHelper
{
    public static void ApplyCreate(BaseEntity entity, ITenantContext tenant, long userId = 0)
    {
        var uid = tenant.UserId ?? userId;
        var now = DateTime.UtcNow;
        entity.TenantId = tenant.TenantId;
        entity.CreatedOn = now;
        entity.ModifiedOn = now;
        entity.CreatedBy = uid;
        entity.ModifiedBy = uid;
        entity.IsActive = true;
        entity.IsDeleted = false;
    }
}
