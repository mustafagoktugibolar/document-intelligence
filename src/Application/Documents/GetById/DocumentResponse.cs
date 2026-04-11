using Domain.Documents;

namespace Application.Documents.GetById;

public sealed class DocumentResponse
{
    public Guid Id { get; set; }
    public Guid OrganizationId { get; set; }
    public Guid UploadedByUserId { get; set; }
    public string FileName { get; set; } = string.Empty;
    public long FileSizeBytes { get; set; }
    public DocumentStatus Status { get; set; }
    public DocumentVisibility Visibility { get; set; }
    public int TotalChunks { get; set; }
    public DateTime UploadedAt { get; set; }
    public List<DocumentSectionResponse> Sections { get; set; } = [];
    public string? Summary { get; set; }
    public string? Classification { get; set; }
}

public sealed class DocumentSectionResponse
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public int StartChunkIndex { get; set; }
    public int EndChunkIndex { get; set; }
    public SectionVisibility Visibility { get; set; }
}
