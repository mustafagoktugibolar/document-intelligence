using Application.Abstractions.Messaging;
using Application.Organizations.AddMember;
using Domain.Organizations;
using SharedKernel;
using Web.Api.Extensions;
using Web.Api.Infrastructure;

namespace Web.Api.Endpoints.Organizations;

internal sealed class AddMember : IEndpoint
{
    public sealed class Request
    {
        public Guid UserId { get; set; }
        public OrganizationRole Role { get; set; }
    }

    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPost("organizations/{id:guid}/members", async (
            Guid id,
            Request request,
            ICommandHandler<AddMemberCommand> handler,
            CancellationToken cancellationToken) =>
        {
            var command = new AddMemberCommand
            {
                OrganizationId = id,
                UserId = request.UserId,
                Role = request.Role
            };

            Result result = await handler.Handle(command, cancellationToken);

            return result.Match(Results.NoContent, CustomResults.Problem);
        })
        .HasPermission(Permissions.OrganizationsManage)
        .WithTags(Tags.Organizations);
    }
}
