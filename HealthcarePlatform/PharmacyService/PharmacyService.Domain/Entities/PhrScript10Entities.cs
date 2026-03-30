using Healthcare.Common.Entities;

namespace PharmacyService.Domain.Entities;

public sealed class PhrInventoryLocation : BaseEntity
{
    public string LocationCode { get; set; } = null!;
    public string LocationName { get; set; } = null!;
    public long LocationTypeReferenceValueId { get; set; }
    public long? ParentLocationId { get; set; }
}

public sealed class PhrSalesReturn : BaseEntity
{
    public string ReturnNo { get; set; } = null!;
    public long OriginalSalesId { get; set; }
    public long? ReturnReasonReferenceValueId { get; set; }
    public long StatusReferenceValueId { get; set; }
    public DateTime ReturnedOn { get; set; }
}

public sealed class PhrSalesReturnItem : BaseEntity
{
    public long SalesReturnId { get; set; }
    public long OriginalSalesItemId { get; set; }
    public decimal QuantityReturned { get; set; }
    public long? ReconciliationStatusReferenceValueId { get; set; }
}

public sealed class PhrControlledDrugRegister : BaseEntity
{
    public long PharmacySalesItemId { get; set; }
    public long PrescribingDoctorId { get; set; }
    public long PatientId { get; set; }
    public bool PatientAcknowledged { get; set; }
    public DateTime? PatientAcknowledgedOn { get; set; }
    public DateTime RegisterEntryOn { get; set; }
}

public sealed class PhrBatchStockLocation : BaseEntity
{
    public long BatchStockId { get; set; }
    public long InventoryLocationId { get; set; }
    public decimal QuantityOnHand { get; set; }
}

public sealed class PhrReorderPolicy : BaseEntity
{
    public long BatchStockId { get; set; }
    public int LeadTimeDays { get; set; }
    public decimal? EconomicOrderQty { get; set; }
}
