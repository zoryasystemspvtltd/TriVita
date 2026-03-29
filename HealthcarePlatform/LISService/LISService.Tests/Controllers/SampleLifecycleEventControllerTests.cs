using FluentAssertions;
using Healthcare.Common.Pagination;
using Healthcare.Common.Responses;
using LISService.API.Controllers.v1.Entities;
using LISService.Application.DTOs.Entities;
using LISService.Application.Services.Entities;
using LISService.Tests.Support;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging.Abstractions;
using Moq;
using Xunit;

namespace LISService.Tests.Controllers;

public sealed class SampleLifecycleEventControllerTests
{
    private readonly Mock<ILisSampleLifecycleEventService> _service = new();

    private SampleLifecycleEventController CreateController() => new(
        _service.Object,
        LisStandardCrudControllerTestTemplate.TenantMock().Object,
        NullLogger<SampleLifecycleEventController>.Instance)
    {
        ControllerContext = LisStandardCrudControllerTestTemplate.DefaultContext
    };

    [Fact]
    public async Task GetById_Should_Return_Ok_When_Valid()
    {
        var dto = new SampleLifecycleEventResponseDto { Id = 1 };
        _service.Setup(s => s.GetByIdAsync(1, It.IsAny<CancellationToken>()))
            .ReturnsAsync(BaseResponse<SampleLifecycleEventResponseDto>.Ok(dto));

        var result = await CreateController().GetById(1, CancellationToken.None);

        LisStandardCrudControllerTestTemplate.AssertOkBaseResponse(result, b =>
        {
            b.Success.Should().BeTrue();
            b.Data!.Id.Should().Be(1);
        });
    }

    [Fact]
    public async Task GetPaged_Should_Pass_SampleCollectionId_To_Service()
    {
        _service.Setup(s => s.GetPagedAsync(It.IsAny<PagedQuery>(), 42, It.IsAny<CancellationToken>()))
            .ReturnsAsync(BaseResponse<PagedResponse<SampleLifecycleEventResponseDto>>.Ok(
                new PagedResponse<SampleLifecycleEventResponseDto>
                {
                    Items = Array.Empty<SampleLifecycleEventResponseDto>(),
                    Page = 1,
                    PageSize = 10,
                    TotalCount = 0
                }));

        var result = await CreateController().GetPaged(new PagedQuery { Page = 1, PageSize = 10 }, 42, CancellationToken.None);

        LisStandardCrudControllerTestTemplate.AssertOkPagedResponse(result, b => b.Success.Should().BeTrue());
        _service.Verify(s => s.GetPagedAsync(It.IsAny<PagedQuery>(), 42, It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Create_Should_Return_Ok_When_Valid()
    {
        var created = new SampleLifecycleEventResponseDto { Id = 2 };
        _service.Setup(s => s.CreateAsync(It.IsAny<CreateSampleLifecycleEventDto>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(BaseResponse<SampleLifecycleEventResponseDto>.Ok(created));

        var result = await CreateController().Create(new CreateSampleLifecycleEventDto(), CancellationToken.None);

        LisStandardCrudControllerTestTemplate.AssertOkBaseResponse(result, b => b.Data!.Id.Should().Be(2));
    }

    [Fact]
    public async Task Update_Should_Return_Ok_With_Error_BaseResponse_When_Invalid()
    {
        _service.Setup(s => s.UpdateAsync(3, It.IsAny<UpdateSampleLifecycleEventDto>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(BaseResponse<SampleLifecycleEventResponseDto>.Fail("Conflict"));

        var result = await CreateController().Update(3, new UpdateSampleLifecycleEventDto(), CancellationToken.None);

        LisStandardCrudControllerTestTemplate.AssertOkBaseResponse(result, b =>
        {
            b.Success.Should().BeFalse();
            b.Message.Should().Be("Conflict");
        });
    }

    [Fact]
    public async Task Delete_Should_Return_Ok_When_Valid()
    {
        _service.Setup(s => s.DeleteAsync(4, It.IsAny<CancellationToken>()))
            .ReturnsAsync(BaseResponse<object?>.Ok(null));

        var result = await CreateController().Delete(4, CancellationToken.None);

        var ok = result.Result.Should().BeOfType<OkObjectResult>().Subject;
        ok.Value.Should().BeOfType<BaseResponse<object?>>().Subject.Success.Should().BeTrue();
    }
}
