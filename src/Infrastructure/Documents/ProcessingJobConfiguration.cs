using Domain.Documents;
using Domain.Processing;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Documents;

internal sealed class ProcessingJobConfiguration : IEntityTypeConfiguration<ProcessingJob>
{
    public void Configure(EntityTypeBuilder<ProcessingJob> builder)
    {
        builder.HasKey(j => j.Id);

        builder.Property(j => j.JobType).HasConversion<int>();
        builder.Property(j => j.JobStatus).HasConversion<int>();

        builder.Property(j => j.ErrorMessage).HasMaxLength(2000);

        builder.Property(j => j.StartedAt)
            .HasConversion(d => d != null ? DateTime.SpecifyKind(d.Value, DateTimeKind.Utc) : d, v => v);

        builder.Property(j => j.CompletedAt)
            .HasConversion(d => d != null ? DateTime.SpecifyKind(d.Value, DateTimeKind.Utc) : d, v => v);

        builder.HasOne<Document>().WithMany().HasForeignKey(j => j.DocumentId);
    }
}
