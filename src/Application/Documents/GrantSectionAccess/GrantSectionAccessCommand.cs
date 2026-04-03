using Application.Abstractions.Messaging;
using Domain.Documents;

namespace Application.Documents.GrantSectionAccess;

public sealed class GrantSectionAccessCommand : ICommand
{
    public Guid SectionId { get; set; }
    public PrincipalType PrincipalType { get; set; }
    public string PrincipalId { get; set; } = string.Empty;
    public AccessPermission Permission { get; set; }
}
