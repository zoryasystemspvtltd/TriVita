using HMSService.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace HMSService.Infrastructure.Persistence.Configurations;

public sealed class HmsClinicalNoteConfiguration : IEntityTypeConfiguration<HmsClinicalNote>
{
    public void Configure(EntityTypeBuilder<HmsClinicalNote> builder)
    {
        builder.ToTable("HMS_ClinicalNotes");
        builder.HasKey(e => e.Id);
        builder.Property(e => e.RowVersion).IsRowVersion();
        builder.Property(e => e.EncounterSection).HasMaxLength(150);
        builder.Property(e => e.NoteText).HasColumnType("nvarchar(max)");
        builder.Property(e => e.StructuredPayload).HasColumnType("nvarchar(max)");
    }
}