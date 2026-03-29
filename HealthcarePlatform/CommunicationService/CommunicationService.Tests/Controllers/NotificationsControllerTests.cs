using CommunicationService.API.Controllers.v1;
using CommunicationService.Application.Abstractions;
using CommunicationService.Contracts.Notifications;
using FluentAssertions;
using Healthcare.Common.Pagination;
using Healthcare.Common.Responses;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Xunit;

namespace CommunicationService.Tests.Controllers;

public sealed class NotificationsControllerTests
{
    private readonly Mock<INotificationService> _service = new();

    [Fact]
    public async Task GetById_ReturnsOk_WithBaseResponse()
    {
        var dto = new NotificationResponseDto { Id = 1, EventType = "E" };
        _service.Setup(s => s.GetByIdAsync(1, It.IsAny<CancellationToken>()))
            .ReturnsAsync(BaseResponse<NotificationResponseDto>.Ok(dto));

        var controller = new NotificationsController(_service.Object);
        var result = await controller.GetById(1, CancellationToken.None);

        var ok = result.Result.Should().BeOfType<OkObjectResult>().Subject;
        var body = ok.Value.Should().BeOfType<BaseResponse<NotificationResponseDto>>().Subject;
        body.Success.Should().BeTrue();
        body.Data!.Id.Should().Be(1);
    }

    [Fact]
    public async Task GetLogs_ReturnsPagedStructure()
    {
        var paged = new PagedResponse<NotificationLogResponseDto>
        {
            Items = Array.Empty<NotificationLogResponseDto>(),
            Page = 1,
            PageSize = 20,
            TotalCount = 0
        };
        _service.Setup(s => s.GetLogsPagedAsync(null, It.IsAny<PagedQuery>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(BaseResponse<PagedResponse<NotificationLogResponseDto>>.Ok(paged));

        var controller = new NotificationsController(_service.Object);
        var result = await controller.GetLogs(new PagedQuery(), null, CancellationToken.None);

        var ok = result.Result.Should().BeOfType<OkObjectResult>().Subject;
        var body = ok.Value.Should().BeOfType<BaseResponse<PagedResponse<NotificationLogResponseDto>>>().Subject;
        body.Data!.TotalCount.Should().Be(0);
    }
}
