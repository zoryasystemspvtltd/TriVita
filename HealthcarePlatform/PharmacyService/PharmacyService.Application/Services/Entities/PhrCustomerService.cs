using System.Text.RegularExpressions;
using Healthcare.Common.MultiTenancy;
using Healthcare.Common.Pagination;
using Healthcare.Common.Responses;
using Microsoft.Extensions.Logging;
using PharmacyService.Application.DTOs.Entities;
using PharmacyService.Domain.Entities;
using PharmacyService.Domain.Repositories;

namespace PharmacyService.Application.Services.Entities;

public interface IPhrCustomerService
{
    Task<BaseResponse<CustomerResponseDto>> GetByIdAsync(long id, CancellationToken cancellationToken = default);
    Task<BaseResponse<PagedResponse<CustomerResponseDto>>> GetPagedAsync(PagedQuery query, CancellationToken cancellationToken = default);
    Task<BaseResponse<CustomerResponseDto>> CreateAsync(CreateCustomerDto dto, CancellationToken cancellationToken = default);
    Task<BaseResponse<CustomerResponseDto>> UpdateAsync(long id, UpdateCustomerDto dto, CancellationToken cancellationToken = default);
    Task<BaseResponse<object?>> DeleteAsync(long id, CancellationToken cancellationToken = default);
}

public sealed class PhrCustomerService : IPhrCustomerService
{
    private const string DuplicateMobileMessage = "Customer already exists with same mobile number.";
    private const string DuplicateAadhaarMessage = "Customer already exists with same Aadhaar number.";

    private readonly IRepository<PhrCustomer> _customers;
    private readonly ITenantContext _tenant;
    private readonly ILogger<PhrCustomerService> _logger;

    public PhrCustomerService(
        IRepository<PhrCustomer> customers,
        ITenantContext tenant,
        ILogger<PhrCustomerService> logger)
    {
        _customers = customers;
        _tenant = tenant;
        _logger = logger;
    }

    private static string? TrimOrNull(string? v)
    {
        var t = v?.Trim();
        return string.IsNullOrWhiteSpace(t) ? null : t;
    }

    private static bool IsValidMobile(string? mobile) =>
        mobile is { } m && Regex.IsMatch(m, "^[0-9]{10}$");

    private static bool IsValidAadhaar(string? aadhaar) =>
        aadhaar is { } a && Regex.IsMatch(a, "^[0-9]{12}$");

    private static string? NormalizeAadhaar(string? aadhaar)
    {
        var t = TrimOrNull(aadhaar);
        return t == null ? null : t;
    }

    private async Task<bool> HasDuplicateMobileAsync(string mobile, long? excludeId, CancellationToken cancellationToken)
    {
        // Query filters (tenant + non-deleted) are applied by DbContext.
        var all = await _customers.ListAsync(c => !c.IsDeleted, cancellationToken);
        foreach (var c in all)
        {
            if (excludeId is long x && c.Id == x) continue;
            if (string.Equals(c.MobileNumber?.Trim(), mobile.Trim(), StringComparison.OrdinalIgnoreCase)) return true;
        }

        return false;
    }

    private async Task<bool> HasDuplicateAadhaarAsync(string aadhaar, long? excludeId, CancellationToken cancellationToken)
    {
        var all = await _customers.ListAsync(c => !c.IsDeleted, cancellationToken);
        foreach (var c in all)
        {
            if (excludeId is long x && c.Id == x) continue;
            if (string.IsNullOrWhiteSpace(c.AadhaarNumber)) continue;
            if (string.Equals(c.AadhaarNumber.Trim(), aadhaar.Trim(), StringComparison.OrdinalIgnoreCase)) return true;
        }

        return false;
    }

    private static CustomerResponseDto ToDto(PhrCustomer e) =>
        new()
        {
            Id = e.Id,
            CustomerName = e.CustomerName,
            MobileNumber = e.MobileNumber,
            AlternatePhone = e.AlternatePhone,
            Email = e.Email,
            Address = e.Address,
            Dob = e.Dob,
            AadhaarNumber = e.AadhaarNumber,
            Gender = e.Gender,
            IsActive = e.IsActive
        };

    public async Task<BaseResponse<CustomerResponseDto>> GetByIdAsync(long id, CancellationToken cancellationToken = default)
    {
        var entity = await _customers.GetByIdAsync(id, cancellationToken);
        if (entity is null)
            return BaseResponse<CustomerResponseDto>.Fail("Customer not found.");
        return BaseResponse<CustomerResponseDto>.Ok(ToDto(entity));
    }

    public async Task<BaseResponse<PagedResponse<CustomerResponseDto>>> GetPagedAsync(PagedQuery query, CancellationToken cancellationToken = default)
    {
        var (items, total) = await _customers.GetPagedByFilterAsync(
            query.Page,
            query.PageSize,
            c => !c.IsDeleted,
            cancellationToken);

        return BaseResponse<PagedResponse<CustomerResponseDto>>.Ok(
            new PagedResponse<CustomerResponseDto>
            {
                Items = items.Select(ToDto).ToList(),
                Page = query.Page,
                PageSize = query.PageSize,
                TotalCount = total
            });
    }

    private BaseResponse<CustomerResponseDto> ValidateForUpsert(
        CreateCustomerDto dto,
        long? excludeId,
        CancellationToken cancellationToken)
    {
        dto.CustomerName = dto.CustomerName.Trim();
        dto.MobileNumber = dto.MobileNumber.Trim();
        dto.AlternatePhone = TrimOrNull(dto.AlternatePhone);
        dto.Email = TrimOrNull(dto.Email);
        dto.Address = TrimOrNull(dto.Address);
        dto.AadhaarNumber = NormalizeAadhaar(dto.AadhaarNumber);
        dto.Gender = TrimOrNull(dto.Gender);

        if (dto.CustomerName.Length is 0)
            return BaseResponse<CustomerResponseDto>.Fail("Customer name is required.");

        if (!IsValidMobile(dto.MobileNumber))
            return BaseResponse<CustomerResponseDto>.Fail("Mobile number must be exactly 10 digits.");

        if (dto.Dob is { } dob && dob.Date > DateTime.UtcNow.Date)
            return BaseResponse<CustomerResponseDto>.Fail("DOB cannot be in the future.");

        if (dto.AadhaarNumber is { } aadhaar && !IsValidAadhaar(aadhaar))
            return BaseResponse<CustomerResponseDto>.Fail("Aadhaar number must be exactly 12 digits.");

        return BaseResponse<CustomerResponseDto>.Ok(new CustomerResponseDto());
    }

    private async Task<BaseResponse<CustomerResponseDto>> ValidateForUpsertAsync(
        CreateCustomerDto dto,
        long? excludeId,
        CancellationToken cancellationToken)
    {
        var dtoValidation = ValidateForUpsert(dto, excludeId, cancellationToken);
        if (!dtoValidation.Success) return dtoValidation;

        if (await HasDuplicateMobileAsync(dto.MobileNumber, excludeId, cancellationToken))
            return BaseResponse<CustomerResponseDto>.Fail(DuplicateMobileMessage);

        if (dto.AadhaarNumber is { } aadhaar && await HasDuplicateAadhaarAsync(aadhaar, excludeId, cancellationToken))
            return BaseResponse<CustomerResponseDto>.Fail(DuplicateAadhaarMessage);

        return BaseResponse<CustomerResponseDto>.Ok(new CustomerResponseDto());
    }

    private async Task<BaseResponse<CustomerResponseDto>> ValidateForUpsertAsync(
        UpdateCustomerDto dto,
        long? excludeId,
        CancellationToken cancellationToken)
    {
        var createLike = new CreateCustomerDto
        {
            CustomerName = dto.CustomerName,
            MobileNumber = dto.MobileNumber,
            AlternatePhone = dto.AlternatePhone,
            Email = dto.Email,
            Address = dto.Address,
            Dob = dto.Dob,
            AadhaarNumber = dto.AadhaarNumber,
            Gender = dto.Gender,
            IsActive = dto.IsActive
        };
        return await ValidateForUpsertAsync(createLike, excludeId, cancellationToken);
    }

    public async Task<BaseResponse<CustomerResponseDto>> CreateAsync(CreateCustomerDto dto, CancellationToken cancellationToken = default)
    {
        var normalized = dto;
        normalized.CustomerName = normalized.CustomerName?.Trim() ?? string.Empty;
        normalized.MobileNumber = normalized.MobileNumber?.Trim() ?? string.Empty;

        var validation = await ValidateForUpsertAsync(normalized, excludeId: null, cancellationToken);
        if (!validation.Success)
            return BaseResponse<CustomerResponseDto>.Fail(validation.Message ?? "Validation failed.");

        var entity = new PhrCustomer
        {
            CustomerName = normalized.CustomerName.Trim(),
            MobileNumber = normalized.MobileNumber.Trim(),
            AlternatePhone = TrimOrNull(normalized.AlternatePhone),
            Email = TrimOrNull(normalized.Email),
            Address = TrimOrNull(normalized.Address),
            Dob = normalized.Dob,
            AadhaarNumber = NormalizeAadhaar(normalized.AadhaarNumber),
            Gender = TrimOrNull(normalized.Gender),
            IsActive = normalized.IsActive
        };

        AuditHelper.ApplyCreate(entity, _tenant);
        await _customers.AddAsync(entity, cancellationToken);
        await _customers.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("Customer created tenant {TenantId} id {Id}", _tenant.TenantId, entity.Id);
        return BaseResponse<CustomerResponseDto>.Ok(ToDto(entity), "Created.");
    }

    public async Task<BaseResponse<CustomerResponseDto>> UpdateAsync(long id, UpdateCustomerDto dto, CancellationToken cancellationToken = default)
    {
        var entity = await _customers.GetByIdAsync(id, cancellationToken);
        if (entity is null)
            return BaseResponse<CustomerResponseDto>.Fail("Customer not found.");

        var validation = await ValidateForUpsertAsync(dto, excludeId: id, cancellationToken);
        if (!validation.Success)
            return BaseResponse<CustomerResponseDto>.Fail(validation.Message ?? "Validation failed.");

        dto.CustomerName = dto.CustomerName.Trim();
        dto.MobileNumber = dto.MobileNumber.Trim();
        dto.AlternatePhone = TrimOrNull(dto.AlternatePhone);
        dto.Email = TrimOrNull(dto.Email);
        dto.Address = TrimOrNull(dto.Address);
        dto.AadhaarNumber = NormalizeAadhaar(dto.AadhaarNumber);
        dto.Gender = TrimOrNull(dto.Gender);

        entity.CustomerName = dto.CustomerName;
        entity.MobileNumber = dto.MobileNumber;
        entity.AlternatePhone = dto.AlternatePhone;
        entity.Email = dto.Email;
        entity.Address = dto.Address;
        entity.Dob = dto.Dob;
        entity.AadhaarNumber = dto.AadhaarNumber;
        entity.Gender = dto.Gender;
        entity.IsActive = dto.IsActive;

        AuditHelper.ApplyUpdate(entity, _tenant);
        await _customers.UpdateAsync(entity, cancellationToken);
        await _customers.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("Customer updated tenant {TenantId} id {Id}", _tenant.TenantId, entity.Id);
        return BaseResponse<CustomerResponseDto>.Ok(ToDto(entity), "Updated.");
    }

    public async Task<BaseResponse<object?>> DeleteAsync(long id, CancellationToken cancellationToken = default)
    {
        var entity = await _customers.GetByIdAsync(id, cancellationToken);
        if (entity is null)
            return BaseResponse<object?>.Fail("Customer not found.");

        entity.IsDeleted = true;
        entity.IsActive = false;
        AuditHelper.ApplyUpdate(entity, _tenant);

        await _customers.UpdateAsync(entity, cancellationToken);
        await _customers.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("Customer deleted tenant {TenantId} id {Id}", _tenant.TenantId, id);
        return BaseResponse<object?>.Ok(null, "Deleted.");
    }
}

