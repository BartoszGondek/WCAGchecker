using AccessAudit.ProjectManagement.Domain.Entities;

namespace AccessAudit.ProjectManagement.Application.Repositories;

public interface IAuditScopeRepository
{
    Task<AuditScope?> GetByIdAsync(Guid scopeId, CancellationToken cancellationToken = default);
    Task<IReadOnlyCollection<AuditScope>> GetByProjectIdAsync(Guid projectId, CancellationToken cancellationToken = default);
    Task SaveAsync(AuditScope scope, CancellationToken cancellationToken = default);
}
