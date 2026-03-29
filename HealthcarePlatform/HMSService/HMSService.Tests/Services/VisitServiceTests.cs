using AutoMapper;
using FluentAssertions;
using FluentValidation;
using FluentValidation.Results;
using Healthcare.Common.Integration.SharedService;
using Healthcare.Common.MultiTenancy;
using Healthcare.Common.Pagination;
using HMSService.Application.DTOs.Visits;
using HMSService.Application.Mapping;
using HMSService.Application.Services;
using HMSService.Domain.Entities;
using HMSService.Domain.Repositories;
using Microsoft.Extensions.Logging.Abstractions;
using Moq;
using Xunit;

namespace HMSService.Tests.Services;

public sealed class VisitServiceTests
{
    private readonly Mock<IVisitRepository> _visitRepository = new();
    private readonly Mock<IAppointmentRepository> _appointmentRepository = new();
    private readonly Mock<ITenantContext> _tenant = new();
    private readonly Mock<IValidator<CreateVisitDto>> _createValidator = new();
    private readonly Mock<IValidator<UpdateVisitDto>> _updateValidator = new();
    private readonly Mock<IFacilityTenantValidator> _facilityValidator = new();
    private readonly IMapper _mapper;

    public VisitServiceTests()
    {
        var cfg = new MapperConfiguration(c => c.AddProfile<HmsMappingProfile>());
        _mapper = cfg.CreateMapper();

        _createValidator
            .Setup(v => v.ValidateAsync(It.IsAny<CreateVisitDto>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new ValidationResult());

        _updateValidator
            .Setup(v => v.ValidateAsync(It.IsAny<UpdateVisitDto>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new ValidationResult());

        _tenant.SetupGet(t => t.TenantId).Returns(1);
        _tenant.SetupGet(t => t.UserId).Returns(99);
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

    private VisitService CreateSut() =>
        new(
            _visitRepository.Object,
            _appointmentRepository.Object,
            _mapper,
            _tenant.Object,
            _createValidator.Object,
            _updateValidator.Object,
            _facilityValidator.Object,
            NullLogger<VisitService>.Instance);

    [Fact]
    public async Task GetByIdAsync_Should_Return_Error_When_NotFound()
    {
        _visitRepository.Setup(r => r.GetByIdAsync(99, It.IsAny<CancellationToken>())).ReturnsAsync((HmsVisit?)null);

        var result = await CreateSut().GetByIdAsync(99);

        result.Success.Should().BeFalse();
        result.Message.Should().Contain("not found");
    }

    [Fact]
    public async Task GetByIdAsync_Should_Return_Error_When_Outside_Facility_Scope()
    {
        var visit = new HmsVisit { Id = 1, FacilityId = 999, TenantId = 1, VisitNo = "V1" };
        _visitRepository.Setup(r => r.GetByIdAsync(1, It.IsAny<CancellationToken>())).ReturnsAsync(visit);

        var result = await CreateSut().GetByIdAsync(1);

        result.Success.Should().BeFalse();
        result.Message.Should().Contain("facility scope");
    }

    [Fact]
    public async Task CreateAsync_Should_Return_Error_When_Facility_Missing()
    {
        _tenant.SetupGet(t => t.FacilityId).Returns((long?)null);

        var result = await CreateSut().CreateAsync(new CreateVisitDto
        {
            PatientId = 1,
            DoctorId = 2,
            DepartmentId = 3,
            VisitTypeId = 4,
            VisitStartOn = DateTime.UtcNow,
            CurrentStatusReferenceValueId = 5
        });

        result.Success.Should().BeFalse();
        result.Message.Should().Contain("FacilityId");
    }

    [Fact]
    public async Task CreateAsync_Should_Return_Error_When_Appointment_In_Different_Facility()
    {
        _appointmentRepository
            .Setup(r => r.GetByIdAsync(50, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new HmsAppointment { Id = 50, FacilityId = 99, TenantId = 1 });

        var result = await CreateSut().CreateAsync(new CreateVisitDto
        {
            AppointmentId = 50,
            PatientId = 1,
            DoctorId = 2,
            DepartmentId = 3,
            VisitTypeId = 4,
            VisitStartOn = DateTime.UtcNow,
            CurrentStatusReferenceValueId = 5
        });

        result.Success.Should().BeFalse();
        result.Message.Should().Contain("different facility");
    }

    [Fact]
    public async Task CreateAsync_Should_Create_When_Valid()
    {
        _visitRepository
            .Setup(r => r.AddAsync(It.IsAny<HmsVisit>(), It.IsAny<CancellationToken>()))
            .Callback<HmsVisit, CancellationToken>((e, _) => e.Id = 200)
            .Returns(Task.CompletedTask);
        _visitRepository.Setup(r => r.SaveChangesAsync(It.IsAny<CancellationToken>())).ReturnsAsync(1);

        var result = await CreateSut().CreateAsync(new CreateVisitDto
        {
            PatientId = 1,
            DoctorId = 2,
            DepartmentId = 3,
            VisitTypeId = 4,
            VisitStartOn = new DateTime(2026, 3, 1, 9, 0, 0, DateTimeKind.Utc),
            CurrentStatusReferenceValueId = 5
        });

        result.Success.Should().BeTrue();
        result.Data!.Id.Should().Be(200);
        result.Data.VisitNo.Should().StartWith("VIS-");
    }

    [Fact]
    public async Task GetPagedAsync_Should_Return_Page_From_Repository()
    {
        var visits = new List<HmsVisit>
        {
            new()
            {
                Id = 1,
                TenantId = 1,
                FacilityId = 10,
                VisitNo = "V-1",
                PatientId = 1,
                DoctorId = 2,
                DepartmentId = 3,
                VisitTypeId = 4,
                VisitStartOn = DateTime.UtcNow,
                CurrentStatusReferenceValueId = 5
            }
        };

        _visitRepository
            .Setup(r => r.GetPagedAsync(
                1,
                20,
                null,
                false,
                null,
                null,
                null,
                null,
                10,
                It.IsAny<CancellationToken>()))
            .ReturnsAsync((visits, 1));

        var result = await CreateSut().GetPagedAsync(new PagedQuery { Page = 1, PageSize = 20 }, null, null, null, null);

        result.Success.Should().BeTrue();
        result.Data!.TotalCount.Should().Be(1);
        result.Data.Items.Should().HaveCount(1);
    }
}
