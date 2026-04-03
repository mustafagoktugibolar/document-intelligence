using Domain.Organizations;

namespace Application.Organizations.GetMine;

public sealed class OrganizationSummaryResponse
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public OrganizationRole MyRole { get; set; }
    public int MemberCount { get; set; }
}
