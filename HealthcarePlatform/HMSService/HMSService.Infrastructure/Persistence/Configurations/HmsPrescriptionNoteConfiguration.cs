using HMSService.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace HMSService.Infrastructure.Persistence.Configurations;

public sealed class HmsPrescriptionNoteConfiguration : IEntityTypeConfiguration<HmsPrescriptionNote>
{
    public void Configure(EntityTypeBuilder<HmsPrescriptionNote> builder)
    {
        builder.ToTable("HMS_PrescriptionNotes");
        builder.HasKey(e => e.Id);
        builder.Property(e => e.RowVersion).IsRowVersion();
        builder.Property(e => e.NoteText).HasColumnType("nvarchar(max)");
    }
}