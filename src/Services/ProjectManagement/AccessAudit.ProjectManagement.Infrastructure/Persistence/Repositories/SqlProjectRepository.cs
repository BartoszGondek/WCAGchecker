using AccessAudit.ProjectManagement.Application.Repositories;
using AccessAudit.ProjectManagement.Domain.Aggregates;
using AccessAudit.ProjectManagement.Domain.ValueObjects;
using Microsoft.EntityFrameworkCore;

namespace AccessAudit.ProjectManagement.Infrastructure.Persistence.Repositories;

internal sealed class SqlProjectRepository(ProjectManagementDbContext db) : IProjectRepository
{
    public Task<Project?> GetByIdAsync(Guid projectId, CancellationToken cancellationToken = default)
    {
        var id = new ProjectId(projectId);
        return db.Projects.FirstOrDefaultAsync(p => p.Identity == id, cancellationToken);
    }

    public async Task<IReadOnlyCollection<Project>> GetAllAsync(CancellationToken cancellationToken = default)
        => await db.Projects.AsNoTracking().OrderBy(p => p.CreatedAt).ThenBy(p => p.Identity).ToListAsync(cancellationToken);

    public async Task SaveAsync(Project project, CancellationToken cancellationToken = default)
    {
        if (db.Entry(project).State == EntityState.Detached)
            await db.Projects.AddAsync(project, cancellationToken);
        await db.SaveChangesAsync(cancellationToken);
    }

    public async Task DeleteAsync(Project project, CancellationToken cancellationToken = default)
    {
        // Explicit cleanup also gives the non-relational InMemory provider the same API behavior.
        var scopes = await db.AuditScopes.Where(s => s.ProjectIdentity == project.Identity).ToListAsync(cancellationToken);
        db.AuditScopes.RemoveRange(scopes);
        db.Projects.Remove(project);
        await db.SaveChangesAsync(cancellationToken);
    }
}
