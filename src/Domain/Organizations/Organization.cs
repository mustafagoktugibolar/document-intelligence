using SharedKernel;

namespace Domain.Organizations;

public sealed class Organization : Entity
{
    private readonly List<OrganizationMember> _members = [];

    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }

    public IReadOnlyList<OrganizationMember> Members => _members.AsReadOnly();

    public static Organization Create(string name, Guid ownerUserId)
    {
        Organization organization = new()
        {
            Id = Guid.NewGuid(),
            Name = name,
            CreatedAt = DateTime.UtcNow
        };

        OrganizationMember ownerMember = new()
        {
            Id = Guid.NewGuid(),
            OrganizationId = organization.Id,
            UserId = ownerUserId,
            Role = OrganizationRole.Owner,
            JoinedAt = DateTime.UtcNow
        };

        organization._members.Add(ownerMember);
        organization.Raise(new OrganizationCreatedDomainEvent(organization.Id, ownerUserId));

        return organization;
    }

    public Result AddMember(Guid userId, OrganizationRole role)
    {
        if (_members.Any(m => m.UserId == userId))
        {
            return Result.Failure(OrganizationErrors.MemberAlreadyExists);
        }

        OrganizationMember member = new()
        {
            Id = Guid.NewGuid(),
            OrganizationId = Id,
            UserId = userId,
            Role = role,
            JoinedAt = DateTime.UtcNow
        };

        _members.Add(member);
        Raise(new MemberAddedDomainEvent(Id, userId, role));

        return Result.Success();
    }

    public Result ChangeMemberRole(Guid userId, OrganizationRole newRole)
    {
        OrganizationMember? member = _members.FirstOrDefault(m => m.UserId == userId);
        if (member is null)
        {
            return Result.Failure(OrganizationErrors.MemberNotFound(userId));
        }

        if (member.Role == OrganizationRole.Owner && newRole != OrganizationRole.Owner)
        {
            int ownerCount = _members.Count(m => m.Role == OrganizationRole.Owner);
            if (ownerCount <= 1)
            {
                return Result.Failure(OrganizationErrors.CannotRemoveLastOwner);
            }
        }

        OrganizationRole previousRole = member.Role;
        member.Role = newRole;
        Raise(new MemberRoleChangedDomainEvent(Id, userId, previousRole, newRole));

        return Result.Success();
    }

    public Result RemoveMember(Guid userId)
    {
        OrganizationMember? member = _members.FirstOrDefault(m => m.UserId == userId);
        if (member is null)
        {
            return Result.Failure(OrganizationErrors.MemberNotFound(userId));
        }

        if (member.Role == OrganizationRole.Owner)
        {
            int ownerCount = _members.Count(m => m.Role == OrganizationRole.Owner);
            if (ownerCount <= 1)
            {
                return Result.Failure(OrganizationErrors.CannotRemoveLastOwner);
            }
        }

        _members.Remove(member);

        return Result.Success();
    }

    public bool HasMember(Guid userId) => _members.Any(m => m.UserId == userId);

    public OrganizationRole? GetMemberRole(Guid userId) =>
        _members.FirstOrDefault(m => m.UserId == userId)?.Role;
}
