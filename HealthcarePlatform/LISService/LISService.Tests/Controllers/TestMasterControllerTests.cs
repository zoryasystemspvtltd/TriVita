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

public sealed class TestMasterControllerTests
{
    private readonly Mock<ILisTestMasterService> _service = new();

    private TestMasterController CreateController() => new(
        _service.Object,
        LisStandardCrudControllerTestTemplate.TenantMock().Object,
        NullLogger<TestMasterController>.Instance)
    {
        ControllerContext = LisStandardCrudControllerTestTemplate.DefaultContext
    };

    [Fact]
    public async Task GetById_Should_Return_Ok_When_Valid()
    {
        var dto = new TestMasterResponseDto { Id = 1 };
        _service.Setup(s => s.GetByIdAsync(1, It.IsAny<CancellationToken>()))
            .ReturnsAsync(BaseResponse<TestMasterResponseDto>.Ok(dto));

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
            .ReturnsAsync(BaseResponse<TestMasterResponseDto>.Fail("Not found"));

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
            .ReturnsAsync(BaseResponse<PagedResponse<TestMasterResponseDto>>.Ok(new PagedResponse<TestMasterResponseDto>
            {
                Items = Array.Empty<TestMasterResponseDto>(),
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
        var created = new TestMasterResponseDto { Id = 2 };
        _service.Setup(s => s.CreateAsync(It.IsAny<CreateTestMasterDto>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(BaseResponse<TestMasterResponseDto>.Ok(created));

        var result = await CreateController().Create(new CreateTestMasterDto(), CancellationToken.None);

        LisStandardCrudControllerTestTemplate.AssertOkBaseResponse(result, b => b.Data!.Id.Should().Be(2));
    }

    [Fact]
    public async Task Create_Should_Return_Ok_With_Error_BaseResponse_When_Invalid()
    {
        _service.Setup(s => s.CreateAsync(It.IsAny<CreateTestMasterDto>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(BaseResponse<TestMasterResponseDto>.Fail("Invalid", new[] { "PatientId required" }));

        var result = await CreateController().Create(new CreateTestMasterDto(), CancellationToken.None);

        LisStandardCrudControllerTestTemplate.AssertOkBaseResponse(result, b =>
        {
            b.Success.Should().BeFalse();
            b.Errors.Should().Contain("PatientId required");
        });
    }

    [Fact]
    public async Task Update_Should_Return_Ok_When_Valid()
    {
        var updated = new TestMasterResponseDto { Id = 3 };
        _service.Setup(s => s.UpdateAsync(3, It.IsAny<UpdateTestMasterDto>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(BaseResponse<TestMasterResponseDto>.Ok(updated));

        var result = await CreateController().Update(3, new UpdateTestMasterDto(), CancellationToken.None);

        LisStandardCrudControllerTestTemplate.AssertOkBaseResponse(result, b => b.Data!.Id.Should().Be(3));
    }

    [Fact]
    public async Task Update_Should_Return_Ok_With_Error_BaseResponse_When_Invalid()
    {
        _service.Setup(s => s.UpdateAsync(3, It.IsAny<UpdateTestMasterDto>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(BaseResponse<TestMasterResponseDto>.Fail("Conflict"));

        var result = await CreateController().Update(3, new UpdateTestMasterDto(), CancellationToken.None);

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
