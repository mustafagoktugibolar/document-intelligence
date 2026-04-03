using Application.Abstractions.Messaging;

namespace Application.Organizations.RemoveMember;

public sealed class RemoveMemberCommand : ICommand
{
    public Guid OrganizationId { get; set; }
    public Guid UserId { get; set; }
}
