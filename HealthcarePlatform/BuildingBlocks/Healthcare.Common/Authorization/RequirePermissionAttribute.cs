using Microsoft.AspNetCore.Authorization;

namespace Healthcare.Common.Authorization;

/// <summary>Requires a permission claim (or Admin role / wildcard permission).</summary>
public sealed class RequirePermissionAttribute : AuthorizeAttribute
{
    public RequirePermissionAttribute(string permissionCode)
    {
        Policy = $"Permission:{permissionCode}";
    }
}
