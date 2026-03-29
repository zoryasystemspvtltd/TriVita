using System.Security.Claims;
using Healthcare.Common.Security;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Logging;

namespace Healthcare.Common.Authorization;

public sealed class PermissionAuthorizationHandler : AuthorizationHandler<PermissionRequirement>
{
    private readonly ILogger<PermissionAuthorizationHandler> _logger;

    public PermissionAuthorizationHandler(ILogger<PermissionAuthorizationHandler> logger)
    {
        _logger = logger;
    }

    protected override Task HandleRequirementAsync(
        AuthorizationHandlerContext context,
        PermissionRequirement requirement)
    {
        if (context.User.Identity?.IsAuthenticated != true)
            return Task.CompletedTask;

        if (context.User.IsInRole("Admin"))
        {
            context.Succeed(requirement);
            return Task.CompletedTask;
        }

        if (HasWildcard(context.User))
        {
            context.Succeed(requirement);
            return Task.CompletedTask;
        }

        if (HasPermission(context.User, requirement.PermissionCode))
        {
            context.Succeed(requirement);
            return Task.CompletedTask;
        }

        _logger.LogWarning(
            "Permission denied for user {Sub}: missing {Permission}",
            context.User.FindFirstValue(ClaimTypes.NameIdentifier) ?? context.User.FindFirstValue("sub"),
            requirement.PermissionCode);

        return Task.CompletedTask;
    }

    private static bool HasWildcard(ClaimsPrincipal user) =>
        user.HasClaim(TriVitaClaimTypes.Permission, TriVitaPermissions.Wildcard);

    private static bool HasPermission(ClaimsPrincipal user, string code) =>
        user.HasClaim(TriVitaClaimTypes.Permission, code);
}
