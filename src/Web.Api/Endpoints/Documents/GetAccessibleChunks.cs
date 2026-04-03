using Application.Abstractions.Messaging;
using Application.Documents.GetChunks;
using SharedKernel;
using Web.Api.Extensions;
using Web.Api.Infrastructure;

namespace Web.Api.Endpoints.Documents;

internal sealed class GetAccessibleChunks : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapGet("documents/{id:guid}/chunks/accessible", async (
            Guid id,
            IQueryHandler<GetAccessibleChunkIndicesQuery, IReadOnlyList<int>> handler,
            CancellationToken cancellationToken) =>
        {
            var query = new GetAccessibleChunkIndicesQuery(id);

            Result<IReadOnlyList<int>> result = await handler.Handle(query, cancellationToken);

            return result.Match(Results.Ok, CustomResults.Problem);
        })
        .HasPermission(Permissions.DocumentsRead)
        .WithTags(Tags.Documents);
    }
}
