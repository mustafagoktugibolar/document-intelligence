using Domain.Documents;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Documents;

internal sealed class DocumentSectionConfiguration : IEntityTypeConfiguration<DocumentSection>
{
    public void Configure(EntityTypeBuilder<DocumentSection> builder)
    {
        builder.HasKey(s => s.Id);

        builder.Property(s => s.Name).HasMaxLength(500).IsRequired();
        builder.Property(s => s.SourceTag).HasMaxLength(200);

        builder.Property(s => s.Visibility).HasConversion<int>();

        builder.OwnsMany(s => s.AccessEntries, entry =>
        {
            entry.ToTable("document_section_access_entries");
            entry.WithOwner().HasForeignKey("section_id");
            entry.Property<Guid>("id");
            entry.HasKey("id");
            entry.Property(e => e.PrincipalType).HasConversion<int>();
            entry.Property(e => e.Permission).HasConversion<int>();
            entry.Property(e => e.PrincipalId).HasMaxLength(100).IsRequired();
        });
    }
}
