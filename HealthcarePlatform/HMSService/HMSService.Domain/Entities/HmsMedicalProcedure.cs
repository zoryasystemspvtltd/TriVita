using Healthcare.Common.Entities;

namespace HMSService.Domain.Entities;

public sealed class HmsMedicalProcedure : BaseEntity
{
    public long VisitId { get; set; }
    public string ProcedureCode { get; set; } = null!;
    public string? ProcedureSystem { get; set; }
    public string? ProcedureDescription { get; set; }
    public DateTime? PerformedOn { get; set; }
    public long? PerformedByDoctorId { get; set; }
    public string? Notes { get; set; }
}