using System.Net;
using System.Text;
using FluentAssertions;
using Healthcare.Common.MultiTenancy;
using HMSService.Application.Integration;
using HMSService.Infrastructure.Integration;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging.Abstractions;
using Moq;
using Xunit;

namespace HMSService.Tests.Services;

public sealed class LmsTestBookingClientTests
{
    private sealed class StubHandler : HttpMessageHandler
    {
        protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            const string json = """
                {"success":true,"message":"Created.","data":{"id":42,"facilityId":7,"bookingNo":"LAB-1","patientId":99,"visitId":null}}
                """;
            return Task.FromResult(new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StringContent(json, Encoding.UTF8, "application/json")
            });
        }
    }

    [Fact]
    public async Task CreateBookingAsync_Returns_Data_From_Lms_Envelope()
    {
        var http = new HttpClient(new StubHandler()) { BaseAddress = new Uri("http://lms/") };
        var accessor = new Mock<IHttpContextAccessor>();
        var tenant = new Mock<ITenantContext>();
        tenant.Setup(t => t.TenantId).Returns(1);
        tenant.Setup(t => t.FacilityId).Returns(7L);

        var client = new LmsTestBookingClient(
            http,
            accessor.Object,
            tenant.Object,
            NullLogger<LmsTestBookingClient>.Instance);

        var req = new CreateLmsTestBookingClientRequestDto
        {
            PatientId = 99,
            Items = new[] { new CreateLmsTestBookingClientItemDto { CatalogTestId = 1, WorkflowStatusReferenceValueId = 2 } }
        };

        var result = await client.CreateBookingAsync(req);

        result.Success.Should().BeTrue();
        result.Data!.Id.Should().Be(42);
        result.Data.BookingNo.Should().Be("LAB-1");
        result.Data.PatientId.Should().Be(99);
    }
}
