using AutoMapper;
using FluentAssertions;
using FluentValidation;
using FluentValidation.Results;
using Healthcare.Common.Integration.SharedService;
using Healthcare.Common.MultiTenancy;
using Healthcare.Common.Pagination;
using HMSService.Application.Abstractions;
using HMSService.Application.DTOs.Appointments;
using HMSService.Application.Mapping;
using HMSService.Application.Services;
using HMSService.Domain.Entities;
using HMSService.Domain.Repositories;
using Microsoft.Extensions.Logging.Abstractions;
using Moq;
using Xunit;

namespace HMSService.Tests.Services;

/// <summary>
/// Unit tests for <see cref="AppointmentService"/> — repositories and validators are mocked.
/// </summary>
public sealed class AppointmentServiceTests
{
    private readonly Mock<IAppointmentRepository> _repository = new();
    private readonly Mock<ITenantContext> _tenant = new();
    private readonly Mock<IValidator<CreateAppointmentDto>> _createValidator = new();
    private readonly Mock<IValidator<UpdateAppointmentDto>> _updateValidator = new();
    private readonly Mock<INotificationHelper> _notificationHelper = new();
    private readonly Mock<IFacilityTenantValidator> _facilityValidator = new();
    private readonly IMapper _mapper;

    public AppointmentServiceTests()
    {
        var cfg = new MapperConfiguration(c => c.AddProfile<HmsMappingProfile>());
        _mapper = cfg.CreateMapper();

        _createValidator
            .Setup(v => v.ValidateAsync(It.IsAny<CreateAppointmentDto>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new ValidationResult());

        _updateValidator
            .Setup(v => v.ValidateAsync(It.IsAny<UpdateAppointmentDto>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new ValidationResult());

        _tenant.SetupGet(t => t.TenantId).Returns(1);
        _tenant.SetupGet(t => t.UserId).Returns(99);

        _notificationHelper
            .Setup(n => n.NotifyAppointmentCreatedAsync(It.IsAny<long>(), It.IsAny<long>(), It.IsAny<string?>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        _facilityValidator
            .Setup(v => v.GetFacilityContextAsync(It.IsAny<long>(), It.IsAny<long>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new FacilityHierarchyContext
            {
                TenantId = 1,
                FacilityId = 1,
                EnterpriseId = 1,
                CompanyId = 1,
                BusinessUnitId = 1
            });
    }

    private AppointmentService CreateSut() =>
        new(
            _repository.Object,
            _mapper,
            _tenant.Object,
            _createValidator.Object,
            _updateValidator.Object,
            _notificationHelper.Object,
            _facilityValidator.Object,
            NullLogger<AppointmentService>.Instance);

    [Fact]
    public async Task CreateAsync_WhenFacilityMissing_ReturnsFailure()
    {
        _tenant.SetupGet(t => t.FacilityId).Returns((long?)null);
        var sut = CreateSut();

        var result = await sut.CreateAsync(new CreateAppointmentDto
        {
            PatientId = 1,
            DoctorId = 2,
            DepartmentId = 3,
            AppointmentStatusValueId = 10,
            ScheduledStartOn = DateTime.UtcNow
        });

        result.Success.Should().BeFalse();
        result.Message.Should().Contain("FacilityId");
        _repository.Verify(r => r.AddAsync(It.IsAny<HmsAppointment>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task CreateAsync_WhenValid_CreatesAndReturnsDto()
    {
        _tenant.SetupGet(t => t.FacilityId).Returns(5L);

        _repository
            .Setup(r => r.AddAsync(It.IsAny<HmsAppointment>(), It.IsAny<CancellationToken>()))
            .Callback<HmsAppointment, CancellationToken>((e, _) => e.Id = 100)
            .Returns(Task.CompletedTask);
        _repository.Setup(r => r.SaveChangesAsync(It.IsAny<CancellationToken>())).ReturnsAsync(1);

        var sut = CreateSut();
        var dto = new CreateAppointmentDto
        {
            PatientId = 1,
            DoctorId = 2,
            DepartmentId = 3,
            AppointmentStatusValueId = 10,
            ScheduledStartOn = new DateTime(2026, 3, 22, 10, 0, 0, DateTimeKind.Utc)
        };

        var result = await sut.CreateAsync(dto);

        result.Success.Should().BeTrue();
        result.Data.Should().NotBeNull();
        result.Data!.PatientId.Should().Be(1);
        result.Data.AppointmentNo.Should().StartWith("APT-");
        _repository.Verify(r => r.AddAsync(It.IsAny<HmsAppointment>(), It.IsAny<CancellationToken>()), Times.Once);
        _repository.Verify(r => r.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task GetByIdAsync_WhenNotFound_ReturnsFailure()
    {
        _tenant.SetupGet(t => t.FacilityId).Returns(1L);
        _repository.Setup(r => r.GetByIdAsync(7, It.IsAny<CancellationToken>())).ReturnsAsync((HmsAppointment?)null);

        var sut = CreateSut();
        var result = await sut.GetByIdAsync(7);

        result.Success.Should().BeFalse();
        result.Message.Should().Contain("not found");
    }

    [Fact]
    public async Task GetPagedAsync_ReturnsPagedWrapper()
    {
        _tenant.SetupGet(t => t.FacilityId).Returns(3L);
        var list = new List<HmsAppointment>
        {
            new()
            {
                Id = 1,
                TenantId = 1,
                FacilityId = 3,
                AppointmentNo = "APT-1",
                PatientId = 1,
                DoctorId = 1,
                DepartmentId = 1,
                AppointmentStatusValueId = 1,
                ScheduledStartOn = DateTime.UtcNow
            }
        };
        _repository
            .Setup(r => r.GetPagedAsync(1, 20, null, false, null, null, null, null, 3, It.IsAny<CancellationToken>()))
            .ReturnsAsync((list, 1));

        var sut = CreateSut();
        var result = await sut.GetPagedAsync(new PagedQuery(), null, null, null, null);

        result.Success.Should().BeTrue();
        result.Data!.TotalCount.Should().Be(1);
        result.Data.Items.Should().HaveCount(1);
    }
}
