using System.Linq.Expressions;
using AutoMapper;
using FluentAssertions;
using FluentValidation;
using FluentValidation.Results;
using Healthcare.Common.Integration.SharedService;
using Healthcare.Common.MultiTenancy;
using Healthcare.Common.Pagination;
using LISService.Application.DTOs.Entities;
using LISService.Application.Mapping;
using LISService.Application.Services.Entities;
using LISService.Domain.Entities;
using LISService.Domain.Repositories;
using Microsoft.Extensions.Logging.Abstractions;
using Moq;
using Xunit;

namespace LISService.Tests.Services;

public sealed class LisEnhancementServicesTests
{
    private readonly Mock<ITenantContext> _tenant = new();
    private readonly Mock<IFacilityTenantValidator> _facilityValidator = new();
    private readonly IMapper _mapper;

    public LisEnhancementServicesTests()
    {
        var cfg = new MapperConfiguration(c =>
        {
            c.AddProfile<LISMappingProfile>();
            c.AddProfile<LisGeneratedMappingProfile>();
        });
        _mapper = cfg.CreateMapper();
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

    [Fact]
    public async Task LisAnalyzerResultMapService_GetPaged_ReturnsSuccess()
    {
        var repo = new Mock<IRepository<LisAnalyzerResultMap>>();
        repo.Setup(r => r.GetPagedByFilterAsync(1, 20, null, It.IsAny<CancellationToken>()))
            .ReturnsAsync((Array.Empty<LisAnalyzerResultMap>(), 0));
        var v1 = new Mock<IValidator<CreateAnalyzerResultMapDto>>();
        var v2 = new Mock<IValidator<UpdateAnalyzerResultMapDto>>();
        v1.Setup(v => v.ValidateAsync(It.IsAny<CreateAnalyzerResultMapDto>(), It.IsAny<CancellationToken>())).ReturnsAsync(new ValidationResult());
        v2.Setup(v => v.ValidateAsync(It.IsAny<UpdateAnalyzerResultMapDto>(), It.IsAny<CancellationToken>())).ReturnsAsync(new ValidationResult());

        var sut = new LisAnalyzerResultMapService(repo.Object, _mapper, _tenant.Object, v1.Object, v2.Object, _facilityValidator.Object, NullLogger<LisAnalyzerResultMapService>.Instance);
        var result = await sut.GetPagedAsync(new PagedQuery { Page = 1, PageSize = 20 });
        result.Success.Should().BeTrue();
    }

    [Fact]
    public async Task LisTestParameterProfileService_Create_Catalog_DoesNotRequireFacility()
    {
        _tenant.SetupGet(t => t.FacilityId).Returns((long?)null);
        var repo = new Mock<IRepository<LisTestParameterProfile>>();
        repo.Setup(r => r.AddAsync(It.IsAny<LisTestParameterProfile>(), It.IsAny<CancellationToken>())).Returns(Task.CompletedTask);
        repo.Setup(r => r.SaveChangesAsync(It.IsAny<CancellationToken>())).ReturnsAsync(1);
        var v1 = new Mock<IValidator<CreateTestParameterProfileDto>>();
        var v2 = new Mock<IValidator<UpdateTestParameterProfileDto>>();
        v1.Setup(v => v.ValidateAsync(It.IsAny<CreateTestParameterProfileDto>(), It.IsAny<CancellationToken>())).ReturnsAsync(new ValidationResult());

        var sut = new LisTestParameterProfileService(repo.Object, _mapper, _tenant.Object, v1.Object, v2.Object, _facilityValidator.Object, NullLogger<LisTestParameterProfileService>.Instance);
        var result = await sut.CreateAsync(new CreateTestParameterProfileDto { TestParameterId = 1 });
        result.Success.Should().BeTrue();
    }

    [Fact]
    public async Task LisSampleLifecycleEventService_GetPaged_WithFilter_UsesRepository()
    {
        var repo = new Mock<IRepository<LisSampleLifecycleEvent>>();
        repo.Setup(r => r.GetPagedByFilterAsync(1, 10, It.IsAny<Expression<Func<LisSampleLifecycleEvent, bool>>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((Array.Empty<LisSampleLifecycleEvent>(), 0));
        var v1 = new Mock<IValidator<CreateSampleLifecycleEventDto>>();
        var v2 = new Mock<IValidator<UpdateSampleLifecycleEventDto>>();
        v1.Setup(v => v.ValidateAsync(It.IsAny<CreateSampleLifecycleEventDto>(), It.IsAny<CancellationToken>())).ReturnsAsync(new ValidationResult());
        v2.Setup(v => v.ValidateAsync(It.IsAny<UpdateSampleLifecycleEventDto>(), It.IsAny<CancellationToken>())).ReturnsAsync(new ValidationResult());

        var sut = new LisSampleLifecycleEventService(repo.Object, _mapper, _tenant.Object, v1.Object, v2.Object, _facilityValidator.Object, NullLogger<LisSampleLifecycleEventService>.Instance);
        var result = await sut.GetPagedAsync(new PagedQuery { Page = 1, PageSize = 10 }, sampleCollectionId: 99);
        result.Success.Should().BeTrue();
        repo.Verify(r => r.GetPagedByFilterAsync(1, 10, It.IsAny<Expression<Func<LisSampleLifecycleEvent, bool>>>(), It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task LisReportLockStateService_GetById_NotFound_Fails()
    {
        var repo = new Mock<IRepository<LisReportLockState>>();
        repo.Setup(r => r.GetByIdAsync(1, It.IsAny<CancellationToken>())).ReturnsAsync((LisReportLockState?)null);
        var v1 = new Mock<IValidator<CreateReportLockStateDto>>();
        var v2 = new Mock<IValidator<UpdateReportLockStateDto>>();

        var sut = new LisReportLockStateService(repo.Object, _mapper, _tenant.Object, v1.Object, v2.Object, _facilityValidator.Object, NullLogger<LisReportLockStateService>.Instance);
        var result = await sut.GetByIdAsync(1);
        result.Success.Should().BeFalse();
    }
}
