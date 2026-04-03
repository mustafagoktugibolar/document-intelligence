using Application.Abstractions.Authentication;
using Application.Abstractions.Data;
using Application.Abstractions.Messaging;
using Domain.Documents;
using Domain.Organizations;
using SharedKernel;

namespace Application.Documents.Upload;

internal sealed class UploadDocumentCommandHandler(
    IApplicationDbContext context,
    IUserContext userContext,
    IOrganizationMembershipService membershipService)
    : ICommandHandler<UploadDocumentCommand, Guid>
{
    public async Task<Result<Guid>> Handle(UploadDocumentCommand command, CancellationToken cancellationToken)
    {
        OrganizationRole? callerRole = await membershipService.GetRoleAsync(
            userContext.UserId,
            command.OrganizationId,
            cancellationToken);

        if (callerRole is null || callerRole < OrganizationRole.Editor)
        {
            return Result.Failure<Guid>(OrganizationErrors.Unauthorized);
        }

        var document = Document.Create(
            command.OrganizationId,
            userContext.UserId,
            command.FileName,
            command.StorageKey,
            command.FileSizeBytes);

        context.Documents.Add(document);

        await context.SaveChangesAsync(cancellationToken);

        return document.Id;
    }
}
