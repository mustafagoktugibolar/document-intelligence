using Application.Abstractions.Messaging;
using Application.Organizations.GetMine;
using SharedKernel;
using Web.Api.Extensions;
using Web.Api.Infrastructure;

namespace Web.Api.Endpoints.Organizations;

internal sealed class GetMine : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapGet("organizations/mine", async (
            IQueryHandler<GetMyOrganizationsQuery, List<OrganizationSummaryResponse>> handler,
            CancellationToken cancellationToken) =>
        {
            var query = new GetMyOrganizationsQuery();

            Result<List<OrganizationSummaryResponse>> result = await handler.Handle(query, cancellationToken);

            return result.Match(Results.Ok, CustomResults.Problem);
        })
        .HasPermission(Permissions.OrganizationsAccess)
        .WithTags(Tags.Organizations);
    }
}
