using Application.Abstractions.Authentication;
using Application.Abstractions.Documents;
using Application.Abstractions.Messaging;
using Domain.Documents;
using SharedKernel;

namespace Application.Documents.GetChunks;

internal sealed class GetAccessibleChunkIndicesQueryHandler(
    IUserContext userContext,
    IDocumentAccessService documentAccessService)
    : IQueryHandler<GetAccessibleChunkIndicesQuery, IReadOnlyList<int>>
{
    public async Task<Result<IReadOnlyList<int>>> Handle(
        GetAccessibleChunkIndicesQuery query,
        CancellationToken cancellationToken)
    {
        bool canAccess = await documentAccessService.CanAccessDocumentAsync(
            userContext.UserId,
            query.DocumentId,
            cancellationToken);

        if (!canAccess)
        {
            return Result.Failure<IReadOnlyList<int>>(DocumentErrors.AccessDenied);
        }

        IReadOnlyList<int> indices = await documentAccessService.GetAccessibleChunkIndicesAsync(
            userContext.UserId,
            query.DocumentId,
            cancellationToken);

        return Result.Success(indices);
    }
}
