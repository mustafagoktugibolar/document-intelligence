using Domain.Organizations;
using Domain.Users;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Organizations;

internal sealed class OrganizationMemberConfiguration : IEntityTypeConfiguration<OrganizationMember>
{
    public void Configure(EntityTypeBuilder<OrganizationMember> builder)
    {
        builder.HasKey(m => m.Id);

        builder.HasIndex(m => new { m.OrganizationId, m.UserId }).IsUnique();

        builder.Property(m => m.Role).HasConversion<int>();

        builder.Property(m => m.JoinedAt)
            .HasConversion(d => DateTime.SpecifyKind(d, DateTimeKind.Utc), v => v);

        builder.HasOne<User>().WithMany().HasForeignKey(m => m.UserId);
    }
}
