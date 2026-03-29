using Microsoft.AspNetCore.Authorization;

namespace Healthcare.Common.Authorization;

public sealed class PermissionRequirement : IAuthorizationRequirement
{
    public PermissionRequirement(string permissionCode)
    {
        PermissionCode = permissionCode ?? throw new ArgumentNullException(nameof(permissionCode));
    }

    public string PermissionCode { get; }
}
