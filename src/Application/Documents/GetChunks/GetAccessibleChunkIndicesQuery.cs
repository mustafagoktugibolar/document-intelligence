using Application.Abstractions.Messaging;

namespace Application.Documents.GetChunks;

public sealed record GetAccessibleChunkIndicesQuery(Guid DocumentId) : IQuery<IReadOnlyList<int>>;
