using AutoMapper;
using FluentAssertions;
using FluentValidation;
using FluentValidation.Results;
using Healthcare.Common.Integration.SharedService;
using Healthcare.Common.MultiTenancy;
using Healthcare.Common.Pagination;
using HMSService.Application.DTOs.Extended;
using HMSService.Application.Mapping;
using HMSService.Application.Services.Extended;
using HMSService.Domain.Entities;
using HMSService.Domain.Repositories;
using Microsoft.Extensions.Logging.Abstractions;
using Moq;
using Xunit;

namespace HMSService.Tests.Services;

public sealed class VitalsServiceTests
{
    private readonly Mock<IRepository<HmsVital>> _repository = new();
    private readonly Mock<ITenantContext> _tenant = new();
    private readonly Mock<IValidator<CreateVitalDto>> _createValidator = new();
    private readonly Mock<IValidator<UpdateVitalDto>> _updateValidator = new();
    private readonly Mock<IFacilityTenantValidator> _facilityValidator = new();
    private readonly IMapper _mapper;

    public VitalsServiceTests()
    {
        var cfg = new MapperConfiguration(c => c.AddProfile<HmsMappingProfile>());
        _mapper = cfg.CreateMapper();

        _createValidator
            .Setup(v => v.ValidateAsync(It.IsAny<CreateVitalDto>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new ValidationResult());

        _updateValidator
            .Setup(v => v.ValidateAsync(It.IsAny<UpdateVitalDto>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new ValidationResult());

        _tenant.SetupGet(t => t.TenantId).Returns(1);
        _tenant.SetupGet(t => t.UserId).Returns(99);
        _tenant.SetupGet(t => t.FacilityId).Returns(5L);

        _facilityValidator
            .Setup(v => v.GetFacilityContextAsync(It.IsAny<long>(), It.IsAny<long>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new FacilityHierarchyContext
            {
                TenantId = 1,
                FacilityId = 5,
                EnterpriseId = 1,
                CompanyId = 1,
                BusinessUnitId = 1
            });
    }

    private VitalService CreateSut() =>
        new(
            _repository.Object,
            _mapper,
            _tenant.Object,
            _createValidator.Object,
            _updateValidator.Object,
            _facilityValidator.Object,
            NullLogger<VitalService>.Instance);

    [Fact]
    public async Task CreateAsync_WhenFacilityMissing_ReturnsFailure()
    {
        _tenant.SetupGet(t => t.FacilityId).Returns((long?)null);
        var sut = CreateSut();

        var result = await sut.CreateAsync(new CreateVitalDto
        {
            VisitId = 1,
            RecordedOn = DateTime.UtcNow,
            VitalReferenceValueId = 10
        });

        result.Success.Should().BeFalse();
        result.Message.Should().Contain("FacilityId");
        _repository.Verify(r => r.AddAsync(It.IsAny<HmsVital>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task GetPagedAsync_WithVisitFilter_PassesPredicate()
    {
        _repository
            .Setup(r => r.GetPagedByFilterAsync(1, 20, It.IsAny<System.Linq.Expressions.Expression<Func<HmsVital, bool>>?>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((new List<HmsVital>(), 0));

        var sut = CreateSut();
        var q = new PagedQuery { Page = 1, PageSize = 20 };

        var result = await sut.GetPagedAsync(q, visitId: 42L);

        result.Success.Should().BeTrue();
        _repository.Verify(
            r => r.GetPagedByFilterAsync(1, 20, It.IsAny<System.Linq.Expressions.Expression<Func<HmsVital, bool>>?>(), It.IsAny<CancellationToken>()),
            Times.Once);
    }
}
