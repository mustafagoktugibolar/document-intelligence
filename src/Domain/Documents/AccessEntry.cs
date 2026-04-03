using Domain.Organizations;

namespace Domain.Documents;

public sealed class AccessEntry
{
    private AccessEntry() { }

    public PrincipalType PrincipalType { get; private set; }
    public string PrincipalId { get; private set; } = string.Empty;
    public AccessPermission Permission { get; private set; }

    public static AccessEntry ForUser(Guid userId, AccessPermission permission) =>
        new()
        {
            PrincipalType = PrincipalType.User,
            PrincipalId = userId.ToString(),
            Permission = permission
        };

    public static AccessEntry ForRole(OrganizationRole role, AccessPermission permission) =>
        new()
        {
            PrincipalType = PrincipalType.OrgRole,
            PrincipalId = role.ToString(),
            Permission = permission
        };

    public bool Matches(Guid userId, OrganizationRole? memberRole)
    {
        if (PrincipalType == PrincipalType.User)
        {
            return PrincipalId == userId.ToString();
        }

        if (PrincipalType == PrincipalType.OrgRole && memberRole.HasValue)
        {
            return PrincipalId == memberRole.Value.ToString();
        }

        return false;
    }
}
