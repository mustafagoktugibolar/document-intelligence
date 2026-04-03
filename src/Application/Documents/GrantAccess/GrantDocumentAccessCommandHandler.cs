using Application.Abstractions.Authentication;
using Application.Abstractions.Data;
using Application.Abstractions.Messaging;
using Domain.Documents;
using Domain.Organizations;
using Microsoft.EntityFrameworkCore;
using SharedKernel;

namespace Application.Documents.GrantAccess;

internal sealed class GrantDocumentAccessCommandHandler(
    IApplicationDbContext context,
    IUserContext userContext,
    IOrganizationMembershipService membershipService)
    : ICommandHandler<GrantDocumentAccessCommand>
{
    public async Task<Result> Handle(GrantDocumentAccessCommand command, CancellationToken cancellationToken)
    {
        Document? document = await context.Documents
            .Include(d => d.AccessEntries)
            .FirstOrDefaultAsync(d => d.Id == command.DocumentId, cancellationToken);

        if (document is null)
        {
            return Result.Failure(DocumentErrors.NotFound(command.DocumentId));
        }

        OrganizationRole? callerRole = await membershipService.GetRoleAsync(
            userContext.UserId,
            document.OrganizationId,
            cancellationToken);

        if (callerRole is null || callerRole < OrganizationRole.Admin)
        {
            return Result.Failure(OrganizationErrors.Unauthorized);
        }

        AccessEntry entry = command.PrincipalType == PrincipalType.User
            ? AccessEntry.ForUser(Guid.Parse(command.PrincipalId), command.Permission)
            : AccessEntry.ForRole(Enum.Parse<OrganizationRole>(command.PrincipalId), command.Permission);

        Result result = document.GrantAccess(entry);
        if (result.IsFailure)
        {
            return result;
        }

        await context.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}
