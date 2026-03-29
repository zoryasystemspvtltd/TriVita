using AutoMapper;
using FluentAssertions;
using FluentValidation;
using FluentValidation.Results;
using Healthcare.Common.Integration.SharedService;
using Healthcare.Common.MultiTenancy;
using Healthcare.Common.Pagination;
using LMSService.Application.DTOs.Entities;
using LMSService.Application.Mapping;
using LMSService.Application.Services.Entities;
using LMSService.Domain.Entities;
using LMSService.Domain.Repositories;
using Microsoft.Extensions.Logging.Abstractions;
using Moq;
using Xunit;

namespace LMSService.Tests.Services;

public sealed class LmsProcessingStageServiceTests
{
    private readonly Mock<IRepository<LmsProcessingStage>> _repository = new();
    private readonly Mock<ITenantContext> _tenant = new();
    private readonly Mock<IValidator<CreateProcessingStageDto>> _createValidator = new();
    private readonly Mock<IValidator<UpdateProcessingStageDto>> _updateValidator = new();
    private readonly Mock<IFacilityTenantValidator> _facilityValidator = new();
    private readonly IMapper _mapper;

    public LmsProcessingStageServiceTests()
    {
        var cfg = new MapperConfiguration(c =>
        {
            c.AddProfile<LMSMappingProfile>();
            c.AddProfile<LmsGeneratedMappingProfile>();
        });
        _mapper = cfg.CreateMapper();

        _createValidator
            .Setup(v => v.ValidateAsync(It.IsAny<CreateProcessingStageDto>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new ValidationResult());
        _updateValidator
            .Setup(v => v.ValidateAsync(It.IsAny<UpdateProcessingStageDto>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new ValidationResult());

        _tenant.SetupGet(t => t.TenantId).Returns(1);
        _tenant.SetupGet(t => t.UserId).Returns(1);
        _tenant.SetupGet(t => t.FacilityId).Returns(10L);

        _facilityValidator
            .Setup(v => v.GetFacilityContextAsync(It.IsAny<long>(), It.IsAny<long>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new FacilityHierarchyContext
            {
                TenantId = 1,
                FacilityId = 10,
                EnterpriseId = 1,
                CompanyId = 1,
                BusinessUnitId = 1
            });
    }

    private LmsProcessingStageService CreateSut() =>
        new(
            _repository.Object,
            _mapper,
            _tenant.Object,
            _createValidator.Object,
            _updateValidator.Object,
            _facilityValidator.Object,
            NullLogger<LmsProcessingStageService>.Instance);

    [Fact]
    public async Task GetPagedAsync_ReturnsSuccess()
    {
        _repository
            .Setup(r => r.GetPagedByFilterAsync(1, 20, null, It.IsAny<CancellationToken>()))
            .ReturnsAsync((Array.Empty<LmsProcessingStage>(), 0));

        var sut = CreateSut();
        var result = await sut.GetPagedAsync(new PagedQuery { Page = 1, PageSize = 20 });

        result.Success.Should().BeTrue();
    }
}
