using Domain.Documents;
using Domain.Organizations;
using Domain.Processing;
using Domain.Todos;
using Domain.Users;
using Microsoft.EntityFrameworkCore;

namespace Application.Abstractions.Data;

public interface IApplicationDbContext
{
    DbSet<User> Users { get; }
    DbSet<TodoItem> TodoItems { get; }
    DbSet<Organization> Organizations { get; }
    DbSet<OrganizationMember> OrganizationMembers { get; }
    DbSet<Document> Documents { get; }
    DbSet<DocumentSection> DocumentSections { get; }
    DbSet<ProcessingJob> ProcessingJobs { get; }

    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}
