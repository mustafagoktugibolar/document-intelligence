using Application.Abstractions.Messaging;
using Domain.Organizations;

namespace Application.Organizations.AddMember;

public sealed class AddMemberCommand : ICommand
{
    public Guid OrganizationId { get; set; }
    public Guid UserId { get; set; }
    public OrganizationRole Role { get; set; }
}
