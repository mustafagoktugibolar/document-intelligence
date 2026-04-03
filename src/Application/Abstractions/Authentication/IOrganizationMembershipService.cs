using Domain.Organizations;

namespace Application.Abstractions.Authentication;

public interface IOrganizationMembershipService
{
    Task<OrganizationRole?> GetRoleAsync(Guid userId, Guid organizationId, CancellationToken cancellationToken = default);

    Task<bool> IsMemberAsync(Guid userId, Guid organizationId, CancellationToken cancellationToken = default);
}
