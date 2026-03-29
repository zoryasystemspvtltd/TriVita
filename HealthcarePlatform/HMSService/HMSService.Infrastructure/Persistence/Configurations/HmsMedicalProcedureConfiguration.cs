using HMSService.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace HMSService.Infrastructure.Persistence.Configurations;

public sealed class HmsMedicalProcedureConfiguration : IEntityTypeConfiguration<HmsMedicalProcedure>
{
    public void Configure(EntityTypeBuilder<HmsMedicalProcedure> builder)
    {
        builder.ToTable("HMS_Procedure");
        builder.HasKey(e => e.Id);
        builder.Property(e => e.RowVersion).IsRowVersion();
        builder.Property(e => e.ProcedureCode).HasMaxLength(50);
        builder.Property(e => e.ProcedureSystem).HasMaxLength(30);
        builder.Property(e => e.ProcedureDescription).HasMaxLength(500);
        builder.Property(e => e.Notes).HasMaxLength(1000);
    }
}