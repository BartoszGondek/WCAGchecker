using AccessAudit.ProjectManagement.Application.Repositories;
using AccessAudit.ProjectManagement.Domain.Entities;
using AccessAudit.ProjectManagement.Domain.ValueObjects;
using Microsoft.EntityFrameworkCore;

namespace AccessAudit.ProjectManagement.Infrastructure.Persistence.Repositories;

internal sealed class SqlAuditScopeRepository(ProjectManagementDbContext db) : IAuditScopeRepository
{
    public Task<AuditScope?> GetByIdAsync(Guid scopeId, CancellationToken cancellationToken = default)
    {
        var id = new AuditScopeId(scopeId);
        return db.AuditScopes.FirstOrDefaultAsync(s => s.Identity == id, cancellationToken);
    }

    public async Task<IReadOnlyCollection<AuditScope>> GetByProjectIdAsync(Guid projectId,
        CancellationToken cancellationToken = default)
    {
        var id = new ProjectId(projectId);
        return await db.AuditScopes.AsNoTracking().Where(s => s.ProjectIdentity == id)
            .OrderBy(s => s.CapturedAt).ToListAsync(cancellationToken);
    }

    public async Task SaveAsync(AuditScope scope, CancellationToken cancellationToken = default)
    {
        if (db.Entry(scope).State == EntityState.Detached)
            await db.AuditScopes.AddAsync(scope, cancellationToken);
        // The same scoped DbContext also persists the loaded project's DomExtracted state atomically.
        await db.SaveChangesAsync(cancellationToken);
    }
}
