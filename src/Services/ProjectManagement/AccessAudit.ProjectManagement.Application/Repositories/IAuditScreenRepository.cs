using AccessAudit.ProjectManagement.Domain.Aggregates;

namespace AccessAudit.ProjectManagement.Application.Repositories;

public interface IAuditScreenRepository
{
    Task<AuditScreen?> GetByIdAsync(Guid screenId, CancellationToken cancellationToken = default);
    Task<IReadOnlyCollection<AuditScreen>> GetByProjectIdAsync(Guid projectId, CancellationToken cancellationToken = default);
    Task SaveAsync(AuditScreen screen, CancellationToken cancellationToken = default);
}
