using Domain.Organizations;

namespace Application.Organizations.GetById;

public sealed class OrganizationResponse
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
    public List<OrganizationMemberResponse> Members { get; set; } = [];
}

public sealed class OrganizationMemberResponse
{
    public Guid UserId { get; set; }
    public OrganizationRole Role { get; set; }
    public DateTime JoinedAt { get; set; }
}
