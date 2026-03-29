using CommunicationService.Application.Models;
using CommunicationService.Application.Options;
using CommunicationService.Infrastructure.Services;
using FluentAssertions;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Options;
using Xunit;

namespace CommunicationService.Tests.Providers;

public sealed class EmailServiceTests
{
    [Fact]
    public async Task SendAsync_WhenNoRecipients_ReturnsFailure()
    {
        var opts = Options.Create(new CommunicationOptions());
        var sut = new EmailService(opts, NullLogger<EmailService>.Instance);

        var result = await sut.SendAsync(new EmailMessage { ToAddresses = Array.Empty<string>() });

        result.Success.Should().BeFalse();
        result.Error.Should().Contain("recipients");
    }
}
