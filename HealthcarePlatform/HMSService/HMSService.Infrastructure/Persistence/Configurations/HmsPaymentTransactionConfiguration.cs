using HMSService.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace HMSService.Infrastructure.Persistence.Configurations;

public sealed class HmsPaymentTransactionConfiguration : IEntityTypeConfiguration<HmsPaymentTransaction>
{
    public void Configure(EntityTypeBuilder<HmsPaymentTransaction> builder)
    {
        builder.ToTable("HMS_PaymentTransactions");
        builder.HasKey(e => e.Id);
        builder.Property(e => e.RowVersion).IsRowVersion();
        builder.Property(e => e.Amount).HasPrecision(18, 4);
        builder.Property(e => e.ReferenceNo).HasMaxLength(120);
        builder.Property(e => e.Notes).HasMaxLength(500);
    }
}