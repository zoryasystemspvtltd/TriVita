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

public sealed class TestCategoryControllerTests
{
    private readonly Mock<ILisTestCategoryService> _service = new();

    private TestCategoryController CreateController() => new(
        _service.Object,
        LisStandardCrudControllerTestTemplate.TenantMock().Object,
        NullLogger<TestCategoryController>.Instance)
    {
        ControllerContext = LisStandardCrudControllerTestTemplate.DefaultContext
    };

    [Fact]
    public async Task GetById_Should_Return_Ok_When_Valid()
    {
        _service
            .Setup(s => s.GetByIdAsync(1, It.IsAny<CancellationToken>()))
            .ReturnsAsync(BaseResponse<TestCategoryResponseDto>.Ok(new TestCategoryResponseDto
            {
                Id = 1,
                TenantId = 1,
                CategoryCode = "LAB",
                CategoryName = "Laboratory"
            }));

        var result = await CreateController().GetById(1, CancellationToken.None);

        LisStandardCrudControllerTestTemplate.AssertOkBaseResponse(result, b =>
        {
            b.Success.Should().BeTrue();
            b.Data!.CategoryCode.Should().Be("LAB");
        });
    }

    [Fact]
    public async Task GetById_Should_Return_Ok_With_Error_BaseResponse_When_Not_Found()
    {
        _service.Setup(s => s.GetByIdAsync(99, It.IsAny<CancellationToken>()))
            .ReturnsAsync(BaseResponse<TestCategoryResponseDto>.Fail("Not found"));

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
            .ReturnsAsync(BaseResponse<PagedResponse<TestCategoryResponseDto>>.Ok(new PagedResponse<TestCategoryResponseDto>
            {
                Items = Array.Empty<TestCategoryResponseDto>(),
                Page = 1,
                PageSize = 10,
                TotalCount = 0
            }));

        var result = await CreateController().GetPaged(new PagedQuery { Page = 1, PageSize = 10 }, CancellationToken.None);

        LisStandardCrudControllerTestTemplate.AssertOkPagedResponse(result, b => b.Success.Should().BeTrue());
    }

    [Fact]
    public async Task Create_Should_Return_Ok_When_Valid()
    {
        var created = new TestCategoryResponseDto { Id = 2, CategoryCode = "HEM" };
        _service.Setup(s => s.CreateAsync(It.IsAny<CreateTestCategoryDto>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(BaseResponse<TestCategoryResponseDto>.Ok(created));

        var result = await CreateController().Create(new CreateTestCategoryDto(), CancellationToken.None);

        LisStandardCrudControllerTestTemplate.AssertOkBaseResponse(result, b => b.Data!.CategoryCode.Should().Be("HEM"));
    }

    [Fact]
    public async Task Update_Should_Return_Ok_With_Error_BaseResponse_When_Invalid()
    {
        _service.Setup(s => s.UpdateAsync(3, It.IsAny<UpdateTestCategoryDto>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(BaseResponse<TestCategoryResponseDto>.Fail("Conflict"));

        var result = await CreateController().Update(3, new UpdateTestCategoryDto(), CancellationToken.None);

        LisStandardCrudControllerTestTemplate.AssertOkBaseResponse(result, b => b.Success.Should().BeFalse());
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
