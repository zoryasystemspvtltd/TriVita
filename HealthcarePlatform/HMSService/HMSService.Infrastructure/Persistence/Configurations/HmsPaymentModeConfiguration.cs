using HMSService.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace HMSService.Infrastructure.Persistence.Configurations;

public sealed class HmsPaymentModeConfiguration : IEntityTypeConfiguration<HmsPaymentMode>
{
    public void Configure(EntityTypeBuilder<HmsPaymentMode> builder)
    {
        builder.ToTable("HMS_PaymentModes");
        builder.HasKey(e => e.Id);
        builder.Property(e => e.RowVersion).IsRowVersion();
        builder.Property(e => e.ModeCode).HasMaxLength(40);
        builder.Property(e => e.ModeName).HasMaxLength(120);
    }
}