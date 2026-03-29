using System.Security.Claims;
using System.Text.Encodings.Web;
using Healthcare.Common.Security;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Healthcare.Common.Authentication;

/// <summary>
/// Test authentication scheme for integration tests (claims include tenant_id / facility_id).
/// Enable via <c>IntegrationTest:UseTestAuth=true</c> in configuration.
/// </summary>
public sealed class TestAuthenticationHandler : AuthenticationHandler<AuthenticationSchemeOptions>
{
    public TestAuthenticationHandler(
        IOptionsMonitor<AuthenticationSchemeOptions> options,
        ILoggerFactory logger,
        UrlEncoder encoder)
        : base(options, logger, encoder)
    {
    }

    protected override Task<AuthenticateResult> HandleAuthenticateAsync()
    {
        var claims = new List<Claim>
        {
            new(ClaimTypes.NameIdentifier, "999"),
            new(TriVitaClaimTypes.TenantId, "1"),
            new(TriVitaClaimTypes.FacilityId, "1"),
            new(ClaimTypes.Role, "Admin"),
            new(TriVitaClaimTypes.Permission, TriVitaPermissions.Wildcard),
        };
        var identity = new ClaimsIdentity(claims, Scheme.Name);
        var principal = new ClaimsPrincipal(identity);
        var ticket = new AuthenticationTicket(principal, Scheme.Name);
        return Task.FromResult(AuthenticateResult.Success(ticket));
    }
}
