using Application.Abstractions.Messaging;
using Domain.Organizations;

namespace Application.Organizations.ChangeMemberRole;

public sealed class ChangeMemberRoleCommand : ICommand
{
    public Guid OrganizationId { get; set; }
    public Guid UserId { get; set; }
    public OrganizationRole NewRole { get; set; }
}
