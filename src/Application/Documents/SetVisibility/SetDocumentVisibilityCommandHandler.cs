using Application.Abstractions.Authentication;
using Application.Abstractions.Data;
using Application.Abstractions.Documents;
using Application.Abstractions.Messaging;
using Domain.Documents;
using Domain.Organizations;
using Microsoft.EntityFrameworkCore;
using SharedKernel;

namespace Application.Documents.SetVisibility;

internal sealed class SetDocumentVisibilityCommandHandler(
    IApplicationDbContext context,
    IUserContext userContext,
    IOrganizationMembershipService membershipService)
    : ICommandHandler<SetDocumentVisibilityCommand>
{
    public async Task<Result> Handle(SetDocumentVisibilityCommand command, CancellationToken cancellationToken)
    {
        Document? document = await context.Documents
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

        if (command.Visibility == DocumentVisibility.OrgWide)
        {
            document.MakeOrgWide();
        }
        else
        {
            document.MakeRestricted();
        }

        await context.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}
