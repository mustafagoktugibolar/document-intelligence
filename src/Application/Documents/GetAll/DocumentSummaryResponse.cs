using Domain.Documents;

namespace Application.Documents.GetAll;

public sealed class DocumentSummaryResponse
{
    public Guid Id { get; set; }
    public string FileName { get; set; } = string.Empty;
    public long FileSizeBytes { get; set; }
    public DocumentStatus Status { get; set; }
    public DocumentVisibility Visibility { get; set; }
    public DateTime UploadedAt { get; set; }
}
