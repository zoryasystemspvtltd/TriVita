using AutoMapper;
using FluentAssertions;
using FluentValidation;
using FluentValidation.Results;
using Healthcare.Common.Integration.SharedService;
using Healthcare.Common.MultiTenancy;
using Healthcare.Common.Pagination;
using PharmacyService.Application.DTOs.Entities;
using PharmacyService.Application.Mapping;
using PharmacyService.Application.Services.Entities;
using PharmacyService.Domain.Entities;
using PharmacyService.Domain.Repositories;
using Microsoft.Extensions.Logging.Abstractions;
using Moq;
using Xunit;

namespace PharmacyService.Tests.Services;

public sealed class PhrMedicineCategoryServiceTests
{
    private readonly Mock<IRepository<PhrMedicineCategory>> _repository = new();
    private readonly Mock<ITenantContext> _tenant = new();
    private readonly Mock<IValidator<CreateMedicineCategoryDto>> _createValidator = new();
    private readonly Mock<IValidator<UpdateMedicineCategoryDto>> _updateValidator = new();
    private readonly Mock<IFacilityTenantValidator> _facilityValidator = new();
    private readonly IMapper _mapper;

    public PhrMedicineCategoryServiceTests()
    {
        var cfg = new MapperConfiguration(c =>
        {
            c.AddProfile<PharmacyMappingProfile>();
            c.AddProfile<PhrGeneratedMappingProfile>();
        });
        _mapper = cfg.CreateMapper();

        _createValidator
            .Setup(v => v.ValidateAsync(It.IsAny<CreateMedicineCategoryDto>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new ValidationResult());
        _updateValidator
            .Setup(v => v.ValidateAsync(It.IsAny<UpdateMedicineCategoryDto>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new ValidationResult());

        _tenant.SetupGet(t => t.TenantId).Returns(1);
        _tenant.SetupGet(t => t.UserId).Returns(1);
        _tenant.SetupGet(t => t.FacilityId).Returns((long?)null);

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

    private PhrMedicineCategoryService CreateSut() =>
        new(
            _repository.Object,
            _mapper,
            _tenant.Object,
            _createValidator.Object,
            _updateValidator.Object,
            _facilityValidator.Object,
            NullLogger<PhrMedicineCategoryService>.Instance);

    [Fact]
    public async Task GetPagedAsync_ReturnsSuccess()
    {
        _repository
            .Setup(r => r.GetPagedByFilterAsync(1, 20, null, It.IsAny<CancellationToken>()))
            .ReturnsAsync((Array.Empty<PhrMedicineCategory>(), 0));

        var sut = CreateSut();
        var result = await sut.GetPagedAsync(new PagedQuery { Page = 1, PageSize = 20 });

        result.Success.Should().BeTrue();
    }
}
