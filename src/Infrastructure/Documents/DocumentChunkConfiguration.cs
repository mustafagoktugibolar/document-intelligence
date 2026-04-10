using Domain.Documents;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Documents;

internal sealed class DocumentChunkConfiguration : IEntityTypeConfiguration<DocumentChunk>
{
    public void Configure(EntityTypeBuilder<DocumentChunk> builder)
    {
        builder.HasKey(c => c.Id);

        builder.Property(c => c.Content).IsRequired();

        builder.Property(c => c.EmbeddingVector)
            .HasColumnType("vector(1536)")
            .HasConversion(
                v => new Pgvector.Vector(v),
                v => v.Memory.ToArray());

        builder.HasOne<Document>().WithMany().HasForeignKey(c => c.DocumentId);
    }
}
