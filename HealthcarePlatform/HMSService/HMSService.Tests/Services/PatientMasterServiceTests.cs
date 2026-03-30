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

public sealed class PatientMasterServiceTests
{
    private readonly Mock<IRepository<HmsPatientMaster>> _masters = new();
    private readonly Mock<IRepository<HmsPatientFacilityLink>> _links = new();
    private readonly Mock<ITenantContext> _tenant = new();
    private readonly Mock<IFacilityTenantValidator> _facilityValidator = new();
    private readonly Mock<IValidator<CreatePatientMasterDto>> _createValidator = new();
    private readonly Mock<IValidator<UpdatePatientMasterDto>> _updateValidator = new();
    private readonly Mock<IValidator<LinkPatientFacilityDto>> _linkValidator = new();
    private readonly IMapper _mapper;

    public PatientMasterServiceTests()
    {
        var cfg = new MapperConfiguration(c => c.AddProfile<HmsMappingProfile>());
        _mapper = cfg.CreateMapper();

        _createValidator
            .Setup(x => x.ValidateAsync(It.IsAny<CreatePatientMasterDto>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new ValidationResult());
        _updateValidator
            .Setup(x => x.ValidateAsync(It.IsAny<UpdatePatientMasterDto>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new ValidationResult());
        _linkValidator
            .Setup(x => x.ValidateAsync(It.IsAny<LinkPatientFacilityDto>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new ValidationResult());

        _tenant.SetupGet(t => t.TenantId).Returns(1);
        _tenant.SetupGet(t => t.UserId).Returns(99);

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

    private PatientMasterService CreateSut() =>
        new(
            _masters.Object,
            _links.Object,
            _mapper,
            _tenant.Object,
            _facilityValidator.Object,
            _createValidator.Object,
            _updateValidator.Object,
            _linkValidator.Object,
            NullLogger<PatientMasterService>.Instance);

    [Fact]
    public async Task SearchPagedAsync_WhenLinkedFacilityHasNoLinks_ReturnsEmptyPage()
    {
        _links.Setup(l => l.ListAsync(It.IsAny<System.Linq.Expressions.Expression<Func<HmsPatientFacilityLink, bool>>?>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<HmsPatientFacilityLink>());

        var sut = CreateSut();
        var result = await sut.SearchPagedAsync(new PagedQuery { Page = 1, PageSize = 20 }, null, 5L);

        result.Success.Should().BeTrue();
        result.Data!.TotalCount.Should().Be(0);
        result.Data.Items.Should().BeEmpty();
    }

    [Fact]
    public async Task CreateAsync_GeneratesUpidAndPersists()
    {
        HmsPatientMaster? captured = null;
        _masters.Setup(m => m.AddAsync(It.IsAny<HmsPatientMaster>(), It.IsAny<CancellationToken>()))
            .Callback<HmsPatientMaster, CancellationToken>((e, _) =>
            {
                captured = e;
                e.Id = 42;
            })
            .Returns(Task.CompletedTask);
        _masters.Setup(m => m.SaveChangesAsync(It.IsAny<CancellationToken>())).ReturnsAsync(1);

        var sut = CreateSut();
        var result = await sut.CreateAsync(new CreatePatientMasterDto { FullName = "Jane Doe" });

        result.Success.Should().BeTrue();
        captured.Should().NotBeNull();
        captured!.Upid.Should().StartWith("UPID-T1-");
        _masters.Verify(m => m.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }
}
