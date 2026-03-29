using SharedService.Application.DTOs.Enterprise;
using SharedService.Domain.Enterprise;

namespace SharedService.Infrastructure.Services.Enterprise;

internal static class EnterpriseEntityMappings
{
    public static EnterpriseResponseDto ToDto(this EnterpriseRoot e) =>
        new()
        {
            Id = e.Id,
            EnterpriseCode = e.EnterpriseCode,
            EnterpriseName = e.EnterpriseName,
            RegistrationDetails = e.RegistrationDetails,
            GlobalSettingsJson = e.GlobalSettingsJson,
            PrimaryAddressId = e.PrimaryAddressId,
            PrimaryContactId = e.PrimaryContactId,
            EffectiveFrom = e.EffectiveFrom,
            EffectiveTo = e.EffectiveTo,
            IsActive = e.IsActive
        };

    public static CompanyResponseDto ToDto(this Company e) =>
        new()
        {
            Id = e.Id,
            EnterpriseId = e.EnterpriseId,
            CompanyCode = e.CompanyCode,
            CompanyName = e.CompanyName,
            PAN = e.PAN,
            GSTIN = e.GSTIN,
            LegalIdentifier1 = e.LegalIdentifier1,
            LegalIdentifier2 = e.LegalIdentifier2,
            PrimaryAddressId = e.PrimaryAddressId,
            PrimaryContactId = e.PrimaryContactId,
            EffectiveFrom = e.EffectiveFrom,
            EffectiveTo = e.EffectiveTo,
            IsActive = e.IsActive
        };

    public static BusinessUnitResponseDto ToDto(this BusinessUnit e) =>
        new()
        {
            Id = e.Id,
            CompanyId = e.CompanyId,
            BusinessUnitCode = e.BusinessUnitCode,
            BusinessUnitName = e.BusinessUnitName,
            BusinessUnitType = e.BusinessUnitType,
            RegionCode = e.RegionCode,
            CountryCode = e.CountryCode,
            PrimaryAddressId = e.PrimaryAddressId,
            PrimaryContactId = e.PrimaryContactId,
            EffectiveFrom = e.EffectiveFrom,
            EffectiveTo = e.EffectiveTo,
            IsActive = e.IsActive
        };

    public static FacilityResponseDto ToDto(this Facility e) =>
        new()
        {
            Id = e.Id,
            BusinessUnitId = e.BusinessUnitId,
            FacilityCode = e.FacilityCode,
            FacilityName = e.FacilityName,
            FacilityType = e.FacilityType,
            LicenseDetails = e.LicenseDetails,
            TimeZoneId = e.TimeZoneId,
            GeoCode = e.GeoCode,
            PrimaryAddressId = e.PrimaryAddressId,
            PrimaryContactId = e.PrimaryContactId,
            EffectiveFrom = e.EffectiveFrom,
            EffectiveTo = e.EffectiveTo,
            IsActive = e.IsActive
        };

    public static EnterpriseB2BContractResponseDto ToDto(this EnterpriseB2BContract e) =>
        new()
        {
            Id = e.Id,
            EnterpriseId = e.EnterpriseId,
            FacilityId = e.FacilityId,
            PartnerType = e.PartnerType,
            PartnerName = e.PartnerName,
            ContractCode = e.ContractCode,
            TermsJson = e.TermsJson,
            EffectiveFrom = e.EffectiveFrom,
            EffectiveTo = e.EffectiveTo,
            IsActive = e.IsActive
        };

    public static DepartmentResponseDto ToDto(this Department e) =>
        new()
        {
            Id = e.Id,
            FacilityId = e.FacilityId,
            FacilityParentId = e.FacilityParentId,
            DepartmentCode = e.DepartmentCode,
            DepartmentName = e.DepartmentName,
            DepartmentType = e.DepartmentType,
            ParentDepartmentId = e.ParentDepartmentId,
            PrimaryAddressId = e.PrimaryAddressId,
            PrimaryContactId = e.PrimaryContactId,
            EffectiveFrom = e.EffectiveFrom,
            EffectiveTo = e.EffectiveTo,
            IsActive = e.IsActive
        };
}
