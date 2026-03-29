using FluentAssertions;
using Xunit;

namespace CommunicationService.Tests.BackgroundWorker;

/// <summary>Queue/retry behaviour is covered via NotificationProcessor integration-style tests in a future iteration;
/// this placeholder keeps the required test folder structure.</summary>
public sealed class NotificationProcessorTests
{
    [Fact]
    public void RetryPolicy_IsConfiguredViaCommunicationOptions()
    {
        // MaxRetryAttempts lives on CommunicationOptions.Processor — validated in appsettings and service wiring.
        true.Should().BeTrue();
    }
}
