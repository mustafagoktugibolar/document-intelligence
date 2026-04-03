using SharedKernel;

namespace Domain.Documents;

public sealed class DocumentSection : Entity
{
    private readonly List<AccessEntry> _accessEntries = [];

    public Guid Id { get; set; }
    public Guid DocumentId { get; set; }
    public string Name { get; set; } = string.Empty;
    public int StartChunkIndex { get; set; }
    public int EndChunkIndex { get; set; }
    public SectionVisibility Visibility { get; set; }
    public string? SourceTag { get; set; }

    public IReadOnlyList<AccessEntry> AccessEntries => _accessEntries.AsReadOnly();

    public Result GrantAccess(AccessEntry entry)
    {
        if (Visibility != SectionVisibility.Restricted)
        {
            return Result.Failure(DocumentErrors.SectionMustBeRestrictedToGrantAccess);
        }

        _accessEntries.Add(entry);
        return Result.Success();
    }

    public void RevokeAccess(PrincipalType principalType, string principalId)
    {
        _accessEntries.RemoveAll(e =>
            e.PrincipalType == principalType && e.PrincipalId == principalId);
    }

    public void MakeRestricted()
    {
        Visibility = SectionVisibility.Restricted;
    }

    public void MakeInherited()
    {
        Visibility = SectionVisibility.InheritFromDocument;
        _accessEntries.Clear();
    }
}
