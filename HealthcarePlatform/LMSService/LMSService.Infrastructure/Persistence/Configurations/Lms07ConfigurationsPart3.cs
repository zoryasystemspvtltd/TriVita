using LMSService.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace LMSService.Infrastructure.Persistence.Configurations;

public sealed class LmsReferralDoctorProfileConfiguration : IEntityTypeConfiguration<LmsReferralDoctorProfile>
{
    public void Configure(EntityTypeBuilder<LmsReferralDoctorProfile> builder)
    {
        builder.ToTable("LMS_ReferralDoctorProfile");
        builder.HasKey(e => e.Id);
        builder.Property(e => e.RowVersion).IsRowVersion();
        builder.Property(e => e.ReferralCode).HasMaxLength(80);
        builder.Property(e => e.DisplayName).HasMaxLength(250);
        builder.Property(e => e.HospitalAffiliation).HasMaxLength(300);
    }
}

public sealed class LmsReferralFeeRuleConfiguration : IEntityTypeConfiguration<LmsReferralFeeRule>
{
    public void Configure(EntityTypeBuilder<LmsReferralFeeRule> builder)
    {
        builder.ToTable("LMS_ReferralFeeRule");
        builder.HasKey(e => e.Id);
        builder.Property(e => e.RowVersion).IsRowVersion();
    }
}

public sealed class LmsReferralFeeLedgerConfiguration : IEntityTypeConfiguration<LmsReferralFeeLedger>
{
    public void Configure(EntityTypeBuilder<LmsReferralFeeLedger> builder)
    {
        builder.ToTable("LMS_ReferralFeeLedger");
        builder.HasKey(e => e.Id);
        builder.Property(e => e.RowVersion).IsRowVersion();
    }
}

public sealed class LmsReferralSettlementConfiguration : IEntityTypeConfiguration<LmsReferralSettlement>
{
    public void Configure(EntityTypeBuilder<LmsReferralSettlement> builder)
    {
        builder.ToTable("LMS_ReferralSettlement");
        builder.HasKey(e => e.Id);
        builder.Property(e => e.RowVersion).IsRowVersion();
        builder.Property(e => e.SettlementNo).HasMaxLength(60);
        builder.Property(e => e.PaymentReferenceNo).HasMaxLength(120);
    }
}

public sealed class LmsReferralSettlementLineConfiguration : IEntityTypeConfiguration<LmsReferralSettlementLine>
{
    public void Configure(EntityTypeBuilder<LmsReferralSettlementLine> builder)
    {
        builder.ToTable("LMS_ReferralSettlementLine");
        builder.HasKey(e => e.Id);
        builder.Property(e => e.RowVersion).IsRowVersion();
    }
}

public sealed class LmsB2BPartnerConfiguration : IEntityTypeConfiguration<LmsB2BPartner>
{
    public void Configure(EntityTypeBuilder<LmsB2BPartner> builder)
    {
        builder.ToTable("LMS_B2BPartner");
        builder.HasKey(e => e.Id);
        builder.Property(e => e.RowVersion).IsRowVersion();
        builder.Property(e => e.PartnerCode).HasMaxLength(80);
        builder.Property(e => e.PartnerName).HasMaxLength(250);
        builder.Property(e => e.ContractReference).HasMaxLength(200);
    }
}

public sealed class LmsB2BPartnerTestRateConfiguration : IEntityTypeConfiguration<LmsB2BPartnerTestRate>
{
    public void Configure(EntityTypeBuilder<LmsB2BPartnerTestRate> builder)
    {
        builder.ToTable("LMS_B2BPartnerTestRate");
        builder.HasKey(e => e.Id);
        builder.Property(e => e.RowVersion).IsRowVersion();
        builder.Property(e => e.ContractDocumentRef).HasMaxLength(200);
    }
}

public sealed class LmsB2BPartnerCreditProfileConfiguration : IEntityTypeConfiguration<LmsB2BPartnerCreditProfile>
{
    public void Configure(EntityTypeBuilder<LmsB2BPartnerCreditProfile> builder)
    {
        builder.ToTable("LMS_B2BPartnerCreditProfile");
        builder.HasKey(e => e.Id);
        builder.Property(e => e.RowVersion).IsRowVersion();
        builder.Property(e => e.CreditCurrencyCode).HasMaxLength(3);
    }
}

public sealed class LmsB2BCreditLedgerConfiguration : IEntityTypeConfiguration<LmsB2BCreditLedger>
{
    public void Configure(EntityTypeBuilder<LmsB2BCreditLedger> builder)
    {
        builder.ToTable("LMS_B2BCreditLedger");
        builder.HasKey(e => e.Id);
        builder.Property(e => e.RowVersion).IsRowVersion();
        builder.Property(e => e.ReferenceNo).HasMaxLength(120);
        builder.Property(e => e.Notes).HasMaxLength(500);
    }
}

public sealed class LmsB2BPartnerBillingStatementConfiguration : IEntityTypeConfiguration<LmsB2BPartnerBillingStatement>
{
    public void Configure(EntityTypeBuilder<LmsB2BPartnerBillingStatement> builder)
    {
        builder.ToTable("LMS_B2BPartnerBillingStatement");
        builder.HasKey(e => e.Id);
        builder.Property(e => e.RowVersion).IsRowVersion();
        builder.Property(e => e.StatementNo).HasMaxLength(60);
    }
}

public sealed class LmsB2BPartnerBillingStatementLineConfiguration : IEntityTypeConfiguration<LmsB2BPartnerBillingStatementLine>
{
    public void Configure(EntityTypeBuilder<LmsB2BPartnerBillingStatementLine> builder)
    {
        builder.ToTable("LMS_B2BPartnerBillingStatementLine");
        builder.HasKey(e => e.Id);
        builder.Property(e => e.RowVersion).IsRowVersion();
        builder.Property(e => e.Description).HasMaxLength(500);
    }
}
