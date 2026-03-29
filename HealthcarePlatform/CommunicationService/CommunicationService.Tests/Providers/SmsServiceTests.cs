using System.Net;
using System.Text;
using CommunicationService.Application.Models;
using CommunicationService.Application.Options;
using CommunicationService.Infrastructure.Services;
using FluentAssertions;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Options;
using Moq;
using Moq.Protected;
using Xunit;

namespace CommunicationService.Tests.Providers;

public sealed class SmsServiceTests
{
    [Fact]
    public async Task SendAsync_WhenHttpSuccess_ReturnsOk()
    {
        var handler = new Mock<HttpMessageHandler>();
        handler.Protected()
            .Setup<Task<HttpResponseMessage>>(
                "SendAsync",
                ItExpr.IsAny<HttpRequestMessage>(),
                ItExpr.IsAny<CancellationToken>())
            .ReturnsAsync(new HttpResponseMessage
            {
                StatusCode = HttpStatusCode.OK,
                Content = new StringContent("{\"ok\":true}", Encoding.UTF8, "application/json")
            });

        var client = new HttpClient(handler.Object) { BaseAddress = new Uri("https://sms.test/") };
        var factory = new Mock<IHttpClientFactory>();
        factory.Setup(f => f.CreateClient("CommunicationSms")).Returns(client);

        var opts = Options.Create(new CommunicationOptions
        {
            Sms = new SmsApiOptions { BaseUrl = "https://sms.test/", ApiKey = "k", SenderId = "S" }
        });

        var sut = new SmsService(factory.Object, opts, NullLogger<SmsService>.Instance);
        var result = await sut.SendAsync(new SmsMessage { ToPhoneNumber = "+100", Body = "x" });

        result.Success.Should().BeTrue();
    }
}
