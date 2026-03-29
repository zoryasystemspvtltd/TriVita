using LISService.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace LISService.Infrastructure.Persistence.Configurations;

public sealed class LisReportDeliveryOtpConfiguration : IEntityTypeConfiguration<LisReportDeliveryOtp>
{
    public void Configure(EntityTypeBuilder<LisReportDeliveryOtp> builder)
    {
        builder.ToTable("LIS_ReportDeliveryOtp");
        builder.HasKey(e => e.Id);
        builder.Property(e => e.RowVersion).IsRowVersion();
        builder.Property(e => e.OtpHash).HasMaxLength(256);
    }
}
