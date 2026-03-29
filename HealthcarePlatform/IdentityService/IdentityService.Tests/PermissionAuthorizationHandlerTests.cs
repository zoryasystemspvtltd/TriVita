using System.Security.Claims;
using FluentAssertions;
using Healthcare.Common.Authorization;
using Healthcare.Common.Security;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Logging;
using Moq;

namespace IdentityService.Tests;

public sealed class PermissionAuthorizationHandlerTests
{
    [Fact]
    public async Task Succeeds_when_permission_claim_present()
    {
        var handler = new PermissionAuthorizationHandler(Mock.Of<ILogger<PermissionAuthorizationHandler>>());
        var requirement = new PermissionRequirement(TriVitaPermissions.HmsApi);
        var identity = new ClaimsIdentity(
            new[]
            {
                new Claim(ClaimTypes.NameIdentifier, "1"),
                new Claim(TriVitaClaimTypes.Permission, TriVitaPermissions.HmsApi),
            },
            "test");
        var ctx = new AuthorizationHandlerContext(
            new IAuthorizationRequirement[] { requirement },
            new ClaimsPrincipal(identity),
            resource: null);

        await handler.HandleAsync(ctx);

        ctx.HasSucceeded.Should().BeTrue();
    }

    [Fact]
    public async Task Succeeds_for_Admin_role_without_explicit_permission()
    {
        var handler = new PermissionAuthorizationHandler(Mock.Of<ILogger<PermissionAuthorizationHandler>>());
        var requirement = new PermissionRequirement(TriVitaPermissions.LmsApi);
        var identity = new ClaimsIdentity(
            new[]
            {
                new Claim(ClaimTypes.NameIdentifier, "1"),
                new Claim(ClaimTypes.Role, "Admin"),
            },
            "test");
        var ctx = new AuthorizationHandlerContext(
            new IAuthorizationRequirement[] { requirement },
            new ClaimsPrincipal(identity),
            resource: null);

        await handler.HandleAsync(ctx);

        ctx.HasSucceeded.Should().BeTrue();
    }

    [Fact]
    public async Task Fails_when_permission_missing()
    {
        var handler = new PermissionAuthorizationHandler(Mock.Of<ILogger<PermissionAuthorizationHandler>>());
        var requirement = new PermissionRequirement(TriVitaPermissions.PharmacyApi);
        var identity = new ClaimsIdentity(
            new[] { new Claim(ClaimTypes.NameIdentifier, "1") },
            "test");
        var ctx = new AuthorizationHandlerContext(
            new IAuthorizationRequirement[] { requirement },
            new ClaimsPrincipal(identity),
            resource: null);

        await handler.HandleAsync(ctx);

        ctx.HasSucceeded.Should().BeFalse();
    }
}
