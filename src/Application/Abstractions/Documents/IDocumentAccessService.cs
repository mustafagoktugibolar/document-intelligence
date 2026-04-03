namespace Application.Abstractions.Documents;

public interface IDocumentAccessService
{
    Task<bool> CanAccessDocumentAsync(Guid userId, Guid documentId, CancellationToken cancellationToken = default);

    Task<bool> CanAccessSectionAsync(Guid userId, Guid sectionId, CancellationToken cancellationToken = default);

    Task<IReadOnlyList<int>> GetAccessibleChunkIndicesAsync(Guid userId, Guid documentId, CancellationToken cancellationToken = default);
}
