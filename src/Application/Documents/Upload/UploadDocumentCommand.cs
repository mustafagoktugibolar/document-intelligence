using Application.Abstractions.Messaging;

namespace Application.Documents.Upload;

public sealed class UploadDocumentCommand : ICommand<Guid>
{
    public Guid OrganizationId { get; set; }
    public string FileName { get; set; } = string.Empty;
    public string StorageKey { get; set; } = string.Empty;
    public long FileSizeBytes { get; set; }
}
