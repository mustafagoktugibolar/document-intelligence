using Domain.Documents;
using Domain.Users;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Documents;

internal sealed class DocumentConfiguration : IEntityTypeConfiguration<Document>
{
    public void Configure(EntityTypeBuilder<Document> builder)
    {
        builder.HasKey(d => d.Id);

        builder.Property(d => d.FileName).HasMaxLength(500).IsRequired();
        builder.Property(d => d.StorageKey).HasMaxLength(1000).IsRequired();
        builder.Property(d => d.Summary).HasMaxLength(4000);
        builder.Property(d => d.Classification).HasMaxLength(200);

        builder.Property(d => d.Status).HasConversion<int>();
        builder.Property(d => d.Visibility).HasConversion<int>();

        builder.Property(d => d.UploadedAt)
            .HasConversion(d => DateTime.SpecifyKind(d, DateTimeKind.Utc), v => v);

        builder.OwnsMany(d => d.AccessEntries, entry =>
        {
            entry.ToTable("document_access_entries");
            entry.WithOwner().HasForeignKey("document_id");
            entry.Property<Guid>("id");
            entry.HasKey("id");
            entry.Property(e => e.PrincipalType).HasConversion<int>();
            entry.Property(e => e.Permission).HasConversion<int>();
            entry.Property(e => e.PrincipalId).HasMaxLength(100).IsRequired();
        });

        builder.HasMany(d => d.Sections)
            .WithOne()
            .HasForeignKey(s => s.DocumentId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne<User>().WithMany().HasForeignKey(d => d.UploadedByUserId);
    }
}
