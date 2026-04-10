using Domain.Documents;

namespace Application.Abstractions.Documents;

public interface IChunkingService
{
    IReadOnlyList<DocumentChunk> Chunk(string text, Guid documentId, int maxTokensPerChunk = 512);
}
