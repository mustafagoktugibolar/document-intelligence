using SharedKernel;

namespace Domain.Organizations;

public sealed class OrganizationMember : Entity
{
    public Guid Id { get; set; }
    public Guid OrganizationId { get; set; }
    public Guid UserId { get; set; }
    public OrganizationRole Role { get; set; }
    public DateTime JoinedAt { get; set; }
}
