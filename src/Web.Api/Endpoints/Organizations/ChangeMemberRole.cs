using Application.Abstractions.Messaging;
using Application.Organizations.ChangeMemberRole;
using Domain.Organizations;
using SharedKernel;
using Web.Api.Extensions;
using Web.Api.Infrastructure;

namespace Web.Api.Endpoints.Organizations;

internal sealed class ChangeMemberRole : IEndpoint
{
    public sealed class Request
    {
        public OrganizationRole NewRole { get; set; }
    }

    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPut("organizations/{id:guid}/members/{userId:guid}", async (
            Guid id,
            Guid userId,
            Request request,
            ICommandHandler<ChangeMemberRoleCommand> handler,
            CancellationToken cancellationToken) =>
        {
            var command = new ChangeMemberRoleCommand
            {
                OrganizationId = id,
                UserId = userId,
                NewRole = request.NewRole
            };

            Result result = await handler.Handle(command, cancellationToken);

            return result.Match(Results.NoContent, CustomResults.Problem);
        })
        .HasPermission(Permissions.OrganizationsManage)
        .WithTags(Tags.Organizations);
    }
}
