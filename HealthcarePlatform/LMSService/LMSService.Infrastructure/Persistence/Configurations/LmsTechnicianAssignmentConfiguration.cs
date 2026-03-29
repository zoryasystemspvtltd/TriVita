using LMSService.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace LMSService.Infrastructure.Persistence.Configurations;

public sealed class LmsTechnicianAssignmentConfiguration : IEntityTypeConfiguration<LmsTechnicianAssignment>
{
    public void Configure(EntityTypeBuilder<LmsTechnicianAssignment> builder)
    {
        builder.ToTable("TechnicianAssignment");
        builder.HasKey(e => e.Id);
        builder.Property(e => e.RowVersion).IsRowVersion();
        builder.Property(e => e.Notes).HasMaxLength(1000);
    }
}