using SharedKernel;

namespace Domain.Documents;

public sealed class Document : Entity
{
    private readonly List<AccessEntry> _accessEntries = [];
    private readonly List<DocumentSection> _sections = [];

    public Guid Id { get; set; }
    public Guid OrganizationId { get; set; }
    public Guid UploadedByUserId { get; set; }
    public string FileName { get; set; } = string.Empty;
    public string StorageKey { get; set; } = string.Empty;
    public long FileSizeBytes { get; set; }
    public DocumentStatus Status { get; set; }
    public DocumentVisibility Visibility { get; set; }
    public int TotalChunks { get; set; }
    public string? Summary { get; set; }
    public string? Classification { get; set; }
    public DateTime UploadedAt { get; set; }

    public IReadOnlyList<AccessEntry> AccessEntries => _accessEntries.AsReadOnly();
    public IReadOnlyList<DocumentSection> Sections => _sections.AsReadOnly();

    public static Document Create(
        Guid organizationId,
        Guid uploadedByUserId,
        string fileName,
        string storageKey,
        long fileSizeBytes)
    {
        var document = new Document
        {
            Id = Guid.NewGuid(),
            OrganizationId = organizationId,
            UploadedByUserId = uploadedByUserId,
            FileName = fileName,
            StorageKey = storageKey,
            FileSizeBytes = fileSizeBytes,
            Status = DocumentStatus.Uploaded,
            Visibility = DocumentVisibility.OrgWide,
            TotalChunks = 0,
            UploadedAt = DateTime.UtcNow
        };

        document.Raise(new DocumentUploadedDomainEvent(document.Id, organizationId, uploadedByUserId));

        return document;
    }

    public void UpdateStatus(DocumentStatus newStatus)
    {
        Status = newStatus;
    }

    public void SetSummary(string summary)
    {
        Summary = summary;
    }

    public void SetClassification(string classification)
    {
        Classification = classification;
    }

    public void SetTotalChunks(int count)
    {
        TotalChunks = count;
    }

    public void MakeRestricted()
    {
        Visibility = DocumentVisibility.Restricted;
    }

    public void MakeOrgWide()
    {
        Visibility = DocumentVisibility.OrgWide;
        _accessEntries.Clear();
    }

    public Result GrantAccess(AccessEntry entry)
    {
        if (Visibility != DocumentVisibility.Restricted)
        {
            return Result.Failure(DocumentErrors.MustBeRestrictedToGrantAccess);
        }

        _accessEntries.Add(entry);
        return Result.Success();
    }

    public void RevokeAccess(PrincipalType principalType, string principalId)
    {
        _accessEntries.RemoveAll(e =>
            e.PrincipalType == principalType && e.PrincipalId == principalId);
    }

    public Result AddSection(
        string name,
        int startChunkIndex,
        int endChunkIndex,
        SectionVisibility visibility,
        string? sourceTag = null)
    {
        if (startChunkIndex > endChunkIndex)
        {
            return Result.Failure(DocumentErrors.InvalidChunkRange);
        }

        var section = new DocumentSection
        {
            Id = Guid.NewGuid(),
            DocumentId = Id,
            Name = name,
            StartChunkIndex = startChunkIndex,
            EndChunkIndex = endChunkIndex,
            Visibility = visibility,
            SourceTag = sourceTag
        };

        _sections.Add(section);
        return Result.Success();
    }
}
