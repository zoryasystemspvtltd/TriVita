using Healthcare.Common.Entities;
using Healthcare.Common.MultiTenancy;

namespace HMSService.Application.Services;

internal static class AuditHelper
{
    public static void ApplyCreate(BaseEntity entity, ITenantContext tenant)
    {
        var now = DateTime.UtcNow;
        var userId = tenant.UserId ?? 0;

        entity.TenantId = tenant.TenantId;
        entity.CreatedOn = now;
        entity.ModifiedOn = now;
        entity.CreatedBy = userId;
        entity.ModifiedBy = userId;
        entity.IsActive = true;
        entity.IsDeleted = false;
    }

    public static void ApplyUpdate(BaseEntity entity, ITenantContext tenant)
    {
        entity.ModifiedOn = DateTime.UtcNow;
        entity.ModifiedBy = tenant.UserId ?? 0;
    }
}
