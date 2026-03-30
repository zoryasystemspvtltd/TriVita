namespace PharmacyService.Application.DTOs.Entities;

public sealed class PhrInventoryLocationResponseDto
{
    public long Id { get; init; }
    public long FacilityId { get; init; }
    public string LocationCode { get; init; } = null!;
    public string LocationName { get; init; } = null!;
    public long LocationTypeReferenceValueId { get; init; }
    public long? ParentLocationId { get; init; }
}

public sealed class CreatePhrInventoryLocationDto
{
    public string LocationCode { get; init; } = null!;
    public string LocationName { get; init; } = null!;
    public long LocationTypeReferenceValueId { get; init; }
    public long? ParentLocationId { get; init; }
}

public sealed class UpdatePhrInventoryLocationDto
{
    public string LocationName { get; init; } = null!;
    public long LocationTypeReferenceValueId { get; init; }
    public long? ParentLocationId { get; init; }
    public bool IsActive { get; init; } = true;
}

public sealed class PhrSalesReturnResponseDto
{
    public long Id { get; init; }
    public long FacilityId { get; init; }
    public string ReturnNo { get; init; } = null!;
    public long OriginalSalesId { get; init; }
    public long? ReturnReasonReferenceValueId { get; init; }
    public long StatusReferenceValueId { get; init; }
    public DateTime ReturnedOn { get; init; }
}

public sealed class CreatePhrSalesReturnDto
{
    public long OriginalSalesId { get; init; }
    public long? ReturnReasonReferenceValueId { get; init; }
    public long StatusReferenceValueId { get; init; }
    public DateTime ReturnedOn { get; init; }
}

public sealed class UpdatePhrSalesReturnDto
{
    public long? ReturnReasonReferenceValueId { get; init; }
    public long StatusReferenceValueId { get; init; }
    public DateTime ReturnedOn { get; init; }
    public bool IsActive { get; init; } = true;
}

public sealed class PhrSalesReturnItemResponseDto
{
    public long Id { get; init; }
    public long SalesReturnId { get; init; }
    public long OriginalSalesItemId { get; init; }
    public decimal QuantityReturned { get; init; }
    public long? ReconciliationStatusReferenceValueId { get; init; }
}

public sealed class CreatePhrSalesReturnItemDto
{
    public long SalesReturnId { get; init; }
    public long OriginalSalesItemId { get; init; }
    public decimal QuantityReturned { get; init; }
    public long? ReconciliationStatusReferenceValueId { get; init; }
}

public sealed class UpdatePhrSalesReturnItemDto
{
    public decimal QuantityReturned { get; init; }
    public long? ReconciliationStatusReferenceValueId { get; init; }
    public bool IsActive { get; init; } = true;
}

public sealed class PhrControlledDrugRegisterResponseDto
{
    public long Id { get; init; }
    public long PharmacySalesItemId { get; init; }
    public long PrescribingDoctorId { get; init; }
    public long PatientId { get; init; }
    public bool PatientAcknowledged { get; init; }
    public DateTime? PatientAcknowledgedOn { get; init; }
    public DateTime RegisterEntryOn { get; init; }
}

public sealed class CreatePhrControlledDrugRegisterDto
{
    public long PharmacySalesItemId { get; init; }
    public long PrescribingDoctorId { get; init; }
    public long PatientId { get; init; }
    public DateTime RegisterEntryOn { get; init; }
}

public sealed class UpdatePhrControlledDrugRegisterDto
{
    public bool PatientAcknowledged { get; init; }
    public DateTime? PatientAcknowledgedOn { get; init; }
    public bool IsActive { get; init; } = true;
}

public sealed class PhrBatchStockLocationResponseDto
{
    public long Id { get; init; }
    public long BatchStockId { get; init; }
    public long InventoryLocationId { get; init; }
    public decimal QuantityOnHand { get; init; }
}

public sealed class CreatePhrBatchStockLocationDto
{
    public long BatchStockId { get; init; }
    public long InventoryLocationId { get; init; }
    public decimal QuantityOnHand { get; init; }
}

public sealed class UpdatePhrBatchStockLocationDto
{
    public decimal QuantityOnHand { get; init; }
    public bool IsActive { get; init; } = true;
}

public sealed class PhrReorderPolicyResponseDto
{
    public long Id { get; init; }
    public long BatchStockId { get; init; }
    public int LeadTimeDays { get; init; }
    public decimal? EconomicOrderQty { get; init; }
}

public sealed class CreatePhrReorderPolicyDto
{
    public long BatchStockId { get; init; }
    public int LeadTimeDays { get; init; }
    public decimal? EconomicOrderQty { get; init; }
}

public sealed class UpdatePhrReorderPolicyDto
{
    public int LeadTimeDays { get; init; }
    public decimal? EconomicOrderQty { get; init; }
    public bool IsActive { get; init; } = true;
}
