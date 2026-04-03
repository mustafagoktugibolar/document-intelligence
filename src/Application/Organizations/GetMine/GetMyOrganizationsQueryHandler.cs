using Application.Abstractions.Authentication;
using Application.Abstractions.Data;
using Application.Abstractions.Messaging;
using Microsoft.EntityFrameworkCore;
using SharedKernel;

namespace Application.Organizations.GetMine;

internal sealed class GetMyOrganizationsQueryHandler(
    IApplicationDbContext context,
    IUserContext userContext)
    : IQueryHandler<GetMyOrganizationsQuery, List<OrganizationSummaryResponse>>
{
    public async Task<Result<List<OrganizationSummaryResponse>>> Handle(
        GetMyOrganizationsQuery query,
        CancellationToken cancellationToken)
    {
        List<OrganizationSummaryResponse> result = await context.OrganizationMembers
            .Where(m => m.UserId == userContext.UserId)
            .Join(
                context.Organizations,
                m => m.OrganizationId,
                o => o.Id,
                (m, o) => new OrganizationSummaryResponse
                {
                    Id = o.Id,
                    Name = o.Name,
                    MyRole = m.Role,
                    MemberCount = o.Members.Count
                })
            .ToListAsync(cancellationToken);

        return result;
    }
}
