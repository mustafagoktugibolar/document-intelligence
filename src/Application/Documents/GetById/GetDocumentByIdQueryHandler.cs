using Application.Abstractions.Authentication;
using Application.Abstractions.Data;
using Application.Abstractions.Documents;
using Application.Abstractions.Messaging;
using Domain.Documents;
using Microsoft.EntityFrameworkCore;
using SharedKernel;

namespace Application.Documents.GetById;

internal sealed class GetDocumentByIdQueryHandler(
    IApplicationDbContext context,
    IUserContext userContext,
    IDocumentAccessService documentAccessService)
    : IQueryHandler<GetDocumentByIdQuery, DocumentResponse>
{
    public async Task<Result<DocumentResponse>> Handle(
        GetDocumentByIdQuery query,
        CancellationToken cancellationToken)
    {
        bool canAccess = await documentAccessService.CanAccessDocumentAsync(
            userContext.UserId,
            query.DocumentId,
            cancellationToken);

        if (!canAccess)
        {
            return Result.Failure<DocumentResponse>(DocumentErrors.AccessDenied);
        }

        DocumentResponse? response = await context.Documents
            .Where(d => d.Id == query.DocumentId)
            .Select(d => new DocumentResponse
            {
                Id = d.Id,
                OrganizationId = d.OrganizationId,
                UploadedByUserId = d.UploadedByUserId,
                FileName = d.FileName,
                FileSizeBytes = d.FileSizeBytes,
                Status = d.Status,
                Visibility = d.Visibility,
                TotalChunks = d.TotalChunks,
                UploadedAt = d.UploadedAt,
                Sections = d.Sections.Select(s => new DocumentSectionResponse
                {
                    Id = s.Id,
                    Name = s.Name,
                    StartChunkIndex = s.StartChunkIndex,
                    EndChunkIndex = s.EndChunkIndex,
                    Visibility = s.Visibility
                }).ToList(),
                Summary = d.Summary,
                Classification = d.Classification
            })
            .FirstOrDefaultAsync(cancellationToken);

        if (response is null)
        {
            return Result.Failure<DocumentResponse>(DocumentErrors.NotFound(query.DocumentId));
        }

        return response;
    }
}
