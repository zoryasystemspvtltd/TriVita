using FluentAssertions;
using Healthcare.Common.MultiTenancy;
using Healthcare.Common.Pagination;
using Healthcare.Common.Responses;
using HMSService.API.Controllers.v1;
using HMSService.Application.Integration;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging.Abstractions;
using Moq;
using Xunit;

namespace HMSService.Tests.Controllers;

public sealed class LabBillingIntegrationControllerTests
{
    [Fact]
    public async Task GetLabInvoices_ReturnsOk_FromClient()
    {
        var client = new Mock<ILmsBillingClient>();
        var tenant = new Mock<ITenantContext>();
        tenant.SetupGet(t => t.TenantId).Returns(1);
        tenant.SetupGet(t => t.FacilityId).Returns(2L);

        var paged = new PagedResponse<LabInvoiceSummaryDto>
        {
            Items = Array.Empty<LabInvoiceSummaryDto>(),
            Page = 1,
            PageSize = 20,
            TotalCount = 0
        };
        client.Setup(c => c.GetLabInvoicesPagedAsync(It.IsAny<PagedQuery>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(BaseResponse<PagedResponse<LabInvoiceSummaryDto>>.Ok(paged));

        var sut = new LabBillingIntegrationController(
            client.Object,
            tenant.Object,
            NullLogger<LabBillingIntegrationController>.Instance);

        var result = await sut.GetLabInvoices(new PagedQuery { Page = 1, PageSize = 20 }, CancellationToken.None);

        var ok = result.Result.Should().BeOfType<OkObjectResult>().Subject;
        var body = ok.Value.Should().BeOfType<BaseResponse<PagedResponse<LabInvoiceSummaryDto>>>().Subject;
        body.Success.Should().BeTrue();
    }
}
