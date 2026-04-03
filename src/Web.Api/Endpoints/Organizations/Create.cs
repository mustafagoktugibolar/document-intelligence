using Application.Abstractions.Messaging;
using Application.Organizations.Create;
using SharedKernel;
using Web.Api.Extensions;
using Web.Api.Infrastructure;

namespace Web.Api.Endpoints.Organizations;

internal sealed class Create : IEndpoint
{
    public sealed class Request
    {
        public string Name { get; set; } = string.Empty;
    }

    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPost("organizations", async (
            Request request,
            ICommandHandler<CreateOrganizationCommand, Guid> handler,
            CancellationToken cancellationToken) =>
        {
            var command = new CreateOrganizationCommand
            {
                Name = request.Name
            };

            Result<Guid> result = await handler.Handle(command, cancellationToken);

            return result.Match(id => Results.Created($"organizations/{id}", id), CustomResults.Problem);
        })
        .HasPermission(Permissions.OrganizationsAccess)
        .WithTags(Tags.Organizations);
    }
}
