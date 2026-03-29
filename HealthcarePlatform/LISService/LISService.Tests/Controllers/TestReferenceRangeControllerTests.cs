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

public sealed class TestReferenceRangeControllerTests
{
    private readonly Mock<ILisTestReferenceRangeService> _service = new();

    private TestReferenceRangeController CreateController() => new(
        _service.Object,
        LisStandardCrudControllerTestTemplate.TenantMock().Object,
        NullLogger<TestReferenceRangeController>.Instance)
    {
        ControllerContext = LisStandardCrudControllerTestTemplate.DefaultContext
    };

    [Fact]
    public async Task GetById_Should_Return_Ok_When_Valid()
    {
        var dto = new TestReferenceRangeResponseDto { Id = 1 };
        _service.Setup(s => s.GetByIdAsync(1, It.IsAny<CancellationToken>()))
            .ReturnsAsync(BaseResponse<TestReferenceRangeResponseDto>.Ok(dto));

        var result = await CreateController().GetById(1, CancellationToken.None);

        LisStandardCrudControllerTestTemplate.AssertOkBaseResponse(result, b =>
        {
            b.Success.Should().BeTrue();
            b.Data!.Id.Should().Be(1);
        });
    }

    [Fact]
    public async Task GetById_Should_Return_Ok_With_Error_BaseResponse_When_Not_Found()
    {
        _service.Setup(s => s.GetByIdAsync(99, It.IsAny<CancellationToken>()))
            .ReturnsAsync(BaseResponse<TestReferenceRangeResponseDto>.Fail("Not found"));

        var result = await CreateController().GetById(99, CancellationToken.None);

        LisStandardCrudControllerTestTemplate.AssertOkBaseResponse(result, b =>
        {
            b.Success.Should().BeFalse();
            b.Message.Should().Be("Not found");
        });
    }

    [Fact]
    public async Task GetPaged_Should_Return_Ok_When_Valid()
    {
        _service.Setup(s => s.GetPagedAsync(It.IsAny<PagedQuery>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(BaseResponse<PagedResponse<TestReferenceRangeResponseDto>>.Ok(new PagedResponse<TestReferenceRangeResponseDto>
            {
                Items = Array.Empty<TestReferenceRangeResponseDto>(),
                Page = 1,
                PageSize = 10,
                TotalCount = 0
            }));

        var result = await CreateController().GetPaged(new PagedQuery { Page = 1, PageSize = 10 }, CancellationToken.None);

        LisStandardCrudControllerTestTemplate.AssertOkPagedResponse(result, b =>
        {
            b.Success.Should().BeTrue();
            b.Data!.TotalCount.Should().Be(0);
        });
    }

    [Fact]
    public async Task Create_Should_Return_Ok_When_Valid()
    {
        var created = new TestReferenceRangeResponseDto { Id = 2 };
        _service.Setup(s => s.CreateAsync(It.IsAny<CreateTestReferenceRangeDto>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(BaseResponse<TestReferenceRangeResponseDto>.Ok(created));

        var result = await CreateController().Create(new CreateTestReferenceRangeDto(), CancellationToken.None);

        LisStandardCrudControllerTestTemplate.AssertOkBaseResponse(result, b => b.Data!.Id.Should().Be(2));
    }

    [Fact]
    public async Task Create_Should_Return_Ok_With_Error_BaseResponse_When_Invalid()
    {
        _service.Setup(s => s.CreateAsync(It.IsAny<CreateTestReferenceRangeDto>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(BaseResponse<TestReferenceRangeResponseDto>.Fail("Invalid", new[] { "PatientId required" }));

        var result = await CreateController().Create(new CreateTestReferenceRangeDto(), CancellationToken.None);

        LisStandardCrudControllerTestTemplate.AssertOkBaseResponse(result, b =>
        {
            b.Success.Should().BeFalse();
            b.Errors.Should().Contain("PatientId required");
        });
    }

    [Fact]
    public async Task Update_Should_Return_Ok_When_Valid()
    {
        var updated = new TestReferenceRangeResponseDto { Id = 3 };
        _service.Setup(s => s.UpdateAsync(3, It.IsAny<UpdateTestReferenceRangeDto>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(BaseResponse<TestReferenceRangeResponseDto>.Ok(updated));

        var result = await CreateController().Update(3, new UpdateTestReferenceRangeDto(), CancellationToken.None);

        LisStandardCrudControllerTestTemplate.AssertOkBaseResponse(result, b => b.Data!.Id.Should().Be(3));
    }

    [Fact]
    public async Task Update_Should_Return_Ok_With_Error_BaseResponse_When_Invalid()
    {
        _service.Setup(s => s.UpdateAsync(3, It.IsAny<UpdateTestReferenceRangeDto>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(BaseResponse<TestReferenceRangeResponseDto>.Fail("Conflict"));

        var result = await CreateController().Update(3, new UpdateTestReferenceRangeDto(), CancellationToken.None);

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
        var body = ok.Value.Should().BeOfType<BaseResponse<object?>>().Subject;
        body.Success.Should().BeTrue();
    }

    [Fact]
    public async Task Delete_Should_Return_Ok_With_Error_BaseResponse_When_Not_Found()
    {
        _service.Setup(s => s.DeleteAsync(4, It.IsAny<CancellationToken>()))
            .ReturnsAsync(BaseResponse<object?>.Fail("Missing"));

        var result = await CreateController().Delete(4, CancellationToken.None);

        var ok = result.Result.Should().BeOfType<OkObjectResult>().Subject;
        var body = ok.Value.Should().BeOfType<BaseResponse<object?>>().Subject;
        body.Success.Should().BeFalse();
    }
}
