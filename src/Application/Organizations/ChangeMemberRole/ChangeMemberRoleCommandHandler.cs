using Application.Abstractions.Authentication;
using Application.Abstractions.Data;
using Application.Abstractions.Messaging;
using Domain.Organizations;
using Microsoft.EntityFrameworkCore;
using SharedKernel;

namespace Application.Organizations.ChangeMemberRole;

internal sealed class ChangeMemberRoleCommandHandler(
    IApplicationDbContext context,
    IUserContext userContext,
    IOrganizationMembershipService membershipService)
    : ICommandHandler<ChangeMemberRoleCommand>
{
    public async Task<Result> Handle(ChangeMemberRoleCommand command, CancellationToken cancellationToken)
    {
        OrganizationRole? callerRole = await membershipService.GetRoleAsync(
            userContext.UserId,
            command.OrganizationId,
            cancellationToken);

        if (callerRole is null || callerRole < OrganizationRole.Admin)
        {
            return Result.Failure(OrganizationErrors.Unauthorized);
        }

        Organization? organization = await context.Organizations
            .Include(o => o.Members)
            .FirstOrDefaultAsync(o => o.Id == command.OrganizationId, cancellationToken);

        if (organization is null)
        {
            return Result.Failure(OrganizationErrors.NotFound(command.OrganizationId));
        }

        return organization.ChangeMemberRole(command.UserId, command.NewRole);
    }
}
