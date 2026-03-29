using FluentAssertions;
using FluentValidation;
using FluentValidation.Results;
using Healthcare.Common.Security;
using IdentityService.Application.Abstractions;
using IdentityService.Application.DTOs;
using IdentityService.Application.Options;
using IdentityService.Application.Services;
using IdentityService.Domain.Entities;
using IdentityService.Domain.Entities.Rbac;
using IdentityService.Domain.Repositories;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;

namespace IdentityService.Tests;

public sealed class AuthServiceTests
{
    private static AuthService CreateSut(
        Mock<IUserRepository>? users = null,
        Mock<IJwtTokenGenerator>? jwt = null,
        Mock<IRbacQueryService>? rbac = null,
        Mock<IRefreshTokenRepository>? refresh = null,
        Mock<IAccountLockoutRepository>? lockout = null,
        Mock<ILoginAuditRepository>? audit = null,
        Mock<IPasswordResetRepository>? reset = null,
        Mock<IUserProfileReadRepository>? profile = null)
    {
        users ??= new Mock<IUserRepository>();
        jwt ??= new Mock<IJwtTokenGenerator>();
        rbac ??= new Mock<IRbacQueryService>();
        refresh ??= new Mock<IRefreshTokenRepository>();
        lockout ??= new Mock<IAccountLockoutRepository>();
        audit ??= new Mock<ILoginAuditRepository>();
        reset ??= new Mock<IPasswordResetRepository>();
        profile ??= new Mock<IUserProfileReadRepository>();

        var validator = new Mock<IValidator<LoginRequestDto>>();
        validator.Setup(v => v.ValidateAsync(It.IsAny<LoginRequestDto>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new ValidationResult());

        rbac.Setup(r => r.GetForUserAsync(It.IsAny<long>(), It.IsAny<long>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new RbacPrincipalData());

        lockout.Setup(l => l.GetAsync(It.IsAny<long>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((IdentityAccountLockoutState?)null);
        lockout.Setup(l => l.ClearFailuresAsync(It.IsAny<long>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        users.Setup(u => u.SaveChangesAsync(It.IsAny<CancellationToken>())).ReturnsAsync(1);
        refresh.Setup(r => r.AddAsync(It.IsAny<IdentityRefreshToken>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);
        refresh.Setup(r => r.SaveChangesAsync(It.IsAny<CancellationToken>())).ReturnsAsync(1);
        audit.Setup(a => a.AppendAsync(It.IsAny<IdentityLoginAttempt>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        profile.Setup(p => p.IsMfaEnabledAsync(It.IsAny<long>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);

        jwt.Setup(j => j.CreateAccessToken(
                It.IsAny<AppUser>(),
                It.IsAny<IReadOnlyList<string>>(),
                It.IsAny<IReadOnlyList<string>>(),
                It.IsAny<IReadOnlyList<long>>(),
                It.IsAny<int>()))
            .Returns("access-jwt");

        var http = new Mock<IHttpContextAccessor>();
        http.Setup(h => h.HttpContext).Returns(new DefaultHttpContext());

        var env = new Mock<IHostEnvironment>();
        env.Setup(e => e.EnvironmentName).Returns(Environments.Production);

        var security = Options.Create(new SecurityOptions());

        return new AuthService(
            users.Object,
            jwt.Object,
            validator.Object,
            rbac.Object,
            refresh.Object,
            lockout.Object,
            audit.Object,
            reset.Object,
            profile.Object,
            security,
            Mock.Of<ILogger<AuthService>>(),
            http.Object,
            env.Object);
    }

    [Fact]
    public async Task LoginAsync_succeeds_with_valid_credentials_and_returns_refresh_token()
    {
        var hasher = new PasswordHasher<AppUser>();
        var email = "user@test.local";
        var user = new AppUser
        {
            Id = 10,
            Email = email,
            TenantId = 1,
            FacilityId = 2,
            Role = "Admin",
            IsActive = true,
            PasswordHash = hasher.HashPassword(new AppUser { Email = email }, "Secret!1"),
        };

        var users = new Mock<IUserRepository>();
        users.Setup(u => u.GetByEmailAsync(email, 1, It.IsAny<CancellationToken>())).ReturnsAsync(user);
        users.Setup(u => u.SaveChangesAsync(It.IsAny<CancellationToken>())).ReturnsAsync(1);

        var sut = CreateSut(users);

        var result = await sut.LoginAsync(new LoginRequestDto
        {
            TenantId = 1,
            Email = email,
            Password = "Secret!1",
        });

        result.Success.Should().BeTrue();
        result.Data!.AccessToken.Should().Be("access-jwt");
        result.Data.RefreshToken.Should().NotBeNullOrEmpty();
    }

    [Fact]
    public async Task LoginAsync_fails_when_user_missing()
    {
        var users = new Mock<IUserRepository>();
        users.Setup(u => u.GetByEmailAsync(It.IsAny<string>(), It.IsAny<long>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((AppUser?)null);
        users.Setup(u => u.SaveChangesAsync(It.IsAny<CancellationToken>())).ReturnsAsync(1);

        var sut = CreateSut(users);

        var result = await sut.LoginAsync(new LoginRequestDto
        {
            TenantId = 1,
            Email = "nope@test.local",
            Password = "x",
        });

        result.Success.Should().BeFalse();
    }

    [Fact]
    public async Task RefreshTokenAsync_fails_when_token_unknown()
    {
        var refresh = new Mock<IRefreshTokenRepository>();
        refresh.Setup(r => r.FindActiveByHashAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((IdentityRefreshToken?)null);

        var sut = CreateSut(refresh: refresh);

        var result = await sut.RefreshTokenAsync(new RefreshTokenRequestDto { RefreshToken = "bad" });

        result.Success.Should().BeFalse();
    }
}
