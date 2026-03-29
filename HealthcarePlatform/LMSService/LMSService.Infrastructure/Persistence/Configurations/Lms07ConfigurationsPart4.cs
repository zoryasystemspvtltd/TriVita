using LMSService.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace LMSService.Infrastructure.Persistence.Configurations;

public sealed class LmsReagentMasterConfiguration : IEntityTypeConfiguration<LmsReagentMaster>
{
    public void Configure(EntityTypeBuilder<LmsReagentMaster> builder)
    {
        builder.ToTable("LMS_ReagentMaster");
        builder.HasKey(e => e.Id);
        builder.Property(e => e.RowVersion).IsRowVersion();
        builder.Property(e => e.ReagentCode).HasMaxLength(80);
        builder.Property(e => e.ReagentName).HasMaxLength(250);
        builder.Property(e => e.StorageNotes).HasMaxLength(500);
    }
}

public sealed class LmsReagentBatchConfiguration : IEntityTypeConfiguration<LmsReagentBatch>
{
    public void Configure(EntityTypeBuilder<LmsReagentBatch> builder)
    {
        builder.ToTable("LMS_ReagentBatch");
        builder.HasKey(e => e.Id);
        builder.Property(e => e.RowVersion).IsRowVersion();
        builder.Property(e => e.LotNo).HasMaxLength(120);
    }
}

public sealed class LmsTestReagentMapConfiguration : IEntityTypeConfiguration<LmsTestReagentMap>
{
    public void Configure(EntityTypeBuilder<LmsTestReagentMap> builder)
    {
        builder.ToTable("LMS_TestReagentMap");
        builder.HasKey(e => e.Id);
        builder.Property(e => e.RowVersion).IsRowVersion();
    }
}

public sealed class LmsReagentConsumptionLogConfiguration : IEntityTypeConfiguration<LmsReagentConsumptionLog>
{
    public void Configure(EntityTypeBuilder<LmsReagentConsumptionLog> builder)
    {
        builder.ToTable("LMS_ReagentConsumptionLog");
        builder.HasKey(e => e.Id);
        builder.Property(e => e.RowVersion).IsRowVersion();
        builder.Property(e => e.Notes).HasMaxLength(500);
    }
}

public sealed class LmsLabOrderContextConfiguration : IEntityTypeConfiguration<LmsLabOrderContext>
{
    public void Configure(EntityTypeBuilder<LmsLabOrderContext> builder)
    {
        builder.ToTable("LMS_LabOrderContext");
        builder.HasKey(e => e.Id);
        builder.Property(e => e.RowVersion).IsRowVersion();
    }
}

public sealed class LmsReportPaymentGateConfiguration : IEntityTypeConfiguration<LmsReportPaymentGate>
{
    public void Configure(EntityTypeBuilder<LmsReportPaymentGate> builder)
    {
        builder.ToTable("LMS_ReportPaymentGate");
        builder.HasKey(e => e.Id);
        builder.Property(e => e.RowVersion).IsRowVersion();
    }
}

public sealed class LmsFinanceLedgerEntryConfiguration : IEntityTypeConfiguration<LmsFinanceLedgerEntry>
{
    public void Configure(EntityTypeBuilder<LmsFinanceLedgerEntry> builder)
    {
        builder.ToTable("LMS_FinanceLedgerEntry");
        builder.HasKey(e => e.Id);
        builder.Property(e => e.RowVersion).IsRowVersion();
        builder.Property(e => e.Notes).HasMaxLength(500);
    }
}

public sealed class LmsAnalyticsDailyFacilityRollupConfiguration : IEntityTypeConfiguration<LmsAnalyticsDailyFacilityRollup>
{
    public void Configure(EntityTypeBuilder<LmsAnalyticsDailyFacilityRollup> builder)
    {
        builder.ToTable("LMS_AnalyticsDailyFacilityRollup");
        builder.HasKey(e => e.Id);
        builder.Property(e => e.RowVersion).IsRowVersion();
    }
}

public sealed class SecDataChangeAuditLogConfiguration : IEntityTypeConfiguration<SecDataChangeAuditLog>
{
    public void Configure(EntityTypeBuilder<SecDataChangeAuditLog> builder)
    {
        builder.ToTable("SEC_DataChangeAuditLog");
        builder.HasKey(e => e.Id);
        builder.Property(e => e.RowVersion).IsRowVersion();
        builder.Property(e => e.EntitySchema).HasMaxLength(50);
        builder.Property(e => e.EntityName).HasMaxLength(120);
        builder.Property(e => e.EntityKeyJson).HasMaxLength(500);
        builder.Property(e => e.ChangeSummary).HasMaxLength(2000);
        builder.Property(e => e.CorrelationId).HasMaxLength(80);
        builder.Property(e => e.ClientIp).HasMaxLength(64);
        builder.Property(e => e.UserAgent).HasMaxLength(500);
    }
}
