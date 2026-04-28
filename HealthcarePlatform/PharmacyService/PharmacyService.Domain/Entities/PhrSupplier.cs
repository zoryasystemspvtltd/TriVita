using Healthcare.Common.Entities;

namespace PharmacyService.Domain.Entities;

public sealed class PhrSupplier : BaseEntity
{
    public string SupplierCode { get; set; } = null!;
    public string SupplierName { get; set; } = null!;
    public string Pan { get; set; } = null!;
    public string? Msme { get; set; }
    public string? Tan { get; set; }
    public string? ExportImportCode { get; set; }
    public string? GstNo { get; set; }
    public string? Cin { get; set; }
    public string? ContactPerson { get; set; }
    public string? Phone { get; set; }
    public string? Email { get; set; }
    public string? Address { get; set; }
    public string? Description { get; set; }
}

