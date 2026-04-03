using Application.Abstractions.Messaging;
using Application.Documents.GetById;
using SharedKernel;
using Web.Api.Extensions;
using Web.Api.Infrastructure;

namespace Web.Api.Endpoints.Documents;

internal sealed class GetById : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapGet("documents/{id:guid}", async (
            Guid id,
            IQueryHandler<GetDocumentByIdQuery, DocumentResponse> handler,
            CancellationToken cancellationToken) =>
        {
            var query = new GetDocumentByIdQuery(id);

            Result<DocumentResponse> result = await handler.Handle(query, cancellationToken);

            return result.Match(Results.Ok, CustomResults.Problem);
        })
        .HasPermission(Permissions.DocumentsRead)
        .WithTags(Tags.Documents);
    }
}
