using Application.Abstractions.Authentication;
using Application.Abstractions.Data;
using Application.Abstractions.Messaging;
using Domain.Documents;
using Domain.Organizations;
using Microsoft.EntityFrameworkCore;
using SharedKernel;

namespace Application.Documents.AddSection;

internal sealed class AddDocumentSectionCommandHandler(
    IApplicationDbContext context,
    IUserContext userContext,
    IOrganizationMembershipService membershipService)
    : ICommandHandler<AddDocumentSectionCommand, Guid>
{
    public async Task<Result<Guid>> Handle(AddDocumentSectionCommand command, CancellationToken cancellationToken)
    {
        Document? document = await context.Documents
            .Include(d => d.Sections)
            .FirstOrDefaultAsync(d => d.Id == command.DocumentId, cancellationToken);

        if (document is null)
        {
            return Result.Failure<Guid>(DocumentErrors.NotFound(command.DocumentId));
        }

        OrganizationRole? callerRole = await membershipService.GetRoleAsync(
            userContext.UserId,
            document.OrganizationId,
            cancellationToken);

        if (callerRole is null || callerRole < OrganizationRole.Editor)
        {
            return Result.Failure<Guid>(OrganizationErrors.Unauthorized);
        }

        Result result = document.AddSection(
            command.Name,
            command.StartChunkIndex,
            command.EndChunkIndex,
            command.Visibility,
            command.SourceTag);

        if (result.IsFailure)
        {
            return Result.Failure<Guid>(result.Error);
        }

        await context.SaveChangesAsync(cancellationToken);

        DocumentSection section = document.Sections[^1];
        return section.Id;
    }
}
