using Healthcare.Common.Entities;

namespace LISService.Domain.Entities;

public sealed class LisTestParameterProfile : BaseEntity
{
    public long TestParameterId { get; set; }
    public long? MethodReferenceValueId { get; set; }
    public long? CollectionMethodReferenceValueId { get; set; }
    public long? ContainerTypeReferenceValueId { get; set; }
    public string? AnalyzerChannelCode { get; set; }
    public string? LoincCode { get; set; }
    public string? Notes { get; set; }
}
