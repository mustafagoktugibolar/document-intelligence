using Application.Abstractions.Messaging;
using Application.Documents.SetVisibility;
using Domain.Documents;
using SharedKernel;
using Web.Api.Extensions;
using Web.Api.Infrastructure;

namespace Web.Api.Endpoints.Documents;

internal sealed class SetVisibility : IEndpoint
{
    public sealed class Request
    {
        public DocumentVisibility Visibility { get; set; }
    }

    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPut("documents/{id:guid}/visibility", async (
            Guid id,
            Request request,
            ICommandHandler<SetDocumentVisibilityCommand> handler,
            CancellationToken cancellationToken) =>
        {
            var command = new SetDocumentVisibilityCommand
            {
                DocumentId = id,
                Visibility = request.Visibility
            };

            Result result = await handler.Handle(command, cancellationToken);

            return result.Match(Results.NoContent, CustomResults.Problem);
        })
        .HasPermission(Permissions.DocumentsManage)
        .WithTags(Tags.Documents);
    }
}
