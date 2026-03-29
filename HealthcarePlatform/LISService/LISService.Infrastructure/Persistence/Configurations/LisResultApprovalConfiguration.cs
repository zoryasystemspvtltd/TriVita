using LISService.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace LISService.Infrastructure.Persistence.Configurations;

public sealed class LisResultApprovalConfiguration : IEntityTypeConfiguration<LisResultApproval>
{
    public void Configure(EntityTypeBuilder<LisResultApproval> builder)
    {
        builder.ToTable("LIS_ResultApproval");
        builder.HasKey(e => e.Id);
        builder.Property(e => e.RowVersion).IsRowVersion();
        builder.Property(e => e.ApprovalNotes).HasMaxLength(1000);
    }
}