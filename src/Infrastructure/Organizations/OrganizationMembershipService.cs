using Application.Abstractions.Authentication;
using Application.Abstractions.Data;
using Domain.Organizations;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Organizations;

internal sealed class OrganizationMembershipService(IApplicationDbContext dbContext) : IOrganizationMembershipService
{
    public async Task<OrganizationRole?> GetRoleAsync(
        Guid userId,
        Guid organizationId,
        CancellationToken cancellationToken = default)
    {
        OrganizationMember? member = await dbContext.OrganizationMembers
            .FirstOrDefaultAsync(
                m => m.UserId == userId && m.OrganizationId == organizationId,
                cancellationToken);

        return member?.Role;
    }

    public async Task<bool> IsMemberAsync(
        Guid userId,
        Guid organizationId,
        CancellationToken cancellationToken = default)
    {
        return await dbContext.OrganizationMembers
            .AnyAsync(
                m => m.UserId == userId && m.OrganizationId == organizationId,
                cancellationToken);
    }
}
