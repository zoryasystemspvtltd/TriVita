namespace PharmacyService.Application.DTOs.Entities;

public sealed class CustomerResponseDto
{
    public long Id { get; set; }
    public string CustomerName { get; set; } = null!;
    public string MobileNumber { get; set; } = null!;
    public string? AlternatePhone { get; set; }
    public string? Email { get; set; }
    public string? Address { get; set; }
    public DateTime? Dob { get; set; }
    public string? AadhaarNumber { get; set; }
    public string? Gender { get; set; }
    public bool IsActive { get; set; }
}

