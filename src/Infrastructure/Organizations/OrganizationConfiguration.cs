using Domain.Organizations;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Organizations;

internal sealed class OrganizationConfiguration : IEntityTypeConfiguration<Organization>
{
    public void Configure(EntityTypeBuilder<Organization> builder)
    {
        builder.HasKey(o => o.Id);

        builder.Property(o => o.Name).HasMaxLength(200).IsRequired();

        builder.Property(o => o.CreatedAt)
            .HasConversion(d => DateTime.SpecifyKind(d, DateTimeKind.Utc), v => v);

        builder.HasMany(o => o.Members)
            .WithOne()
            .HasForeignKey(m => m.OrganizationId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
