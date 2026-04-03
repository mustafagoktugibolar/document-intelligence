using Application.Abstractions.Messaging;
using Application.Organizations.RemoveMember;
using SharedKernel;
using Web.Api.Extensions;
using Web.Api.Infrastructure;

namespace Web.Api.Endpoints.Organizations;

internal sealed class RemoveMember : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapDelete("organizations/{id:guid}/members/{userId:guid}", async (
            Guid id,
            Guid userId,
            ICommandHandler<RemoveMemberCommand> handler,
            CancellationToken cancellationToken) =>
        {
            var command = new RemoveMemberCommand
            {
                OrganizationId = id,
                UserId = userId
            };

            Result result = await handler.Handle(command, cancellationToken);

            return result.Match(Results.NoContent, CustomResults.Problem);
        })
        .HasPermission(Permissions.OrganizationsManage)
        .WithTags(Tags.Organizations);
    }
}
