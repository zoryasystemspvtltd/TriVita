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

public sealed class Lms07EntityServicesTests
{
    private readonly Mock<ITenantContext> _tenant = new();
    private readonly Mock<IFacilityTenantValidator> _facilityValidator = new();
    private readonly IMapper _mapper;

    public Lms07EntityServicesTests()
    {
        var cfg = new MapperConfiguration(c =>
        {
            c.AddProfile<LMSMappingProfile>();
            c.AddProfile<LmsGeneratedMappingProfile>();
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
    public async Task LmsLabInvoiceHeaderService_GetPaged_ReturnsSuccess()
    {
        var repo = new Mock<IRepository<LmsLabInvoiceHeader>>();
        repo.Setup(r => r.GetPagedByFilterAsync(1, 20, null, It.IsAny<CancellationToken>()))
            .ReturnsAsync((Array.Empty<LmsLabInvoiceHeader>(), 0));
        var v1 = new Mock<IValidator<CreateLabInvoiceHeaderDto>>();
        var v2 = new Mock<IValidator<UpdateLabInvoiceHeaderDto>>();
        v1.Setup(v => v.ValidateAsync(It.IsAny<CreateLabInvoiceHeaderDto>(), It.IsAny<CancellationToken>())).ReturnsAsync(new ValidationResult());
        v2.Setup(v => v.ValidateAsync(It.IsAny<UpdateLabInvoiceHeaderDto>(), It.IsAny<CancellationToken>())).ReturnsAsync(new ValidationResult());

        var sut = new LmsLabInvoiceHeaderService(repo.Object, _mapper, _tenant.Object, v1.Object, v2.Object, _facilityValidator.Object, NullLogger<LmsLabInvoiceHeaderService>.Instance);
        var result = await sut.GetPagedAsync(new PagedQuery { Page = 1, PageSize = 20 });
        result.Success.Should().BeTrue();
    }

    [Fact]
    public async Task IamUserAccountService_Create_WithoutFacility_Succeeds()
    {
        _tenant.SetupGet(t => t.FacilityId).Returns((long?)null);
        var repo = new Mock<IRepository<IamUserAccount>>();
        repo.Setup(r => r.AddAsync(It.IsAny<IamUserAccount>(), It.IsAny<CancellationToken>())).Returns(Task.CompletedTask);
        repo.Setup(r => r.SaveChangesAsync(It.IsAny<CancellationToken>())).ReturnsAsync(1);
        var v1 = new Mock<IValidator<CreateIamUserAccountDto>>();
        var v2 = new Mock<IValidator<UpdateIamUserAccountDto>>();
        v1.Setup(v => v.ValidateAsync(It.IsAny<CreateIamUserAccountDto>(), It.IsAny<CancellationToken>())).ReturnsAsync(new ValidationResult());

        var sut = new IamUserAccountService(repo.Object, _mapper, _tenant.Object, v1.Object, v2.Object, _facilityValidator.Object, NullLogger<IamUserAccountService>.Instance);
        var result = await sut.CreateAsync(new CreateIamUserAccountDto
        {
            LoginName = "u1",
            UserStatusReferenceValueId = 1
        });
        result.Success.Should().BeTrue();
    }
}
