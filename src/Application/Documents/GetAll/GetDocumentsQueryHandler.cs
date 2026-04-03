using Application.Abstractions.Authentication;
using Application.Abstractions.Data;
using Application.Abstractions.Messaging;
using Domain.Documents;
using Domain.Organizations;
using Microsoft.EntityFrameworkCore;
using SharedKernel;

namespace Application.Documents.GetAll;

internal sealed class GetDocumentsQueryHandler(
    IApplicationDbContext context,
    IUserContext userContext,
    IOrganizationMembershipService membershipService)
    : IQueryHandler<GetDocumentsQuery, List<DocumentSummaryResponse>>
{
    public async Task<Result<List<DocumentSummaryResponse>>> Handle(
        GetDocumentsQuery query,
        CancellationToken cancellationToken)
    {
        OrganizationRole? callerRole = await membershipService.GetRoleAsync(
            userContext.UserId,
            query.OrganizationId,
            cancellationToken);

        if (callerRole is null)
        {
            return Result.Failure<List<DocumentSummaryResponse>>(OrganizationErrors.Unauthorized);
        }

        IQueryable<Document> documentsQuery = context.Documents
            .Where(d => d.OrganizationId == query.OrganizationId);

        if (callerRole < OrganizationRole.Admin)
        {
            string userId = userContext.UserId.ToString();
            documentsQuery = documentsQuery.Where(d =>
                d.Visibility == DocumentVisibility.OrgWide ||
                d.AccessEntries.Any(e =>
                    e.PrincipalType == PrincipalType.User && e.PrincipalId == userId));
        }

        List<DocumentSummaryResponse> result = await documentsQuery
            .Select(d => new DocumentSummaryResponse
            {
                Id = d.Id,
                FileName = d.FileName,
                FileSizeBytes = d.FileSizeBytes,
                Status = d.Status,
                Visibility = d.Visibility,
                UploadedAt = d.UploadedAt
            })
            .ToListAsync(cancellationToken);

        return result;
    }
}
