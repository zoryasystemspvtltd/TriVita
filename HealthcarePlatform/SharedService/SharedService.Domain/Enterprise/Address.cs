namespace SharedService.Domain.Enterprise;

public sealed class Address : AuditedEntityBase
{
    public long? FacilityId { get; set; }

    public string? AddressType { get; set; }

    public string Line1 { get; set; } = null!;

    public string? Line2 { get; set; }

    public string? Area { get; set; }

    public string? City { get; set; }

    public string? StateProvince { get; set; }

    public string? PostalCode { get; set; }

    public string? CountryCode { get; set; }

    public decimal? Latitude { get; set; }

    public decimal? Longitude { get; set; }

    public DateTime? EffectiveFrom { get; set; }

    public DateTime? EffectiveTo { get; set; }
}
