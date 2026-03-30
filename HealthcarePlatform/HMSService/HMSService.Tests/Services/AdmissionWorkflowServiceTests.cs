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

public sealed class AdmissionWorkflowServiceTests
{
    private readonly Mock<IRepository<HmsAdmission>> _admissions = new();
    private readonly Mock<IRepository<HmsBed>> _beds = new();
    private readonly Mock<IRepository<HmsAdmissionTransfer>> _transfers = new();
    private readonly Mock<IRepository<HmsPatientMaster>> _masters = new();
    private readonly Mock<ITenantContext> _tenant = new();
    private readonly Mock<IFacilityTenantValidator> _facilityValidator = new();
    private readonly Mock<IValidator<AdmitPatientDto>> _admitValidator = new();
    private readonly Mock<IValidator<TransferPatientDto>> _transferValidator = new();
    private readonly Mock<IValidator<DischargePatientDto>> _dischargeValidator = new();
    private readonly IMapper _mapper;

    public AdmissionWorkflowServiceTests()
    {
        var cfg = new MapperConfiguration(c => c.AddProfile<HmsMappingProfile>());
        _mapper = cfg.CreateMapper();

        _admitValidator
            .Setup(v => v.ValidateAsync(It.IsAny<AdmitPatientDto>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new ValidationResult());
        _transferValidator
            .Setup(v => v.ValidateAsync(It.IsAny<TransferPatientDto>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new ValidationResult());
        _dischargeValidator
            .Setup(v => v.ValidateAsync(It.IsAny<DischargePatientDto>(), It.IsAny<CancellationToken>()))
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

    private AdmissionWorkflowService CreateSut() =>
        new(
            _admissions.Object,
            _beds.Object,
            _transfers.Object,
            _masters.Object,
            _mapper,
            _tenant.Object,
            _facilityValidator.Object,
            _admitValidator.Object,
            _transferValidator.Object,
            _dischargeValidator.Object,
            NullLogger<AdmissionWorkflowService>.Instance);

    [Fact]
    public async Task GetPagedAsync_WhenFacilityMissing_ReturnsFailure()
    {
        _tenant.SetupGet(t => t.FacilityId).Returns((long?)null);
        var sut = CreateSut();

        var result = await sut.GetPagedAsync(new PagedQuery { Page = 1, PageSize = 10 }, null);

        result.Success.Should().BeFalse();
        result.Message.Should().Contain("FacilityId");
    }

    [Fact]
    public async Task AdmitAsync_WhenBedOccupied_ReturnsFailure()
    {
        _masters.Setup(m => m.GetByIdAsync(10L, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new HmsPatientMaster { Id = 10, FullName = "A", Upid = "UPID-X" });
        _beds.Setup(b => b.GetByIdAsync(20L, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new HmsBed { Id = 20, FacilityId = 5, WardId = 1, BedCode = "B1", BedOperationalStatusReferenceValueId = 1 });
        _admissions.Setup(a => a.ListAsync(It.IsAny<System.Linq.Expressions.Expression<Func<HmsAdmission, bool>>?>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<HmsAdmission> { new() { Id = 99, BedId = 20, DischargedOn = null } });

        var sut = CreateSut();
        var result = await sut.AdmitAsync(new AdmitPatientDto
        {
            PatientMasterId = 10,
            BedId = 20,
            AdmissionStatusReferenceValueId = 1
        });

        result.Success.Should().BeFalse();
        result.Message.Should().Contain("occupied");
        _admissions.Verify(a => a.AddAsync(It.IsAny<HmsAdmission>(), It.IsAny<CancellationToken>()), Times.Never);
    }
}
