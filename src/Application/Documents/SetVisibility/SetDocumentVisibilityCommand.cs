using Application.Abstractions.Messaging;
using Domain.Documents;

namespace Application.Documents.SetVisibility;

public sealed class SetDocumentVisibilityCommand : ICommand
{
    public Guid DocumentId { get; set; }
    public DocumentVisibility Visibility { get; set; }
}
