using Healthcare.Common.Entities;

namespace LISService.Domain.Entities;

public sealed class LisAnalyzerResultMap : BaseEntity
{
    public long EquipmentId { get; set; }
    public string ExternalTestCode { get; set; } = null!;
    public string ExternalParameterCode { get; set; } = string.Empty;
    public long TestMasterId { get; set; }
    public long? TestParameterId { get; set; }
    public long? ProtocolReferenceValueId { get; set; }
    public long? UnitOverrideId { get; set; }
}
