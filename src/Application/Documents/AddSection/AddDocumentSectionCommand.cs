using Application.Abstractions.Messaging;
using Domain.Documents;

namespace Application.Documents.AddSection;

public sealed class AddDocumentSectionCommand : ICommand<Guid>
{
    public Guid DocumentId { get; set; }
    public string Name { get; set; } = string.Empty;
    public int StartChunkIndex { get; set; }
    public int EndChunkIndex { get; set; }
    public SectionVisibility Visibility { get; set; }
    public string? SourceTag { get; set; }
}
