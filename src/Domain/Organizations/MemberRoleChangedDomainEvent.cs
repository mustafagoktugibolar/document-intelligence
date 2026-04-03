using SharedKernel;

namespace Domain.Organizations;

public sealed record MemberRoleChangedDomainEvent(
    Guid OrganizationId,
    Guid UserId,
    OrganizationRole PreviousRole,
    OrganizationRole NewRole) : IDomainEvent;
