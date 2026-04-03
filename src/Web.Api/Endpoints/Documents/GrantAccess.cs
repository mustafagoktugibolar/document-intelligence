using Application.Abstractions.Messaging;
using Application.Documents.GrantAccess;
using Domain.Documents;
using SharedKernel;
using Web.Api.Extensions;
using Web.Api.Infrastructure;

namespace Web.Api.Endpoints.Documents;

internal sealed class GrantAccess : IEndpoint
{
    public sealed class Request
    {
        public PrincipalType PrincipalType { get; set; }
        public string PrincipalId { get; set; } = string.Empty;
        public AccessPermission Permission { get; set; }
    }

    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPost("documents/{id:guid}/access", async (
            Guid id,
            Request request,
            ICommandHandler<GrantDocumentAccessCommand> handler,
            CancellationToken cancellationToken) =>
        {
            var command = new GrantDocumentAccessCommand
            {
                DocumentId = id,
                PrincipalType = request.PrincipalType,
                PrincipalId = request.PrincipalId,
                Permission = request.Permission
            };

            Result result = await handler.Handle(command, cancellationToken);

            return result.Match(Results.NoContent, CustomResults.Problem);
        })
        .HasPermission(Permissions.DocumentsManage)
        .WithTags(Tags.Documents);
    }
}
