using Application.Abstractions.Authentication;
using Application.Abstractions.Data;
using Application.Abstractions.Messaging;
using Domain.Organizations;
using Microsoft.EntityFrameworkCore;
using SharedKernel;

namespace Application.Organizations.GetById;

internal sealed class GetOrganizationByIdQueryHandler(
    IApplicationDbContext context,
    IUserContext userContext,
    IOrganizationMembershipService membershipService)
    : IQueryHandler<GetOrganizationByIdQuery, OrganizationResponse>
{
    public async Task<Result<OrganizationResponse>> Handle(
        GetOrganizationByIdQuery query,
        CancellationToken cancellationToken)
    {
        bool isMember = await membershipService.IsMemberAsync(
            userContext.UserId,
            query.OrganizationId,
            cancellationToken);

        if (!isMember)
        {
            return Result.Failure<OrganizationResponse>(OrganizationErrors.Unauthorized);
        }

        OrganizationResponse? response = await context.Organizations
            .Where(o => o.Id == query.OrganizationId)
            .Select(o => new OrganizationResponse
            {
                Id = o.Id,
                Name = o.Name,
                CreatedAt = o.CreatedAt,
                Members = o.Members.Select(m => new OrganizationMemberResponse
                {
                    UserId = m.UserId,
                    Role = m.Role,
                    JoinedAt = m.JoinedAt
                }).ToList()
            })
            .FirstOrDefaultAsync(cancellationToken);

        if (response is null)
        {
            return Result.Failure<OrganizationResponse>(OrganizationErrors.NotFound(query.OrganizationId));
        }

        return response;
    }
}
