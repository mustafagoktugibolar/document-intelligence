using Application.Abstractions.Messaging;
using Domain.Documents;

namespace Application.Documents.GrantAccess;

public sealed class GrantDocumentAccessCommand : ICommand
{
    public Guid DocumentId { get; set; }
    public PrincipalType PrincipalType { get; set; }
    public string PrincipalId { get; set; } = string.Empty;
    public AccessPermission Permission { get; set; }
}
