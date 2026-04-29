using System;
using Healthcare.Common.Entities;

namespace PharmacyService.Domain.Entities;

public sealed class PhrCustomer : BaseEntity
{
    public string CustomerName { get; set; } = null!;
    public string MobileNumber { get; set; } = null!;
    public string? AlternatePhone { get; set; }
    public string? Email { get; set; }
    public string? Address { get; set; }
    public DateTime? Dob { get; set; }
    public string? AadhaarNumber { get; set; }
    public string? Gender { get; set; }
}

