using LISService.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace LISService.Infrastructure.Persistence.Configurations;

public sealed class LisOrderStatusHistoryConfiguration : IEntityTypeConfiguration<LisOrderStatusHistory>
{
    public void Configure(EntityTypeBuilder<LisOrderStatusHistory> builder)
    {
        builder.ToTable("LIS_OrderStatusHistory");
        builder.HasKey(e => e.Id);
        builder.Property(e => e.RowVersion).IsRowVersion();
        builder.Property(e => e.StatusNote).HasMaxLength(1000);
    }
}