namespace IdentityService.Application.Abstractions;

public sealed class RbacPrincipalData
{
    public List<string> RoleCodes { get; } = new();

    public List<string> PermissionCodes { get; } = new();

    public List<long> FacilityGrantIds { get; } = new();
}

public interface IRbacQueryService
{
    Task<RbacPrincipalData> GetForUserAsync(long userId, long tenantId, CancellationToken cancellationToken = default);
}
