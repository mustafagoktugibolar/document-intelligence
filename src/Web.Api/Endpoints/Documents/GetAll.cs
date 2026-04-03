using Application.Abstractions.Messaging;
using Application.Documents.GetAll;
using SharedKernel;
using Web.Api.Extensions;
using Web.Api.Infrastructure;

namespace Web.Api.Endpoints.Documents;

internal sealed class GetAll : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapGet("documents", async (
            Guid orgId,
            IQueryHandler<GetDocumentsQuery, List<DocumentSummaryResponse>> handler,
            CancellationToken cancellationToken) =>
        {
            var query = new GetDocumentsQuery(orgId);

            Result<List<DocumentSummaryResponse>> result = await handler.Handle(query, cancellationToken);

            return result.Match(Results.Ok, CustomResults.Problem);
        })
        .HasPermission(Permissions.DocumentsRead)
        .WithTags(Tags.Documents);
    }
}
