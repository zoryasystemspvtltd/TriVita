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

public sealed class WhatsAppServiceTests
{
    [Fact]
    public async Task SendAsync_WhenHttpFails_ReturnsFailure()
    {
        var handler = new Mock<HttpMessageHandler>();
        handler.Protected()
            .Setup<Task<HttpResponseMessage>>(
                "SendAsync",
                ItExpr.IsAny<HttpRequestMessage>(),
                ItExpr.IsAny<CancellationToken>())
            .ReturnsAsync(new HttpResponseMessage
            {
                StatusCode = HttpStatusCode.BadRequest,
                Content = new StringContent("err", Encoding.UTF8, "text/plain")
            });

        var client = new HttpClient(handler.Object) { BaseAddress = new Uri("https://wa.test/") };
        var factory = new Mock<IHttpClientFactory>();
        factory.Setup(f => f.CreateClient("CommunicationWhatsApp")).Returns(client);

        var opts = Options.Create(new CommunicationOptions
        {
            WhatsApp = new WhatsAppApiOptions { BaseUrl = "https://wa.test/", ApiKey = "k", FromNumber = "1" }
        });

        var sut = new WhatsAppService(factory.Object, opts, NullLogger<WhatsAppService>.Instance);
        var result = await sut.SendAsync(new WhatsAppMessage { ToPhoneNumber = "+1", Body = "m" });

        result.Success.Should().BeFalse();
    }
}
