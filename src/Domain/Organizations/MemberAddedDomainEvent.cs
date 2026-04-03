using SharedKernel;

namespace Domain.Organizations;

public sealed record MemberAddedDomainEvent(Guid OrganizationId, Guid UserId, OrganizationRole Role) : IDomainEvent;
