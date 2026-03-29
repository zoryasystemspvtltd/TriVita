using System.Security.Claims;
using Healthcare.Common.Security;
using Microsoft.AspNetCore.Http;

namespace Healthcare.Common.MultiTenancy;

public sealed class HttpTenantContext : ITenantContext
{
    private const string HeaderTenant = "X-Tenant-Id";
    private const string HeaderFacility = "X-Facility-Id";

    public HttpTenantContext(IHttpContextAccessor httpContextAccessor)
    {
        var http = httpContextAccessor.HttpContext
                   ?? throw new InvalidOperationException("HttpContext is not available.");

        var user = http.User;

        UserId = TryParseLong(user.FindFirstValue(ClaimTypes.NameIdentifier) ?? user.FindFirstValue("sub"));
        TenantId = ReadLongClaim(user, "tenant_id")
                   ?? ReadLongHeader(http, HeaderTenant)
                   ?? 0;

        FacilityId = ReadLongClaim(user, "facility_id") ?? ReadLongHeader(http, HeaderFacility);

        Roles = user.FindAll(ClaimTypes.Role).Select(c => c.Value).ToList();
        Permissions = user.FindAll(TriVitaClaimTypes.Permission).Select(c => c.Value).ToList();
    }

    public long TenantId { get; }

    public long? FacilityId { get; }

    public long? UserId { get; }

    public IReadOnlyList<string> Roles { get; }

    public IReadOnlyList<string> Permissions { get; }

    private static long? ReadLongClaim(ClaimsPrincipal user, string type)
    {
        var v = user.FindFirstValue(type);
        return TryParseLong(v);
    }

    private static long? ReadLongHeader(HttpContext http, string name)
    {
        if (!http.Request.Headers.TryGetValue(name, out var values))
            return null;
        return TryParseLong(values.FirstOrDefault());
    }

    private static long? TryParseLong(string? s) =>
        long.TryParse(s, out var v) ? v : null;
}
