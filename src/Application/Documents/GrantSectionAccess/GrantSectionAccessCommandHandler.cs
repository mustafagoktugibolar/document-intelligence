using Application.Abstractions.Authentication;
using Application.Abstractions.Data;
using Application.Abstractions.Messaging;
using Domain.Documents;
using Domain.Organizations;
using Microsoft.EntityFrameworkCore;
using SharedKernel;

namespace Application.Documents.GrantSectionAccess;

internal sealed class GrantSectionAccessCommandHandler(
    IApplicationDbContext context,
    IUserContext userContext,
    IOrganizationMembershipService membershipService)
    : ICommandHandler<GrantSectionAccessCommand>
{
    public async Task<Result> Handle(GrantSectionAccessCommand command, CancellationToken cancellationToken)
    {
        DocumentSection? section = await context.DocumentSections
            .Include(s => s.AccessEntries)
            .FirstOrDefaultAsync(s => s.Id == command.SectionId, cancellationToken);

        if (section is null)
        {
            return Result.Failure(DocumentErrors.SectionNotFound(command.SectionId));
        }

        Document? document = await context.Documents
            .FirstOrDefaultAsync(d => d.Id == section.DocumentId, cancellationToken);

        if (document is null)
        {
            return Result.Failure(DocumentErrors.NotFound(section.DocumentId));
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

        return section.GrantAccess(entry);
    }
}
