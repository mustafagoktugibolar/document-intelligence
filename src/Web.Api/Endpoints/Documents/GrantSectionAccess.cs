using Application.Abstractions.Messaging;
using Application.Documents.GrantSectionAccess;
using Domain.Documents;
using SharedKernel;
using Web.Api.Extensions;
using Web.Api.Infrastructure;

namespace Web.Api.Endpoints.Documents;

internal sealed class GrantSectionAccess : IEndpoint
{
    public sealed class Request
    {
        public PrincipalType PrincipalType { get; set; }
        public string PrincipalId { get; set; } = string.Empty;
        public AccessPermission Permission { get; set; }
    }

    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPost("sections/{id:guid}/access", async (
            Guid id,
            Request request,
            ICommandHandler<GrantSectionAccessCommand> handler,
            CancellationToken cancellationToken) =>
        {
            var command = new GrantSectionAccessCommand
            {
                SectionId = id,
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
