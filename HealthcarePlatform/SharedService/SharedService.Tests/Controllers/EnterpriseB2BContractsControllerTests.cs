using FluentAssertions;
using Healthcare.Common.Responses;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using SharedService.API.Controllers.v1;
using SharedService.Application.DTOs.Enterprise;
using SharedService.Application.Services.Enterprise;
using Xunit;

namespace SharedService.Tests.Controllers;

public sealed class EnterpriseB2BContractsControllerTests
{
    private readonly Mock<IEnterpriseB2BContractService> _service = new();

    [Fact]
    public async Task List_ReturnsOk_When_Service_Succeeds()
    {
        var dto = new EnterpriseB2BContractResponseDto { Id = 1, ContractCode = "C1", EnterpriseId = 5 };
        _service
            .Setup(s => s.ListByEnterpriseAsync(5, It.IsAny<CancellationToken>()))
            .ReturnsAsync(BaseResponse<IReadOnlyList<EnterpriseB2BContractResponseDto>>.Ok(new[] { dto }));

        var controller = new EnterpriseB2BContractsController(_service.Object);
        controller.ControllerContext = new ControllerContext { HttpContext = new DefaultHttpContext() };

        var result = await controller.List(5, CancellationToken.None);

        var ok = result.Result.Should().BeOfType<OkObjectResult>().Subject;
        var body = ok.Value.Should().BeOfType<BaseResponse<IReadOnlyList<EnterpriseB2BContractResponseDto>>>().Subject;
        body.Data.Should().HaveCount(1);
    }

    [Fact]
    public async Task GetById_ReturnsNotFound_When_Missing()
    {
        _service
            .Setup(s => s.GetByIdAsync(1, It.IsAny<CancellationToken>()))
            .ReturnsAsync(BaseResponse<EnterpriseB2BContractResponseDto>.Fail("Contract not found."));

        var controller = new EnterpriseB2BContractsController(_service.Object);
        controller.ControllerContext = new ControllerContext { HttpContext = new DefaultHttpContext() };

        var result = await controller.GetById(1, CancellationToken.None);

        result.Result.Should().BeOfType<NotFoundObjectResult>();
    }

    [Fact]
    public async Task Create_ReturnsBadRequest_When_Validation_Fails()
    {
        _service
            .Setup(s => s.CreateAsync(It.IsAny<CreateEnterpriseB2BContractDto>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(BaseResponse<EnterpriseB2BContractResponseDto>.Fail("Invalid."));

        var controller = new EnterpriseB2BContractsController(_service.Object);
        controller.ControllerContext = new ControllerContext { HttpContext = new DefaultHttpContext() };

        var result = await controller.Create(
            new CreateEnterpriseB2BContractDto
            {
                EnterpriseId = 1,
                PartnerType = "X",
                PartnerName = "Y",
                ContractCode = "Z"
            },
            CancellationToken.None);

        result.Result.Should().BeOfType<BadRequestObjectResult>();
    }

    [Fact]
    public async Task Delete_ReturnsOk_When_Deleted()
    {
        _service
            .Setup(s => s.DeleteAsync(3, It.IsAny<CancellationToken>()))
            .ReturnsAsync(BaseResponse<object?>.Ok(null, "Deleted."));

        var controller = new EnterpriseB2BContractsController(_service.Object);
        controller.ControllerContext = new ControllerContext { HttpContext = new DefaultHttpContext() };

        var result = await controller.Delete(3, CancellationToken.None);

        var ok = result.Result.Should().BeOfType<OkObjectResult>().Subject;
        var body = ok.Value.Should().BeOfType<BaseResponse<object?>>().Subject;
        body.Success.Should().BeTrue();
    }
}
