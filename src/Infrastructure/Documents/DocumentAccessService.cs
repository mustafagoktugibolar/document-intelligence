using Application.Abstractions.Documents;
using Application.Abstractions.Data;
using Domain.Documents;
using Domain.Organizations;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Documents;

internal sealed class DocumentAccessService(IApplicationDbContext dbContext) : IDocumentAccessService
{
    public async Task<bool> CanAccessDocumentAsync(
        Guid userId,
        Guid documentId,
        CancellationToken cancellationToken = default)
    {
        Document? document = await dbContext.Documents
            .Include(d => d.AccessEntries)
            .FirstOrDefaultAsync(d => d.Id == documentId, cancellationToken);

        if (document is null)
        {
            return false;
        }

        OrganizationMember? membership = await dbContext.OrganizationMembers
            .FirstOrDefaultAsync(
                m => m.UserId == userId && m.OrganizationId == document.OrganizationId,
                cancellationToken);

        if (membership is null)
        {
            return false;
        }

        if (document.Visibility == DocumentVisibility.OrgWide)
        {
            return true;
        }

        return document.AccessEntries.Any(e => e.Matches(userId, membership.Role));
    }

    public async Task<bool> CanAccessSectionAsync(
        Guid userId,
        Guid sectionId,
        CancellationToken cancellationToken = default)
    {
        DocumentSection? section = await dbContext.DocumentSections
            .Include(s => s.AccessEntries)
            .FirstOrDefaultAsync(s => s.Id == sectionId, cancellationToken);

        if (section is null)
        {
            return false;
        }

        if (section.Visibility == SectionVisibility.InheritFromDocument)
        {
            return await CanAccessDocumentAsync(userId, section.DocumentId, cancellationToken);
        }

        OrganizationMember? membership = await dbContext.OrganizationMembers
            .Join(
                dbContext.Documents,
                m => m.OrganizationId,
                d => d.OrganizationId,
                (m, d) => new { m, d })
            .Where(x => x.m.UserId == userId && x.d.Id == section.DocumentId)
            .Select(x => x.m)
            .FirstOrDefaultAsync(cancellationToken);

        if (membership is null)
        {
            return false;
        }

        return section.AccessEntries.Any(e => e.Matches(userId, membership.Role));
    }

    public async Task<IReadOnlyList<int>> GetAccessibleChunkIndicesAsync(
        Guid userId,
        Guid documentId,
        CancellationToken cancellationToken = default)
    {
        Document? document = await dbContext.Documents
            .Include(d => d.Sections)
                .ThenInclude(s => s.AccessEntries)
            .FirstOrDefaultAsync(d => d.Id == documentId, cancellationToken);

        if (document is null || document.TotalChunks == 0)
        {
            return [];
        }

        OrganizationMember? membership = await dbContext.OrganizationMembers
            .FirstOrDefaultAsync(
                m => m.UserId == userId && m.OrganizationId == document.OrganizationId,
                cancellationToken);

        if (membership is null)
        {
            return [];
        }

        HashSet<int> blockedIndices = [];

        foreach (DocumentSection section in document.Sections)
        {
            if (section.Visibility != SectionVisibility.Restricted)
            {
                continue;
            }

            bool hasAccess = section.AccessEntries.Any(e => e.Matches(userId, membership.Role));
            if (!hasAccess)
            {
                for (int i = section.StartChunkIndex; i <= section.EndChunkIndex; i++)
                {
                    blockedIndices.Add(i);
                }
            }
        }

        List<int> accessible = [];
        for (int i = 0; i < document.TotalChunks; i++)
        {
            if (!blockedIndices.Contains(i))
            {
                accessible.Add(i);
            }
        }

        return accessible;
    }
}
