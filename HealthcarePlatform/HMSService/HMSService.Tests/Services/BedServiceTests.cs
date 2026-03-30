using AutoMapper;
using FluentAssertions;
using FluentValidation;
using FluentValidation.Results;
using Healthcare.Common.Integration.SharedService;
using Healthcare.Common.MultiTenancy;
using Healthcare.Common.Pagination;
using HMSService.Application.DTOs.Gap;
using HMSService.Application.Mapping;
using HMSService.Application.Services.Gap;
using HMSService.Domain.Entities;
using HMSService.Domain.Repositories;
using Microsoft.Extensions.Logging.Abstractions;
using Moq;
using Xunit;

namespace HMSService.Tests.Services;

public sealed class BedServiceTests
{
    private readonly Mock<IRepository<HmsBed>> _beds = new();
    private readonly Mock<IRepository<HmsWard>> _wards = new();
    private readonly Mock<ITenantContext> _tenant = new();
    private readonly Mock<IFacilityTenantValidator> _facilityValidator = new();
    private readonly Mock<IValidator<CreateBedDto>> _createValidator = new();
    private readonly Mock<IValidator<UpdateBedDto>> _updateValidator = new();
    private readonly IMapper _mapper;

    public BedServiceTests()
    {
        var cfg = new MapperConfiguration(c => c.AddProfile<HmsMappingProfile>());
        _mapper = cfg.CreateMapper();

        _createValidator
            .Setup(v => v.ValidateAsync(It.IsAny<CreateBedDto>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new ValidationResult());
        _updateValidator
            .Setup(v => v.ValidateAsync(It.IsAny<UpdateBedDto>(), It.IsAny<CancellationToken>()))
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

    private BedService CreateSut() =>
        new(
            _beds.Object,
            _wards.Object,
            _mapper,
            _tenant.Object,
            _createValidator.Object,
            _updateValidator.Object,
            _facilityValidator.Object,
            NullLogger<BedService>.Instance);

    [Fact]
    public async Task CreateAsync_WhenWardWrongFacility_ReturnsFailure()
    {
        _wards.Setup(w => w.GetByIdAsync(7L, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new HmsWard { Id = 7, FacilityId = 99, WardCode = "W", WardName = "Ward" });

        var sut = CreateSut();
        var result = await sut.CreateAsync(new CreateBedDto
        {
            WardId = 7,
            BedCode = "A1",
            BedOperationalStatusReferenceValueId = 1
        });

        result.Success.Should().BeFalse();
        result.Message.Should().Contain("Ward");
        _beds.Verify(b => b.AddAsync(It.IsAny<HmsBed>(), It.IsAny<CancellationToken>()), Times.Never);
    }
}
